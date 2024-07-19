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
    List<CardItem> _temSelectedAllCardList = new List<CardItem>();
    List<CardItem> _temSelectedUpgradeCardList = new List<CardItem>();
    List<CardItem> _temSelectedComboCardList = new List<CardItem>();
    List<CardItem> _temSelectedFusionList = new List<CardItem>();
    List<CardItem> _temSelectedWinList = new List<CardItem>();
    /// <summary>
    /// 这里在每一次点击卡牌出列时需要执行的方法
    /// 用来检测出列的卡牌中是否有满足条件的，有满足条件的则开启相关的方法
    /// 如当满足两张卡牌相同时可以进行升星，则将升星的按钮点亮
    /// </summary>
    void RefreshCards(object card)
    {
        //将每次选中的卡获取到,然后添加到集合中,对集合进行处理
        GameObject curSelectedCard = (GameObject)card;
        CardItem cardItem = curSelectedCard.GetComponent<CardItem>();
        if (cardItem._isSelected)
        {
            if (!_temSelectedAllCardList.Contains(cardItem))
            {
                _temSelectedAllCardList.Add(cardItem);
            }
        }
        else
        {
            _temSelectedAllCardList.Remove(cardItem);
        }
        //对选择的卡牌进行判定
        var groupByIdCardList = _temSelectedAllCardList.GroupBy(x => x._cardInfo.id);
        int _sameCardNum = 0;
        foreach (var group in groupByIdCardList)
        {
            foreach (var item in group)
            {
                if (cardItem._cardInfo.id==item._cardInfo.id)
                {
                    if (!_temSelectedUpgradeCardList.Contains(item))
                    {
                        _temSelectedUpgradeCardList.Add((CardItem)item);
                    }
                }
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
            }
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
        //将最先选中的卡片进行献祭销毁，将新的卡片覆盖到点击升级的这张卡片上
        _temSelectedAllCardList.Remove(_temSelectedAllCardList[0]);
        _temSelectedUpgradeCardList[0].Disappear();
        BattleSystemMgr.Instance?.RemoveCardInHand(_temSelectedUpgradeCardList[0].transform.parent.gameObject);
        _temSelectedUpgradeCardList.Remove(_temSelectedUpgradeCardList[0]);
        //根据父级id在battlesystem脚本中读取到对应的卡片信息
        CardInfoBean newCardBean = BattleSystemMgr.Instance?.LoadCardItemById(upgradeid);
        //覆盖原有的卡片信息
        tem2UpgradeCard.GetComponent<CardItem>().Init(newCardBean);
        tem2UpgradeCard.GetComponent<CardItem>().CloseUpdadteCard();
        this.RefreshCards(tem2UpgradeCard);
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
