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

/// <summary>
/// 回合制对战系统的管理脚本
/// </summary>
public class BattleSystemMgr : MonoSingleton<BattleSystemMgr>
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

    //private Dic

    [SerializeField]
    private BattleType _battleType;

    public PlayerInfo _playerInfo;
    public EnermyInfo _enermyInfo;
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
                //初始化卡牌
                StartCoroutine(InitBattleCard());
                //初始化双方信息
                InitPlayerInfo();
                break;
            case BattleType.PlayerTurn:
                //显示菜单栏
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_MENU);
                //自动发牌
                GetCard();
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);
                //进行血量判断
                break;
            case BattleType.EnermyTurn:
                //先判断手牌是否大于5张，是的话则需要进行卡牌的打出进行消耗
                if (_cardsInHand.Count > 5)
                {
                    ToastManager.Instance?.CreatToast("手牌数量不能超过5张！");
                    return;
                }
                //关闭菜单栏
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_CLOSE_MENU);
                //进行血量判断
                Debug.Log("敌方回合");
                break;
            case BattleType.Winner:
                Debug.Log("player win");
                break;
            case BattleType.Lose:
                Debug.Log("player lose");
                break;
            case BattleType.End:
                Debug.Log("battle is over");
                break;
        }
    }
    private void InitPlayerInfo()
    {
        //从本地读取数据

        Dictionary<int, CharacterInfo> characterDicts = CsvManager.Instance?.ReadCharacterInfoCSVFile();
        _playerInfo = (PlayerInfo)characterDicts[0];
        _enermyInfo = (EnermyInfo)characterDicts[1];
    }
    public IEnumerator CreateCardGo(int cardMax)
    {
        //执行完成之前不允许使用卡组
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_INACTIVATE_CARDSINHAND);
        string assetsName = "BattleCard";
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
            gameObject1.GetComponent<CardTurnOver>().StartFront();
            Transform _cardFront = gameObject1.transform.Find("CardFront");
            _cardFront.AddComponent<CardItem>();
            CardInfoBean cardInfoBean = CreateCardInfoBean(cardinfos.ElementAt(_temIndex).Value);
            //_cardFront.GetComponent<CardItem>()._index = i;
            _cardFront.GetComponent<CardItem>().Init(cardInfoBean);

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
        CardItem cardItem = cardGo.GetComponentInChildren<CardItem>();
        CardInfoBean cardInfo = cardItem._cardInfo;
        switch (cardInfo.type)
        {
            case CardType.Atk:
                //直接造成伤害
                //计算伤害
                float temAtk = (float)_playerInfo._curAtk;
                _enermyInfo._curHP -= temAtk;
                string log = $"玩家使用卡片对敌人造成了{cardInfo.value}点的伤害";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.AtkUp:
                _playerInfo._curAtk += cardInfo.value;
                log = $"玩家使用卡片提升自身{cardInfo.value}点的攻击力";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.AtkDown:
                _enermyInfo._curAtk -= cardInfo.value;
                log = $"玩家使用卡片降低敌方{cardInfo.value}点的攻击力";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.DefUp:
                _playerInfo._curDef += cardInfo.value;
                log = $"玩家使用卡片提升自身{cardInfo.value}点的防御力";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.DefDown:
                _enermyInfo._curDef -= cardInfo.value;
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
                _playerInfo._curHP += cardInfo.value;
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
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);

        //对敌我双方进行血量判断，查看当前是否满足胜利/失败条件
        CheckIsWin();
    }
    void CheckIsWin()
    {
        if (_enermyInfo._curHP<=0)
        {
            //胜利
            SwitchBattleType(BattleType.Winner);
            return;
        }
        if (_playerInfo._curHP<=0)
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
            CardItem cardItem = cardGos[i].GetComponentInChildren<CardItem>();
            CardInfoBean cardInfo = cardItem._cardInfo;
            switch (cardInfo.type)
            {
                case CardType.Atk:
                    //直接造成伤害
                    //计算伤害
                    float temAtk = _playerInfo._curAtk * 1.2f;
                    _enermyInfo._curHP -= temAtk;
                    string log = $"玩家使用卡片对敌人造成了{temAtk}点的伤害";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.AtkUp:
                    _playerInfo._curAtk += cardInfo.value * 1.2f;
                    log = $"玩家使用卡片提升自身{cardInfo.value * 1.2f}点的攻击力";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.AtkDown:
                    _enermyInfo._curAtk -= cardInfo.value * 1.2f;
                    log = $"玩家使用卡片降低敌方{cardInfo.value * 1.2f}点的攻击力";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.DefUp:
                    _playerInfo._curDef += cardInfo.value * 1.2f;
                    log = $"玩家使用卡片提升自身{cardInfo.value * 1.2f}点的防御力";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.DefDown:
                    _enermyInfo._curDef -= cardInfo.value * 1.2f;
                    log = $"玩家使用卡片降低敌方{cardInfo.value * 1.2f}点的防御力";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.Sleep:
                    log = $"玩家使用卡片让敌方睡眠跳过一回合";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.Cover:
                    _playerInfo._curHP += cardInfo.value * 1.2f;
                    log = $"玩家使用卡片回复我方{cardInfo.value * 1.2f}点的血量";
                    break;
                case CardType.None:
                    log = $"玩家使用卡片....无事发生";
                    Debug.Log(log);
                    //text.text = log;
                    break;
            }
            //调用卡牌自己的消失功能
            cardItem.Disappear();
            EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);
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
[Serializable]
public class CharacterInfo
{
    public int _id;
    public string _name;
    public string _description;
    public int _level;
    public int _type;
    public float _maxHP;
    public float _curHP;
    public float _oriAtk;
    public float _curAtk;
    public float _oriDef;
    public float _curDef;
}
[Serializable]
public class PlayerInfo : CharacterInfo
{
}
[Serializable]
public class EnermyInfo : CharacterInfo
{
}