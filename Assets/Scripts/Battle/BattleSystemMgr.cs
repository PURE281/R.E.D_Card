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
/// �غ��ƶ�սϵͳ�Ĺ���ű�
/// </summary>
public class BattleSystemMgr : MonoSington<BattleSystemMgr>
{
    [SerializeField]
    /// <summary>
    /// ���浱ǰ�ֳ����ƵĶ���
    /// </summary>
    private List<GameObject> _cardsInHand = new List<GameObject>();

    /// <summary>
    /// �������п�ʹ�õĿ�����Ϣ�Ķ���
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
            //˵���������������������ȡ��Ƭ
            ToastManager.Instance?.CreatToast("�뱣֤���Ʋ�����5��");
            return;
        }
        StartCoroutine(GetCardIE());
    }
    IEnumerator GetCardIE()
    {
        //���ؿ�Ƭ��Ϣ
        yield return CreateCardGo(3);
    }
    IEnumerator InitBattleCard()
    {
        //�������п�����Ϣ
        cardinfos = CsvManager.Instance?.ReadCardInfoCSVFile();

        // ����1: ��Dictionary�ļ�ֵ����ӵ��б���  
        List<KeyValuePair<string, CardInfoBean>> cardList = cardinfos.ToList();

        // ����2: ��������б�  
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
            int k = UnityEngine.Random.Range(0, n + 1); // ע��Unity��Random.Range�ǰ������޵�  
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
                //��ʼ��˫����Ϣ
                InitPlayerInfo();
                //��ʼ������
                StartCoroutine(InitBattleCard());
                break;
            case BattleType.PlayerTurn:
                Sequence sequence = DOTween.Sequence();
                sequence.AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    //��ʾ�˵���
                    EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_MENU);
                    //�Զ�����
                    GetCard();
                    EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);
                    //����Ѫ���ж�
                });
                break;
            case BattleType.EnermyTurn:
                //���ж������Ƿ����5�ţ��ǵĻ�����Ҫ���п��ƵĴ����������
                if (_cardsInHand.Count > 5)
                {
                    ToastManager.Instance?.CreatToast("�����������ܳ���5�ţ�");
                    return;
                }
                //����Ѫ���ж�
                Debug.Log("�з��غ�");
                //�رղ˵���
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_CLOSE_MENU);
                //����ӳ�

                //������һ���˺�
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
                Debug.Log("battle is over>>��Ҫ�������»�����һ�ֵ�ѡ��");
                break;
        }
    }
    private void InitPlayerInfo()
    {
        //�ӱ��ض�ȡ����
        Dictionary<int, CharacterBean> characterDicts = CsvManager.Instance?.ReadCharacterInfoCSVFile();
        //��ʼ����ɫgo
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
        //ִ�����֮ǰ������ʹ�ÿ���
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_INACTIVATE_CARDSINHAND);
        string assetsName = "BattleCard1";
        GameObject cardGo = Resources.Load<GameObject>($"Prefabs/{assetsName}");
        for (int i = 0; i < cardMax; i++)
        {

            //��ʼ��goԤ����
            GameObject gameObject1 = Instantiate(cardGo);
            yield return new WaitForSeconds(0.5f);
            gameObject1.transform.SetParent(GameObject.FindWithTag("MainCanvas").transform.Find("PC/CardGroup/Panel"));
            //gameObject1.transform.localScale = Vector3.one;
            //��Ԥ������ӿ�Ƭ��Ϣ
            int _temIndex = new MinMaxRandomInt(0, cardinfos.Count).GetRandomValue();
            //Transform _cardFront = gameObject1.transform.Find("CardFront");
            gameObject1.AddComponent<BattleCardItem>();
            gameObject1.GetComponent<BattleCardItem>().StartFront();
            CardInfoBean cardInfoBean = CreateCardInfoBean(cardinfos.ElementAt(_temIndex).Value);
            //_cardFront.GetComponent<CardItem>()._index = i;
            gameObject1.GetComponent<BattleCardItem>().Init(cardInfoBean);

            CardsInHand.Add(gameObject1);

        }
        //ִ�����֮��������ʹ�ÿ���
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_ACTIVATE_CARDSINHAND);
    }
    public void RemoveCardInHand(GameObject cardGo)
    {
        _cardsInHand.Remove(cardGo);
    }
    public void HandleCard(GameObject cardGo)
    {
        //��Ҫ��һ��򵥵�Ч��
        //��Ƭ�е�ͼƬ����߽���Ŵ����ұ��ƶ���Ȼ��������ʧ
        Action action = new Action(() =>
        {
            //ͬʱ�������ѣ�ͬʱ��Ӧ��Ч������
            BattleCardItem cardItem = cardGo.GetComponent<BattleCardItem>();
            CardInfoBean cardInfo = cardItem._cardInfo;
            switch (cardInfo.type)
            {
                case CardType.Atk:
                    //ֱ������˺�
                    //�����˺�
                    float temAtk = (float)BattlePlayerInfo.Instance?.Character._curAtk;
                    BattleEnermyInfo.Instance?.UpdateHp(-(cardInfo.value + temAtk));
                    string log = $"���ʹ�ÿ�Ƭ�Ե��������{cardInfo.value}����˺�";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.AtkUp:
                    BattlePlayerInfo.Instance?.UpdateAtk(cardInfo.value);
                    log = $"���ʹ�ÿ�Ƭ��������{cardInfo.value}��Ĺ�����";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.AtkDown:
                    BattleEnermyInfo.Instance?.UpdateAtk(-cardInfo.value);
                    log = $"���ʹ�ÿ�Ƭ���͵з�{cardInfo.value}��Ĺ�����";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.DefUp:
                    BattlePlayerInfo.Instance?.UpdateDef(cardInfo.value);
                    log = $"���ʹ�ÿ�Ƭ��������{cardInfo.value}��ķ�����";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.DefDown:
                    BattleEnermyInfo.Instance?.UpdateDef(-cardInfo.value);
                    log = $"���ʹ�ÿ�Ƭ���͵з�{cardInfo.value}��ķ�����";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.Sleep:
                    log = $"���ʹ�ÿ�Ƭ�õз�˯������һ�غ�";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.Cover:
                    BattlePlayerInfo.Instance?.UpdateHp(cardInfo.value);
                    log = $"���ʹ�ÿ�Ƭ�ظ��ҷ�{cardInfo.value}���Ѫ��";
                    break;
                case CardType.None:
                    log = $"���ʹ�ÿ�Ƭ....���·���";
                    Debug.Log(log);
                    //text.text = log;
                    break;
            }
            //���ÿ����Լ�����ʧ����
            cardItem.Disappear();

            //�Ե���˫������Ѫ���жϣ��鿴��ǰ�Ƿ�����ʤ��/ʧ������
            CheckIsWin();
        });
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_CARD_EFFECT, action);
    }
    void CheckIsWin()
    {
        if (BattleEnermyInfo.Instance?.Character._curHP <= 0)
        {
            //ʤ��
            SwitchBattleType(BattleType.Winner);
            return;
        }
        if (BattlePlayerInfo.Instance?.Character._curHP <= 0)
        {
            //ʧ��
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
                        //ֱ������˺�
                        //�����˺�
                        float temAtk = (float)BattlePlayerInfo.Instance?.Character._curAtk;
                        BattleEnermyInfo.Instance?.UpdateHp(-(cardInfo.value + temAtk) * 1.2f);
                        string log = $"���ʹ�ÿ�Ƭ�Ե��������{cardInfo.value}����˺�";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.AtkUp:
                        BattlePlayerInfo.Instance?.UpdateAtk(cardInfo.value * 1.2f);
                        log = $"���ʹ�ÿ�Ƭ��������{cardInfo.value}��Ĺ�����";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.AtkDown:
                        BattleEnermyInfo.Instance?.UpdateAtk(-cardInfo.value * 1.2f);
                        log = $"���ʹ�ÿ�Ƭ���͵з�{cardInfo.value}��Ĺ�����";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.DefUp:
                        BattlePlayerInfo.Instance?.UpdateDef(cardInfo.value * 1.2f);
                        log = $"���ʹ�ÿ�Ƭ��������{cardInfo.value}��ķ�����";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.DefDown:
                        BattleEnermyInfo.Instance?.UpdateDef(-cardInfo.value);
                        log = $"���ʹ�ÿ�Ƭ���͵з�{cardInfo.value}��ķ�����";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.Sleep:
                        log = $"���ʹ�ÿ�Ƭ�õз�˯������һ�غ�";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                    case CardType.Cover:
                        BattlePlayerInfo.Instance?.UpdateHp(cardInfo.value * 1.2f);
                        log = $"���ʹ�ÿ�Ƭ�ظ��ҷ�{cardInfo.value}���Ѫ��";
                        break;
                    case CardType.None:
                        log = $"���ʹ�ÿ�Ƭ....���·���";
                        Debug.Log(log);
                        //text.text = log;
                        break;
                }
                //���ÿ����Լ�����ʧ����
                cardItem.Disappear();
                //�Ե���˫������Ѫ���жϣ��鿴��ǰ�Ƿ�����ʤ��/ʧ������
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
            Debug.LogError("�Ҳ�����Ӧid�Ŀ�Ƭ��Ϣ");
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