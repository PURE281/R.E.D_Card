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
/// �غ��ƶ�սϵͳ�Ĺ���ű�
/// </summary>
public class BattleSystemMgr : MonoSingleton<BattleSystemMgr>
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
                //��ʼ������
                StartCoroutine(InitBattleCard());
                //��ʼ��˫����Ϣ
                InitPlayerInfo();
                break;
            case BattleType.PlayerTurn:
                //��ʾ�˵���
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_MENU);
                //�Զ�����
                GetCard();
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);
                //����Ѫ���ж�
                break;
            case BattleType.EnermyTurn:
                //���ж������Ƿ����5�ţ��ǵĻ�����Ҫ���п��ƵĴ����������
                if (_cardsInHand.Count > 5)
                {
                    ToastManager.Instance?.CreatToast("�����������ܳ���5�ţ�");
                    return;
                }
                //�رղ˵���
                EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_CLOSE_MENU);
                //����Ѫ���ж�
                Debug.Log("�з��غ�");
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
        //�ӱ��ض�ȡ����

        Dictionary<int, CharacterInfo> characterDicts = CsvManager.Instance?.ReadCharacterInfoCSVFile();
        _playerInfo = (PlayerInfo)characterDicts[0];
        _enermyInfo = (EnermyInfo)characterDicts[1];
    }
    public IEnumerator CreateCardGo(int cardMax)
    {
        //ִ�����֮ǰ������ʹ�ÿ���
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_INACTIVATE_CARDSINHAND);
        string assetsName = "BattleCard";
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
            gameObject1.GetComponent<CardTurnOver>().StartFront();
            Transform _cardFront = gameObject1.transform.Find("CardFront");
            _cardFront.AddComponent<CardItem>();
            CardInfoBean cardInfoBean = CreateCardInfoBean(cardinfos.ElementAt(_temIndex).Value);
            //_cardFront.GetComponent<CardItem>()._index = i;
            _cardFront.GetComponent<CardItem>().Init(cardInfoBean);

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
        CardItem cardItem = cardGo.GetComponentInChildren<CardItem>();
        CardInfoBean cardInfo = cardItem._cardInfo;
        switch (cardInfo.type)
        {
            case CardType.Atk:
                //ֱ������˺�
                //�����˺�
                float temAtk = (float)_playerInfo._curAtk;
                _enermyInfo._curHP -= temAtk;
                string log = $"���ʹ�ÿ�Ƭ�Ե��������{cardInfo.value}����˺�";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.AtkUp:
                _playerInfo._curAtk += cardInfo.value;
                log = $"���ʹ�ÿ�Ƭ��������{cardInfo.value}��Ĺ�����";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.AtkDown:
                _enermyInfo._curAtk -= cardInfo.value;
                log = $"���ʹ�ÿ�Ƭ���͵з�{cardInfo.value}��Ĺ�����";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.DefUp:
                _playerInfo._curDef += cardInfo.value;
                log = $"���ʹ�ÿ�Ƭ��������{cardInfo.value}��ķ�����";
                Debug.Log(log);
                //text.text = log;
                break;
            case CardType.DefDown:
                _enermyInfo._curDef -= cardInfo.value;
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
                _playerInfo._curHP += cardInfo.value;
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
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);

        //�Ե���˫������Ѫ���жϣ��鿴��ǰ�Ƿ�����ʤ��/ʧ������
        CheckIsWin();
    }
    void CheckIsWin()
    {
        if (_enermyInfo._curHP<=0)
        {
            //ʤ��
            SwitchBattleType(BattleType.Winner);
            return;
        }
        if (_playerInfo._curHP<=0)
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
            CardItem cardItem = cardGos[i].GetComponentInChildren<CardItem>();
            CardInfoBean cardInfo = cardItem._cardInfo;
            switch (cardInfo.type)
            {
                case CardType.Atk:
                    //ֱ������˺�
                    //�����˺�
                    float temAtk = _playerInfo._curAtk * 1.2f;
                    _enermyInfo._curHP -= temAtk;
                    string log = $"���ʹ�ÿ�Ƭ�Ե��������{temAtk}����˺�";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.AtkUp:
                    _playerInfo._curAtk += cardInfo.value * 1.2f;
                    log = $"���ʹ�ÿ�Ƭ��������{cardInfo.value * 1.2f}��Ĺ�����";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.AtkDown:
                    _enermyInfo._curAtk -= cardInfo.value * 1.2f;
                    log = $"���ʹ�ÿ�Ƭ���͵з�{cardInfo.value * 1.2f}��Ĺ�����";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.DefUp:
                    _playerInfo._curDef += cardInfo.value * 1.2f;
                    log = $"���ʹ�ÿ�Ƭ��������{cardInfo.value * 1.2f}��ķ�����";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.DefDown:
                    _enermyInfo._curDef -= cardInfo.value * 1.2f;
                    log = $"���ʹ�ÿ�Ƭ���͵з�{cardInfo.value * 1.2f}��ķ�����";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.Sleep:
                    log = $"���ʹ�ÿ�Ƭ�õз�˯������һ�غ�";
                    Debug.Log(log);
                    //text.text = log;
                    break;
                case CardType.Cover:
                    _playerInfo._curHP += cardInfo.value * 1.2f;
                    log = $"���ʹ�ÿ�Ƭ�ظ��ҷ�{cardInfo.value * 1.2f}���Ѫ��";
                    break;
                case CardType.None:
                    log = $"���ʹ�ÿ�Ƭ....���·���";
                    Debug.Log(log);
                    //text.text = log;
                    break;
            }
            //���ÿ����Լ�����ʧ����
            cardItem.Disappear();
            EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);
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