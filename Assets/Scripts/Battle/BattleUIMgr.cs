using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static EnumMgr;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;

public class BattleUIMgr : MonoSingleton<BattleUIMgr>
{
    private GameObject _cardDetailGO;
    private void Awake()
    {
        //初始化UI相关组件
        GameObject temGO = Resources.Load<GameObject>("Prefabs/BattleCardDetailGO");
        _cardDetailGO = Instantiate(temGO);
        Vector3 _temCardDetailGO = _cardDetailGO.transform.localPosition;
        _cardDetailGO.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("PC/BattleCardDetailPanel"));
        _cardDetailGO.transform.localPosition = _temCardDetailGO;
        _cardDetailGO.name = "BattleCardDetailGO";
        _cardDetailGO.SetActive(false);
        //初始化UI相关的委托事件
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_SHOW_CARD_DETAIL, ShowCardDetail);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL, CloseCardDetail);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_REFRESH_CARDS, RefreshCards);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_UPDATE_CARDS, UpdateCards);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_COMBO_CARDS, ComboCards);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_FUSION_CARDS, FusionCards);
    }
    Dictionary<string, List<CardItem>> _selectedCardDicts = new Dictionary<string, List<CardItem>>();
    List<CardItem> _temSelectedCardList = null;
    /// <summary>
    /// 这里在每一次点击卡牌出列时需要执行的方法
    /// 用来检测出列的卡牌中是否有满足条件的，有满足条件的则开启相关的方法
    /// 如当满足两张卡牌相同时可以进行升星，则将升星的按钮点亮
    /// </summary>
    void RefreshCards(object card)
    {
        List<GameObject> cardsInHand = BattleSystemMgr.Instance?.CardsInHand;
        List<CardItem> cardsSelected = new List<CardItem>();
        _selectedCardDicts.Clear();
        CardItem tem = null;
        foreach (var item in cardsInHand)
        {
            tem = item.GetComponentInChildren<CardItem>();
            if (tem._isSelected)
            {
                cardsSelected.Add(item.GetComponentInChildren<CardItem>());
            }
        }
        //对选择的卡牌进行判定
        var duplicates = cardsSelected.GroupBy(x => x._cardInfo.id);
        int _sameCardNum = 0;
        string _temCardId = "";
        foreach (var group in duplicates)
        {
            _temSelectedCardList = new List<CardItem>();
            foreach (var item in group)
            {
                _temCardId = item._cardInfo.id;
                if (_sameCardNum >= 1)
                {
                    Debug.Log("相同，可升星");
                    item.ShowUpdadteCard();
                }
                else
                {
                    item.CloseUpdadteCard();
                }
                _sameCardNum++;

                _temSelectedCardList.Add(item);
            }
            _selectedCardDicts.Add(_temCardId, _temSelectedCardList);
            _sameCardNum = 0;
        }
    }
    void UpdateCards(object card)
    {
        //获取两个对象，销毁两个对象，合成一个新的对象
        Debug.Log("升级卡牌");
        //根据传入的对象id，查询缓存中的对象集合，将相同的集合并且靠前的索引进行销毁，并根据传入的对象的upgradeid进行新对象的生成
        GameObject tem2UpgradeCard = (GameObject)card;
        if (tem2UpgradeCard == null) { Debug.LogError("请检查传入的对象"); return; }
        string id = tem2UpgradeCard.GetComponent<CardItem>()._cardInfo.id;
        string upgradeid = tem2UpgradeCard.GetComponent<CardItem>()._cardInfo.upgrade_id;
        if (_selectedCardDicts.ContainsKey(id))
        {
            List<CardItem> list = _selectedCardDicts[id];
            list[0].Disappear();
            //Destroy(tem2UpgradeCard.transform.parent);
            //在当前选中的卡片上覆盖新的信息
            CardInfoBean newCardBean = BattleSystemMgr.Instance?.LoadCardItemById(upgradeid);
            tem2UpgradeCard.GetComponent<CardItem>().Init(newCardBean);

        }
    }
    void ComboCards(object card)
    {
        Debug.Log("连携");
    }

    void FusionCards(object card)
    {
        Debug.Log("融合");
    }
    void ShowCardDetail(object card)
    {
        GameObject tem = (GameObject)card;

        _cardDetailGO.SetActive(true);
        _cardDetailGO.transform.DOScale(1, 0.5f);
        _cardDetailGO.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        _cardDetailGO.GetComponentInChildren<Text>().text = tem.GetComponent<CardItem>()._cardInfo.description;
        _cardDetailGO.transform.Find("picframe/pic").GetComponent<Image>().sprite = tem.GetComponent<CardItem>()._cardPic;
        PictureMgr.Instance?.AdjustImageToAspectFit(_cardDetailGO.transform.Find("picframe/pic").GetComponent<Image>(), _cardDetailGO.transform.Find("picframe").GetComponent<RectTransform>());
    }

    void CloseCardDetail(object card)
    {
        _cardDetailGO.transform.DOScale(0, 0.5f);
        _cardDetailGO.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        _cardDetailGO.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.Instance?.removeAll(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL);
        EventCenter.Instance?.removeAll(CustomEvent.BATTLE_UI_SHOW_CARD_DETAIL);
    }
}
