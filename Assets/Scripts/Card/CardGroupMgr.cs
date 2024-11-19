using DG.Tweening;
using Newtonsoft.Json.Linq;
using RandomElementsSystem.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using zFramework.Extension;

/// <summary>
/// 
/// </summary>
public class CardGroupMgr : MonoSington<CardGroupMgr>
{

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<int, CardInfoBean> _allCardsDicts = new Dictionary<int, CardInfoBean>();
    private List<CardInfoBean> rCardList = new List<CardInfoBean>();
    private List<CardInfoBean> srCardList = new List<CardInfoBean>();
    private List<CardInfoBean> ssrCardList = new List<CardInfoBean>();
    private List<CardInfoBean> urCardList = new List<CardInfoBean>();
    private List<CardInfoBean> _temDrawCardList = new List<CardInfoBean>();
    //private List<Sprite> sprite_cards_list = new List<Sprite>();

    private bool isActive = false;

    public Button _startCardsbtn;

    private IEnumerator _getCardIE;
    [SerializeField]
    /// <summary>
    /// 
    /// </summary>
    private List<GameObject> _cardsInHand = new List<GameObject>();
    public List<GameObject> CardsInHand { get => _cardsInHand; }

    void Awake()
    {
    }
    public void Init()
    {
        _startCardsbtn.interactable = false;
        //��ʼ������
        StartCoroutine(InitCard());
    }
    public void GetCard()
    {
        this._startCardsbtn.interactable = false;
        for (int i = 0; i < _cardsInHand.Count; i++)
        {
            Destroy(_cardsInHand[i]);
        }
        _temDrawCardList.Clear();
        _getCardIE = CreateCardGo(10);
        StartCoroutine(_getCardIE);
    }

    IEnumerator InitCard()
    {
        //�������п�����Ϣ
        List<CardInfoBean> allCardInfoBeans = CsvUtility.Read<CardInfoBean>((GlobalConfig.Instance?.GetPath() + "/StreamingAssets") + "/" + "CardData.csv");

        foreach (var cardInfoBean in allCardInfoBeans)
        {
            _allCardsDicts.Add(cardInfoBean.id, cardInfoBean);
        }
        List<KeyValuePair<int, CardInfoBean>> cardList = _allCardsDicts.ToList();
        var groups = cardList.GroupBy(x => x.Value.probability);
        foreach (var group in groups)
        {
            foreach (var card in group)
            {
                switch (card.Value.probability)
                {
                    case "r":
                        rCardList.Add(card.Value);
                        break;
                    case "sr":
                        srCardList.Add(card.Value);
                        break;
                    case "ssr":
                        ssrCardList.Add(card.Value);
                        break;
                    case "ur":
                        urCardList.Add(card.Value);
                        break;
                }
            }
        }
        // ����2: ��������б�  
        //ShuffleList(cardList);
        //RollCards();
        yield return null;

        _startCardsbtn.interactable = true;
    }

    public IEnumerator CreateCardGo(int cardMax)
    {
        //EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_INACTIVATE_CARDSINHAND);
        string assetsName = "Card";
        GameObject cardGo = Resources.Load<GameObject>($"Prefabs/{assetsName}");
        for (int i = 0; i < cardMax; i++)
        {
            GameObject gameObject1 = Instantiate(cardGo);
            yield return new WaitForSeconds(0.5f);
            gameObject1.transform.SetParent(GameObject.FindWithTag("MainCanvas").transform.Find("PC/CardPanel"));
            //gameObject1.transform.localScale = Vector3.one;
            int _temIndex = new MinMaxRandomInt(0, _allCardsDicts.Count).GetRandomValue();
            //Transform _cardFront = gameObject1.transform.Find("CardFront");
            gameObject1.AddComponent<CardItem>();
            CardInfoBean cardInfoBean = CreateCardInfoBean(GetCardBeanByProbability());
            _temDrawCardList.Add(cardInfoBean);
            //_cardFront.GetComponent<CardItem>()._index = i;
            gameObject1.GetComponent<CardItem>().Init(cardInfoBean);
            gameObject1.GetComponent<CardItem>().StartFront();

            CardsInHand.Add(gameObject1);

        }
        //这里需要记录保存到数据库中

        this._startCardsbtn.interactable = true;
    }

    float commonCardChance = 0.8f;//r
    float rareCardChance = 0.13f;//sr
    float epicCardChance = 0.06f;//ssr
    float legendaryCardChance = 0.01f;//ur


    CardInfoBean GetCardBeanByProbability()
    {
        CardProbability cardProbability = GetCardProbability();
        int tem = 0;
        switch (cardProbability)
        {
            case CardProbability.R:
                tem = new MinMaxRandomInt(0, rCardList.Count).GetRandomValue();
                return rCardList[tem];
            case CardProbability.SR:
                tem = new MinMaxRandomInt(0, srCardList.Count).GetRandomValue();
                return srCardList[tem];
            case CardProbability.SSR:
                tem = new MinMaxRandomInt(0, ssrCardList.Count).GetRandomValue();
                Debug.Log("ssr");
                return ssrCardList[tem];
            case CardProbability.UR:
                tem = new MinMaxRandomInt(0, urCardList.Count).GetRandomValue();
                Debug.Log("ur");
                return urCardList[tem];
        }
        return null;
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
    public enum CardProbability
    {
        R, SR, SSR, UR
    }

    public CardProbability GetCardProbability()
    {
        float cardType = new MinMaxRandomFloat(0f, 1f).GetRandomValue();
        //Debug.Log(cardType);
        if (cardType < commonCardChance)
        {
            return CardProbability.R;
        }
        else if (cardType < commonCardChance + rareCardChance)
        {
            return CardProbability.SR;
        }
        else if (cardType < commonCardChance + rareCardChance + epicCardChance)
        {
            return CardProbability.SSR;
        }
        else
        {
            return CardProbability.UR;
        }
    }
}
