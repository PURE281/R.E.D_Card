using DG.Tweening;
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

/// <summary>
/// 这是用来管理抽卡的卡组的管理类
/// 实现功能，打乱卡牌顺序，随机按照顺序依次出现，然后依次转出正面，在特别的图片中需要有特定的处理
/// </summary>
public class CardGroupMgr : MonoSington<CardGroupMgr>
{

    /// <summary>
    /// 储存所有可使用的卡牌信息的对象
    /// </summary>
    private Dictionary<string, CardInfoBean> cardinfos = new Dictionary<string, CardInfoBean>();
    private List<CardInfoBean> rCardList = new List<CardInfoBean>();
    private List<CardInfoBean> srCardList = new List<CardInfoBean>();
    private List<CardInfoBean> ssrCardList = new List<CardInfoBean>();
    private List<CardInfoBean> urCardList = new List<CardInfoBean>();
    //private List<Sprite> sprite_cards_list = new List<Sprite>();

    private bool isActive = false;

    public Button _startCardsbtn;
    [SerializeField]
    /// <summary>
    /// 储存当前现场卡牌的对象
    /// </summary>
    private List<GameObject> _cardsInHand = new List<GameObject>();
    public List<GameObject> CardsInHand { get => _cardsInHand; }

    void Awake()
    {
        //初始化卡牌
        StartCoroutine(InitBattleCard());
    }

    public void GetCard()
    {
        for (int i = 0; i < _cardsInHand.Count; i++)
        {
            Destroy(_cardsInHand[i]);
        }
        StartCoroutine(CreateCardGo(10));
    }

    IEnumerator InitBattleCard()
    {
        //加载所有卡牌信息
        cardinfos = CsvManager.Instance?.ReadCardInfoCSVFile();

        // 步骤1: 将Dictionary的键值对添加到列表中  
        List<KeyValuePair<string, CardInfoBean>> cardList = cardinfos.ToList();
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
        // 步骤2: 随机打乱列表  
        ShuffleList(cardList);
        //RollCards();
        yield return null;
    }

    public IEnumerator CreateCardGo(int cardMax)
    {
        //执行完成之前不允许使用卡组
        //EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_INACTIVATE_CARDSINHAND);
        string assetsName = "Card";
        GameObject cardGo = Resources.Load<GameObject>($"Prefabs/{assetsName}");
        for (int i = 0; i < cardMax; i++)
        {

            //初始化go预制体
            GameObject gameObject1 = Instantiate(cardGo);
            yield return new WaitForSeconds(0.5f);
            gameObject1.transform.SetParent(GameObject.FindWithTag("MainCanvas").transform.Find("PC/CardPanel"));
            //gameObject1.transform.localScale = Vector3.one;
            //给预制体添加卡片信息
            int _temIndex = new MinMaxRandomInt(0, cardinfos.Count).GetRandomValue();
            //Transform _cardFront = gameObject1.transform.Find("CardFront");
            gameObject1.AddComponent<CardItem>();
            CardInfoBean cardInfoBean = CreateCardInfoBean(GetCardBeanByProbability());
            //_cardFront.GetComponent<CardItem>()._index = i;
            gameObject1.GetComponent<CardItem>().Init(cardInfoBean);
            gameObject1.GetComponent<CardItem>().StartFront();

            CardsInHand.Add(gameObject1);

        }
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
                Debug.Log("抽到ssr啦");
                return ssrCardList[tem];
            case CardProbability.UR:
                tem = new MinMaxRandomInt(0, urCardList.Count).GetRandomValue();
                Debug.Log("抽到ur啦");
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
