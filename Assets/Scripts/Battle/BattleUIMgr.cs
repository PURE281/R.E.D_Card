using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static EnumMgr;
using Sequence = DG.Tweening.Sequence;

public class BattleUIMgr : MonoSington<BattleUIMgr>
{
    private GameObject _cardDetailGO;
    private GameObject _cardMenuGO;
    private GameObject _showCardGO;
    private GameObject _settlementGO;

    private GameObject _mainCanvas;
    private void Awake()
    {
        _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        //初始化UI相关组件
        GameObject temGO = Resources.Load<GameObject>("Prefabs/BattleCardDetailGO");
        _cardDetailGO = Instantiate(temGO);
        Vector3 _temCardDetailGO = _cardDetailGO.transform.localPosition;
        _cardDetailGO.transform.SetParent(_mainCanvas.transform.Find("PC/BattleCardDetailPanel"));
        _cardDetailGO.transform.localPosition = _temCardDetailGO;
        _cardDetailGO.name = "BattleCardDetailGO";
        _cardDetailGO.SetActive(false);

        //初始化菜单栏
        _cardMenuGO = _mainCanvas.transform.Find("PC/BattleMenu/Panel").gameObject;
        _showCardGO = _mainCanvas.transform.Find("PC/CardEffect").gameObject;

        //初始化结算画面
        GameObject temSettlementGO = Resources.Load<GameObject>("Prefabs/SettlementGo");
        _settlementGO = Instantiate(temSettlementGO);
        _settlementGO.transform.SetParent(_mainCanvas.transform.Find("PC"));
        _settlementGO.transform.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        _settlementGO.transform.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        _settlementGO.SetActive(false);

        _cardMenuGO.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            BattleSystemMgr.Instance?.SwitchBattleType(BattleType.EnermyTurn);
        });
        _cardMenuGO.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            BattleSystemMgr.Instance?.ToMainScene();
        });
        //初始化卡片UI相关的委托事件
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_SHOW_CARD_DETAIL, ShowCardDetail);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL, CloseCardDetail);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_REFRESH_CARDS, RefreshCards);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_UPDATE_CARDS, UpdateCards);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_COMBO_CARDS, ComboCards);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_FUSION_CARDS, FusionCards);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_SHOW_MENU, ShowMenu);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_CLOSE_MENU, CloseMenu);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_RESET_CARDS, ResetCards);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_ACTIVATE_CARDSINHAND, ActivateCardsInHandGroup);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_INACTIVATE_CARDSINHAND, InActivateCardsInHandGroup);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_SHOW_CARD_EFFECT, ShowCardEffect);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_PLAYER_WIN, ShowPlayerWin);
        EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_PLAYER_LOSE, ShowPlayerLose);

        //初始化角色UI相关的委托事件
        //EventCenter.Instance?.listen(CustomEvent.BATTLE_UI_REFRESH_CHARACTERINFO, RefreshCharacterInfo);
        //初始化
    }

    #region 这些是关于更新卡片信息的方法
    List<BattleCardItem> _temSelectedAllCardList = new List<BattleCardItem>();
    List<BattleCardItem> _temSelectedUpgradeCardList = new List<BattleCardItem>();
    List<BattleCardItem> _temSelectedComboCardList = new List<BattleCardItem>();
    List<BattleCardItem> _temSelectedFusionList = new List<BattleCardItem>();
    List<BattleCardItem> _temSelectedWinList = new List<BattleCardItem>();
    /// <summary>
    /// 这里在每一次点击卡牌出列时需要执行的方法
    /// 用来检测出列的卡牌中是否有满足条件的，有满足条件的则开启相关的方法
    /// 如当满足两张卡牌相同时可以进行升星，则将升星的按钮点亮
    /// </summary>
    void RefreshCards(object card)
    {
        //将每次选中的卡获取到,然后添加到集合中,对集合进行处理
        GameObject curSelectedCard = (GameObject)card;
        BattleCardItem cardItem = null;

        cardItem = curSelectedCard.GetComponent<BattleCardItem>();
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
            _temSelectedUpgradeCardList.Remove(cardItem);
            _temSelectedComboCardList.Remove(cardItem);
            _temSelectedFusionList.Remove(cardItem);
        }
        if (cardItem._isDestroy)
        {
            //
            _temSelectedAllCardList.Remove(cardItem);
            _temSelectedUpgradeCardList.Remove(cardItem);
            _temSelectedComboCardList.Remove(cardItem);
            _temSelectedFusionList.Remove(cardItem);
        }
        //对选择的卡牌进行判定---
        #region 检查是否可以升星
        var groupByIdCardList = _temSelectedAllCardList.GroupBy(x => x._cardInfo.id);
        int _sameCardNum = 0;
        foreach (var group in groupByIdCardList)
        {
            foreach (var item in group)
            {

                if (!_temSelectedUpgradeCardList.Contains(cardItem) && cardItem._isSelected)
                {
                    _temSelectedUpgradeCardList.Add(cardItem);
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
        #endregion
        #region 检查是否可以连携
        var groupByComboIdCardList = _temSelectedAllCardList.GroupBy(x => x._cardInfo.combo_id);
        //_sameCardNum = 0;
        foreach (var group in groupByComboIdCardList)
        {
            foreach (var item in group)
            {
                if (!_temSelectedComboCardList.Contains(cardItem) && cardItem._isSelected)
                {
                    _temSelectedComboCardList.Add(cardItem);
                }
                if (_temSelectedComboCardList.Count > 1)
                {
                    if (_temSelectedComboCardList[0]._cardInfo.id != item._cardInfo.id
                        && _temSelectedComboCardList[0]._cardInfo.combo_id == item._cardInfo.combo_id)
                    {
                        Debug.Log("可进行连携攻击");
                        item.ShowComboCard();
                    }
                    else
                    {
                        item.CloseComboCard();
                    }
                }
                else
                {
                    item.CloseComboCard();
                }
            }
        }
        #endregion

        #region 检查是否可以融合
        var groupByFusionIdCardList = _temSelectedAllCardList.GroupBy(x => x._cardInfo.fusion_id);
        //int _sameFusionCardNum = 0;
        foreach (var group in groupByFusionIdCardList)
        {
            foreach (var item in group)
            {

                if (!_temSelectedFusionList.Contains(cardItem) && cardItem._isSelected)
                {
                    _temSelectedFusionList.Add(cardItem);
                }
                if (_temSelectedComboCardList[0]._cardInfo.id != item._cardInfo.id
                    && _temSelectedFusionList[0]._cardInfo.fusion_id == item._cardInfo.fusion_id)
                {
                    Debug.Log("相同，可融合");
                    item.ShowFusionCard();
                }
                else
                {
                    item.CloseFusionCard();
                }

            }
            //_sameFusionCardNum = 0;
        }
        #endregion
    }
    void ActivateCardsInHandGroup(object data = null)
    {
        _mainCanvas.transform.Find("PC/CardGroup/Panel").GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    void InActivateCardsInHandGroup(object data = null)
    {
        _mainCanvas.transform.Find("PC/CardGroup/Panel").GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
    /// <summary>
    /// 需要在每次执行对卡片的处理后都重置手牌状态
    /// </summary>
    void ResetCards(object data = null)
    {
        _temSelectedAllCardList.Clear();
        _temSelectedUpgradeCardList.Clear();
        _temSelectedComboCardList.Clear();
        _temSelectedFusionList.Clear();
        _temSelectedWinList.Clear();
        //循环手牌，将所有拍重置为默认状态
        List<GameObject> cardsInHand = BattleSystemMgr.Instance?.CardsInHand;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            BattleCardItem cardItem = cardsInHand[i].GetComponent<BattleCardItem>();
            cardItem._isSelected = false;
            cardItem._isDestroy = false;
            cardItem.CloseUpdadteCard();
            cardItem.CloseComboCard();
            cardItem.CloseFusionCard();
            cardItem.CloseMenu();
        }
    }
    void UpdateCards(object card)
    {
        //获取两个对象，销毁两个对象，合成一个新的对象
        Debug.Log("升级卡牌");
        //根据传入的对象id，查询缓存中的对象集合，将相同的集合并且靠前的索引进行销毁，并根据传入的对象的upgradeid进行新对象的生成
        GameObject tem2UpgradeCard = (GameObject)card;
        if (tem2UpgradeCard == null) { Debug.LogError("请检查传入的对象"); return; }
        string id = tem2UpgradeCard.GetComponent<BattleCardItem>()._cardInfo.id;
        string upgradeid = tem2UpgradeCard.GetComponent<BattleCardItem>()._cardInfo.upgrade_id;
        //找到在卡组中跟这个升级的卡片相同id的第一个
        List<BattleCardItem> temList = _temSelectedUpgradeCardList.Where(x => x._cardInfo.id == id).ToList();
        //将最先选中的卡片进行献祭销毁，将新的卡片覆盖到点击升级的这张卡片上
        _temSelectedAllCardList.Remove(temList[0]);
        temList[0].Disappear();
        //根据父级id在battlesystem脚本中读取到对应的卡片信息
        CardInfoBean newCardBean = BattleSystemMgr.Instance?.LoadCardItemById(upgradeid);
        //覆盖原有的卡片信息
        tem2UpgradeCard.GetComponent<BattleCardItem>().Init(newCardBean);
        tem2UpgradeCard.GetComponent<BattleCardItem>()._isSelected = false;
        tem2UpgradeCard.GetComponent<BattleCardItem>().CloseMenu();
        //_temSelectedUpgradeCardList.Clear();
        this.ResetCards();
    }
    void ComboCards(object card)
    {
        Debug.Log("连携");
        //根据传入的对象id，查询缓存中的对象集合，将相同的集合并且靠前的索引进行销毁，并根据传入的对象的upgradeid进行新对象的生成
        GameObject tem2ComboCard = (GameObject)card;
        if (tem2ComboCard == null) { Debug.LogError("请检查传入的对象"); return; }
        string id = tem2ComboCard.GetComponent<BattleCardItem>()._cardInfo.id;
        string combo_id = tem2ComboCard.GetComponent<BattleCardItem>()._cardInfo.combo_id;
        //找到在卡组中跟这个升级的卡片相同id的第一个
        List<BattleCardItem> temList = _temSelectedComboCardList.Where(x => x._cardInfo.combo_id == combo_id).ToList();
        //将最先选中的卡片进行献祭销毁，将新的卡片覆盖到点击升级的这张卡片上
        _temSelectedAllCardList.Remove(temList[0]);
        temList[0].Disappear();
        tem2ComboCard.GetComponent<BattleCardItem>().Disappear();
        //将当前的卡和索引为0的卡打出去
        BattleSystemMgr.Instance?.HandleComboCards(new List<GameObject>() { temList[0].gameObject, tem2ComboCard.GetComponent<BattleCardItem>().gameObject });
        this.ResetCards();
    }

    void FusionCards(object card)
    {
        Debug.Log("融合");
        //根据传入的对象id，查询缓存中的对象集合，将相同的集合并且靠前的索引进行销毁，并根据传入的对象的upgradeid进行新对象的生成
        GameObject tem2FusionCard = (GameObject)card;
        if (tem2FusionCard == null) { Debug.LogError("请检查传入的对象"); return; }
        string id = tem2FusionCard.GetComponent<BattleCardItem>()._cardInfo.id;
        string fusion_id = tem2FusionCard.GetComponent<BattleCardItem>()._cardInfo.fusion_id;
        //找到在卡组中跟这个升级的卡片相同id的第一个
        List<BattleCardItem> temList = _temSelectedFusionList.Where(x => x._cardInfo.fusion_id == fusion_id).ToList();
        //将最先选中的卡片进行献祭销毁，将新的卡片覆盖到点击升级的这张卡片上
        _temSelectedAllCardList.Remove(temList[0]);
        temList[0].Disappear();
        //根据父级id在battlesystem脚本中读取到对应的卡片信息
        CardInfoBean newCardBean = BattleSystemMgr.Instance?.LoadCardItemById(fusion_id);
        //覆盖原有的卡片信息
        tem2FusionCard.GetComponent<BattleCardItem>().Init(newCardBean);
        tem2FusionCard.GetComponent<BattleCardItem>()._isSelected = false;
        tem2FusionCard.GetComponent<BattleCardItem>().CloseMenu();
        this.ResetCards();
    }
    void ShowCardDetail(object card)
    {
        GameObject tem = (GameObject)card;

        _cardDetailGO.SetActive(true);
        _cardDetailGO.transform.DOScale(1, 0.5f);
        _cardDetailGO.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        _cardDetailGO.GetComponentInChildren<Text>().text = tem.GetComponent<BattleCardItem>()._cardInfo.description;
        _cardDetailGO.transform.Find("picframe/pic").GetComponent<Image>().sprite = tem.GetComponent<BattleCardItem>()._cardPic;
        PictureMgr.Instance?.AdjustImageToAspectFit(_cardDetailGO.transform.Find("picframe/pic").GetComponent<Image>(), _cardDetailGO.transform.Find("picframe").GetComponent<RectTransform>());
    }

    void CloseCardDetail(object card)
    {
        if (_temSelectedAllCardList.Count != 0)
        {
            return;
        }
        _cardDetailGO.transform.DOScale(0, 0.5f);
        _cardDetailGO.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        _cardDetailGO.SetActive(false);
    }

    public void ShowCardEffect(object data)
    {
        _showCardGO.transform.DOLocalMoveX(-600, 0);
        //需要有一点简单的效果
        //卡片中的图片从左边渐变放大，像右边移动，然后慢慢消失
        Sequence sequence = DOTween.Sequence();
        sequence
        .AppendCallback(() =>
        {
            _showCardGO.transform.DOLocalMoveX(0, 0.3f);
            _showCardGO.transform.DOScale(1.5f, 0.3f);
            _showCardGO.transform.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
        })
        .AppendInterval(1.5f)
        .AppendCallback(() =>
        {
            _showCardGO.transform.DOLocalMoveX(600, 0.5f);
            _showCardGO.transform.DOScale(0.6f, 0.5f);
            _showCardGO.transform.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        }).AppendCallback(() =>
        {
            Action action = (Action)data;
            if (action != null)
            {
                action();
            }
        });
        //同时文字提醒，同时对应的效果发动
        ToastManager.Instance?.CreatToast("发动卡片效果：xxxxxxx");
    }

    private void ShowMenu(object data)
    {
        this._cardMenuGO.SetActive(true);
        if (BattleSystemMgr.Instance?.BattleType == BattleType.PlayerTurn)
        {
            this._cardMenuGO.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }
        else
        {
            this._cardMenuGO.transform.GetChild(1).GetComponent<Button>().interactable = false;
        }
    }

    private void CloseMenu(object data)
    {
        this._cardMenuGO.SetActive(false);
    }
    #endregion

    void ShowPlayerWin(object data)
    {
        this._settlementGO.GetComponent<CanvasGroup>().DOFade(0,0);
        this._settlementGO.GetComponent<CanvasGroup>().transform.DOScale(1,0);
        this._settlementGO.SetActive(true);
        this._settlementGO.transform.GetChild(0).gameObject.SetActive(true);
        this._settlementGO.transform.GetChild(1).gameObject.SetActive(false);
        this._settlementGO.GetComponent<CanvasGroup>().DOFade(1,1f);
        this._settlementGO.GetComponent<CanvasGroup>().DOFade(1,1f);
        this._settlementGO.GetComponent<CanvasGroup>().transform.DOScale(1.2f, 1.5f);
        BattleSystemMgr.Instance?.SwitchBattleType(BattleType.End);
    }
    void ShowPlayerLose(object data)
    {
        this._settlementGO.GetComponent<CanvasGroup>().DOFade(0, 0);
        this._settlementGO.GetComponent<CanvasGroup>().transform.DOScale(1, 0);
        this._settlementGO.SetActive(true);
        this._settlementGO.transform.GetChild(1).gameObject.SetActive(true);
        this._settlementGO.transform.GetChild(0).gameObject.SetActive(false);
        this._settlementGO.GetComponent<CanvasGroup>().DOFade(1, 1f);
        this._settlementGO.GetComponent<CanvasGroup>().DOFade(1, 1f);
        this._settlementGO.GetComponent<CanvasGroup>().transform.DOScale(1.2f, 1.5f);
        BattleSystemMgr.Instance?.SwitchBattleType(BattleType.End);
    }

    private void OnDestroy()
    {
        EventCenter.Instance?.removeAll(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL);
        EventCenter.Instance?.removeAll(CustomEvent.BATTLE_UI_SHOW_CARD_DETAIL);
    }
    #region 这些是关于更新角色信息的方法
    void RefreshCharacterInfo(object characterObj)
    {
        if (characterObj is BattlePlayerBean)
        {
            //更新主角信息

        }
        else
        {
            //更新敌方信息
        }
    }


    #endregion
}
