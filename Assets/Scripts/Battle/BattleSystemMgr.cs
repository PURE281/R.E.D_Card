using RandomElementsSystem.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EnumMgr;
using DG.Tweening;
using Sequence = DG.Tweening.Sequence;
/// <summary>
/// 回合制对战系统的管理脚本
/// </summary>
public class BattleSystemMgr : MonoSington<BattleSystemMgr>
{
    [SerializeField]
    /// <summary>
    /// 储存当前现场卡牌的对象
    /// </summary>
    private List<GameObject> _cardsInHand = new List<GameObject>();

    /// <summary>
    /// 储存所有可使用的卡牌信息的对象
    /// </summary>
    private Dictionary<string, CardInfoBean> cardinfos = new Dictionary<string, CardInfoBean>();


    [SerializeField]
    private BattleType _battleType;

    private GameObject _enermyGo;
    private GameObject _playerGo;

    public List<GameObject> CardsInHand { get => _cardsInHand; }
    public BattleType BattleType { get => _battleType; }

    // Start is called before the first frame update
    void Start()
    {
        _battleType = BattleType.Init;
        SwitchBattleType(BattleType.Init);
        //SwitchBattleType(BattleType.PlayerTurn);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetCard()
    {
        if (_cardsInHand.Count > 5)
        {
            //说明不满足条件，不允许获取卡片
            ToastManager.Instance?.CreatToast("请保证手牌不超过5张");
            return;
        }
        StartCoroutine(GetCardIE());
    }
    IEnumerator GetCardIE()
    {
        //加载卡片信息
        yield return CreateCardGo(3);
    }
    IEnumerator InitBattleCard()
    {
        //加载所有卡牌信息
        cardinfos = CsvManager.Instance?.ReadCardInfoCSVFile();

        // 步骤1: 将Dictionary的键值对添加到列表中  
        List<KeyValuePair<string, CardInfoBean>> cardList = cardinfos.ToList();

        // 步骤2: 随机打乱列表  
        ShuffleList(cardList);
        yield return StartCoroutine(CreateCardGo(5));
        //RollCards();
    }

    CardInfoBean CreateCardInfoBean(CardInfoBean bean)
    {

        CardInfoBean cardInfo = new CardInfoBean();
        cardInfo.id = bean.id;
        cardInfo.name = bean.name;
        cardInfo.cast = bean.cast;
        cardInfo.value = bean.value;
        cardInfo.type = (bean.type);
        cardInfo.description = bean.description;
        cardInfo.clipPath = bean.clipPath;
        cardInfo.spritePath = bean.spritePath;
        cardInfo.upgrade_id = bean.upgrade_id;
        cardInfo.combo_id = bean.combo_id;
        cardInfo.fusion_id = bean.fusion_id;
        cardInfo.proficiency = bean.proficiency;
        cardInfo.probability = bean.probability;
        return cardInfo;
    }
    void ShuffleList(List<KeyValuePair<string, CardInfoBean>> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1); // 注意Unity的Random.Range是包含上限的  
            KeyValuePair<string, CardInfoBean> temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
        var shuffledCardDict = new Dictionary<string, CardInfoBean>(list.Count);
        foreach (var kvp in list)
        {
            shuffledCardDict.Add(kvp.Key, kvp.Value);
        }

        cardinfos = shuffledCardDict;
    }

    public void SwitchBattleType(BattleType battleType)
    {
        _battleType = battleType;
        switch (battleType)
        {
            case BattleType.Init:
                //初始化双方信息
                InitPlayerInfo();
                //初始化卡牌
                StartCoroutine(InitBattleCard());
                break;
            case BattleType.PlayerTurn:
                Sequence sequence = DOTween.Sequence();
                sequence.AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    //显示菜单栏
                    EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_MENU);
                    //自动发牌
                    GetCard();
                    EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);
                    //进行血量判断
                });
                break;
            case BattleType.EnermyTurn:
                //先判断手牌是否大于5张，是的话则需要进行卡牌的打出进行消耗
                if (_cardsInHand.Count > 5)
                {
                    ToastManager.Instance?.CreatToast("手牌数量不能超过5张！");
                    return;
                }
                //进行血量判断
                Debug.Log("敌方回合");
                //关闭菜单栏
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_CLOSE_MENU);
                //添加延迟

                //随机造成一定伤害
                BattleEnermyInfo.Instance?.Attack(() =>
                {
                    SwitchBattleType(BattleType.PlayerTurn);
                });
                break;
            case BattleType.Winner:
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_PLAYER_WIN);
                Debug.Log("player win");
                break;
            case BattleType.Lose:
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_PLAYER_LOSE);
                Debug.Log("player lose");
                break;
            case BattleType.End:
                Debug.Log("battle is over>>需要弹出重新还是下一局的选项");
                break;
        }
    }
    private void InitPlayerInfo()
    {
        //从本地读取数据
        Dictionary<int, CharacterBean> characterDicts = CsvManager.Instance?.ReadCharacterInfoCSVFile();
        //初始化角色go
        GameObject _templayer = Resources.Load<GameObject>("Prefabs/BattlePlayerGo");
        GameObject _temEnermy = Resources.Load<GameObject>("Prefabs/BattleEnermyGo");
        _playerGo = Instantiate(_templayer);
        _playerGo.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("PC"));
        _playerGo.transform.localPosition = Vector3.zero;
        _playerGo.transform.SetAsFirstSibling();
        _playerGo.name = "BattlePlayerGo";
        _enermyGo = Instantiate(_temEnermy);
        _enermyGo.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("PC"));
        _enermyGo.transform.SetAsFirstSibling();
        _enermyGo.transform.localPosition = Vector3.zero;
        _enermyGo.name = "BattleEnermyGo";
        BattlePlayerBean battlePlayerBean = (BattlePlayerBean)characterDicts[0];
        _playerGo.AddComponent<BattlePlayerInfo>();
        _playerGo.GetComponent<BattlePlayerInfo>().InitInfo(battlePlayerBean);
        BattleEnermyBean battleEnermyBean = (BattleEnermyBean)characterDicts[1];
        _enermyGo.AddComponent<BattleEnermyInfo>();
        _enermyGo.GetComponent<BattleEnermyInfo>().InitInfo(battleEnermyBean);
    }
    public IEnumerator CreateCardGo(int cardMax)
    {
        //执行完成之前不允许使用卡组
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_INACTIVATE_CARDSINHAND);
        string assetsName = "BattleCard1";
        GameObject cardGo = Resources.Load<GameObject>($"Prefabs/{assetsName}");
        for (int i = 0; i < cardMax; i++)
        {

            //初始化go预制体
            GameObject gameObject1 = Instantiate(cardGo);
            yield return new WaitForSeconds(0.5f);
            gameObject1.transform.SetParent(GameObject.FindWithTag("MainCanvas").transform.Find("PC/CardGroup/Panel"));
            //gameObject1.transform.localScale = Vector3.one;
            //给预制体添加卡片信息
            int _temIndex = new MinMaxRandomInt(0, cardinfos.Count).GetRandomValue();
            //Transform _cardFront = gameObject1.transform.Find("CardFront");
            gameObject1.AddComponent<BattleCardItem>();
            gameObject1.GetComponent<BattleCardItem>().StartFront();
            CardInfoBean cardInfoBean = CreateCardInfoBean(cardinfos.ElementAt(_temIndex).Value);
            //_cardFront.GetComponent<CardItem>()._index = i;
            gameObject1.GetComponent<BattleCardItem>().Init(cardInfoBean);

            CardsInHand.Add(gameObject1);

        }
        //执行完成之后再允许使用卡组
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_ACTIVATE_CARDSINHAND);
    }
    public void RemoveCardInHand(GameObject cardGo)
    {
        _cardsInHand.Remove(cardGo);
    }
    public void HandleCard(GameObject cardGo)
    {
        //需要有一点简单的效果
        //卡片中的图片从左边渐变放大，像右边移动，然后慢慢消失
        Action action = new Action(() =>
        {
            //同时文字提醒，同时对应的效果发动
            BattleCardItem cardItem = cardGo.GetComponent<BattleCardItem>();
            CardInfoBean cardInfo = cardItem._cardInfo;
            switch (cardInfo.type)
            {
                case CardType.Atk:
                    //直接造成伤害
                    //计算伤害
                    float temAtk = (float)BattlePlayerInfo.Instance?.Character._curAtk;
                    BattleEnermyInfo.Instance?.UpdateHp(-(cardInfo.value + temAtk));
                    string log = $"玩家使用卡片对敌人造成了{cardInfo.value}点的伤害";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.AtkUp:
                    BattlePlayerInfo.Instance?.UpdateAtk(cardInfo.value);
                    log = $"玩家使用卡片提升自身{cardInfo.value}点的攻击力";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.AtkDown:
                    BattleEnermyInfo.Instance?.UpdateAtk(-cardInfo.value);
                    log = $"玩家使用卡片降低敌方{cardInfo.value}点的攻击力";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.DefUp:
                    BattlePlayerInfo.Instance?.UpdateDef(cardInfo.value);
                    log = $"玩家使用卡片提升自身{cardInfo.value}点的防御力";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.DefDown:
                    BattleEnermyInfo.Instance?.UpdateDef(-cardInfo.value);
                    log = $"玩家使用卡片降低敌方{cardInfo.value}点的防御力";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.Sleep:
                    log = $"玩家使用卡片让敌方睡眠跳过一回合";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.Cover:
                    BattlePlayerInfo.Instance?.UpdateHp(cardInfo.value);
                    log = $"玩家使用卡片回复我方{cardInfo.value}点的血量";
                    break;
                case CardType.None:
                    log = $"玩家使用卡片....无事发生";
                    Debug.Log(log);
                    //text.text = log;
                    break;
            }
            //调用卡牌自己的消失功能
            cardItem.Disappear();

            //对敌我双方进行血量判断，查看当前是否满足胜利/失败条件
            CheckIsWin();
        });
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_CARD_EFFECT, action);
    }
    void CheckIsWin()
    {
        if (BattleEnermyInfo.Instance?.Character._curHP <= 0)
        {
            //胜利
            SwitchBattleType(BattleType.Winner);
            return;
        }
        if (BattlePlayerInfo.Instance?.Character._curHP <= 0)
        {
            //失败
            SwitchBattleType(BattleType.Lose);
            return;
        }
    }
    public void HandleComboCards(List<GameObject> cardGos)
    {
        for (int i = 0; i < cardGos.Count; i++)
        {
            Action action = new Action(() =>
            {
                BattleCardItem cardItem = cardGos[i].GetComponent<BattleCardItem>();
                CardInfoBean cardInfo = cardItem._cardInfo;
                switch (cardInfo.type)
                {
                    case CardType.Atk:
                        //直接造成伤害
                        //计算伤害
                        float temAtk = (float)BattlePlayerInfo.Instance?.Character._curAtk;
                        BattleEnermyInfo.Instance?.UpdateHp(-(cardInfo.value + temAtk) * 1.2f);
                        string log = $"玩家使用卡片对敌人造成了{cardInfo.value}点的伤害";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.AtkUp:
                        BattlePlayerInfo.Instance?.UpdateAtk(cardInfo.value * 1.2f);
                        log = $"玩家使用卡片提升自身{cardInfo.value}点的攻击力";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.AtkDown:
                        BattleEnermyInfo.Instance?.UpdateAtk(-cardInfo.value * 1.2f);
                        log = $"玩家使用卡片降低敌方{cardInfo.value}点的攻击力";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.DefUp:
                        BattlePlayerInfo.Instance?.UpdateDef(cardInfo.value * 1.2f);
                        log = $"玩家使用卡片提升自身{cardInfo.value}点的防御力";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.DefDown:
                        BattleEnermyInfo.Instance?.UpdateDef(-cardInfo.value);
                        log = $"玩家使用卡片降低敌方{cardInfo.value}点的防御力";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.Sleep:
                        log = $"玩家使用卡片让敌方睡眠跳过一回合";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.Cover:
                        BattlePlayerInfo.Instance?.UpdateHp(cardInfo.value * 1.2f);
                        log = $"玩家使用卡片回复我方{cardInfo.value}点的血量";
                        break;
                    case CardType.None:
                        log = $"玩家使用卡片....无事发生";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                }
                //调用卡牌自己的消失功能
                cardItem.Disappear();
                //对敌我双方进行血量判断，查看当前是否满足胜利/失败条件
                CheckIsWin();
            });
            EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);
            EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_CARD_EFFECT, action);
        }

    }

    public CardInfoBean LoadCardItemById(string id)
    {
        if (!cardinfos.ContainsKey(id))
        {
            Debug.LogError("找不到相应id的卡片信息");
            return null;
        }
        CardInfoBean bean = cardinfos[id];
        return bean;
    }
    public void ToMainScene()
    {
        //AssetsBundlesMgr.Instance?.UnloadAllAssetBundles();
        SceneManager.LoadScene("MainScene");
    }
}