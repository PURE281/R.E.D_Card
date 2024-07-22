using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static EnumMgr;

public class BattleUIMgr : MonoSingleton<BattleUIMgr>
{
    private GameObject _cardDetailGO;
    private GameObject _cardMenuGO;

    private GameObject _mainCanvas;
    private void Awake()
    {
        _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        //��ʼ��UI������
        GameObject temGO = Resources.Load<GameObject>("Prefabs/BattleCardDetailGO");
        _cardDetailGO = Instantiate(temGO);
        Vector3 _temCardDetailGO = _cardDetailGO.transform.localPosition;
        _cardDetailGO.transform.SetParent(_mainCanvas.transform.Find("PC/BattleCardDetailPanel"));
        _cardDetailGO.transform.localPosition = _temCardDetailGO;
        _cardDetailGO.name = "BattleCardDetailGO";
        _cardDetailGO.SetActive(false);

        //��ʼ���˵���
        _cardMenuGO = _mainCanvas.transform.Find("PC/BattleMenu/Panel").gameObject;
        _cardMenuGO.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            BattleSystemMgr.Instance?.SwitchBattleType(BattleType.EnermyTurn);
        });
        //_cardMenuGO.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    this.ResetCards();
        //    BattleSystemMgr.Instance?.GetCard();
        //});
        _cardMenuGO.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
        {
            BattleSystemMgr.Instance?.ToMainScene();
        });
        _cardMenuGO.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
        {
            //ģ��з���������
            BattleSystemMgr.Instance?.SwitchBattleType(BattleType.PlayerTurn);
        });
        //��ʼ��UI��ص�ί���¼�
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
    }
    List<CardItem> _temSelectedAllCardList = new List<CardItem>();
    List<CardItem> _temSelectedUpgradeCardList = new List<CardItem>();
    List<CardItem> _temSelectedComboCardList = new List<CardItem>();
    List<CardItem> _temSelectedFusionList = new List<CardItem>();
    List<CardItem> _temSelectedWinList = new List<CardItem>();
    /// <summary>
    /// ������ÿһ�ε�����Ƴ���ʱ��Ҫִ�еķ���
    /// ���������еĿ������Ƿ������������ģ�������������������صķ���
    /// �統�������ſ�����ͬʱ���Խ������ǣ������ǵİ�ť����
    /// </summary>
    void RefreshCards(object card)
    {
        //��ÿ��ѡ�еĿ���ȡ��,Ȼ����ӵ�������,�Լ��Ͻ��д���
        GameObject curSelectedCard = (GameObject)card;
        CardItem cardItem = null;

        cardItem = curSelectedCard.GetComponent<CardItem>();
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
        //��ѡ��Ŀ��ƽ����ж�---
        #region ����Ƿ��������
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
                    Debug.Log("��ͬ��������");
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
        #region ����Ƿ������Я
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
                if (_temSelectedComboCardList.Count>1)
                {
                    if (_temSelectedComboCardList[0]._cardInfo.id != item._cardInfo.id 
                        && _temSelectedComboCardList[0]._cardInfo.combo_id == item._cardInfo.combo_id)
                    {
                        Debug.Log("�ɽ�����Я����");
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

        #region ����Ƿ�����ں�
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
                    Debug.Log("��ͬ�����ں�");
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
    /// ��Ҫ��ÿ��ִ�жԿ�Ƭ�Ĵ������������״̬
    /// </summary>
    void ResetCards(object data = null) {
        _temSelectedAllCardList.Clear();
        _temSelectedUpgradeCardList.Clear();
        _temSelectedComboCardList.Clear();
        _temSelectedFusionList.Clear();
        _temSelectedWinList.Clear();
        //ѭ�����ƣ�������������ΪĬ��״̬
        List<GameObject> cardsInHand = BattleSystemMgr.Instance?.CardsInHand;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            CardItem cardItem = cardsInHand[i].GetComponentInChildren<CardItem>();
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
        //��ȡ�������������������󣬺ϳ�һ���µĶ���
        Debug.Log("��������");
        //���ݴ���Ķ���id����ѯ�����еĶ��󼯺ϣ�����ͬ�ļ��ϲ��ҿ�ǰ�������������٣������ݴ���Ķ����upgradeid�����¶��������
        GameObject tem2UpgradeCard = (GameObject)card;
        if (tem2UpgradeCard == null) { Debug.LogError("���鴫��Ķ���"); return; }
        string id = tem2UpgradeCard.GetComponent<CardItem>()._cardInfo.id;
        string upgradeid = tem2UpgradeCard.GetComponent<CardItem>()._cardInfo.upgrade_id;
        //�ҵ��ڿ����и���������Ŀ�Ƭ��ͬid�ĵ�һ��
        List<CardItem> temList = _temSelectedUpgradeCardList.Where(x=>x._cardInfo.id==id).ToList();
        //������ѡ�еĿ�Ƭ�����׼����٣����µĿ�Ƭ���ǵ�������������ſ�Ƭ��
        _temSelectedAllCardList.Remove(temList[0]);
        temList[0].Disappear();
        //���ݸ���id��battlesystem�ű��ж�ȡ����Ӧ�Ŀ�Ƭ��Ϣ
        CardInfoBean newCardBean = BattleSystemMgr.Instance?.LoadCardItemById(upgradeid);
        //����ԭ�еĿ�Ƭ��Ϣ
        tem2UpgradeCard.GetComponent<CardItem>().Init(newCardBean);
        tem2UpgradeCard.GetComponent<CardItem>()._isSelected = false;
        tem2UpgradeCard.GetComponent<CardItem>().CloseMenu();
        //_temSelectedUpgradeCardList.Clear();
        this.ResetCards();
    }
    void ComboCards(object card)
    {
        Debug.Log("��Я");
        //���ݴ���Ķ���id����ѯ�����еĶ��󼯺ϣ�����ͬ�ļ��ϲ��ҿ�ǰ�������������٣������ݴ���Ķ����upgradeid�����¶��������
        GameObject tem2ComboCard = (GameObject)card;
        if (tem2ComboCard == null) { Debug.LogError("���鴫��Ķ���"); return; }
        string id = tem2ComboCard.GetComponent<CardItem>()._cardInfo.id;
        string combo_id = tem2ComboCard.GetComponent<CardItem>()._cardInfo.combo_id;
        //�ҵ��ڿ����и���������Ŀ�Ƭ��ͬid�ĵ�һ��
        List<CardItem> temList = _temSelectedComboCardList.Where(x => x._cardInfo.combo_id == combo_id).ToList();
        //������ѡ�еĿ�Ƭ�����׼����٣����µĿ�Ƭ���ǵ�������������ſ�Ƭ��
        _temSelectedAllCardList.Remove(temList[0]);
        temList[0].Disappear();
        tem2ComboCard.GetComponent<CardItem>().Disappear();
        //����ǰ�Ŀ�������Ϊ0�Ŀ����ȥ
        BattleSystemMgr.Instance?.HandleComboCards(new List<GameObject>() { temList[0]._cardGo, tem2ComboCard.GetComponent<CardItem>()._cardGo });
        this.ResetCards();
    }

    void FusionCards(object card)
    {
        Debug.Log("�ں�");
        //���ݴ���Ķ���id����ѯ�����еĶ��󼯺ϣ�����ͬ�ļ��ϲ��ҿ�ǰ�������������٣������ݴ���Ķ����upgradeid�����¶��������
        GameObject tem2FusionCard = (GameObject)card;
        if (tem2FusionCard == null) { Debug.LogError("���鴫��Ķ���"); return; }
        string id = tem2FusionCard.GetComponent<CardItem>()._cardInfo.id;
        string fusion_id = tem2FusionCard.GetComponent<CardItem>()._cardInfo.fusion_id;
        //�ҵ��ڿ����и���������Ŀ�Ƭ��ͬid�ĵ�һ��
        List<CardItem> temList = _temSelectedFusionList.Where(x => x._cardInfo.fusion_id == fusion_id).ToList();
        //������ѡ�еĿ�Ƭ�����׼����٣����µĿ�Ƭ���ǵ�������������ſ�Ƭ��
        _temSelectedAllCardList.Remove(temList[0]);
        temList[0].Disappear();
        //���ݸ���id��battlesystem�ű��ж�ȡ����Ӧ�Ŀ�Ƭ��Ϣ
        CardInfoBean newCardBean = BattleSystemMgr.Instance?.LoadCardItemById(fusion_id);
        //����ԭ�еĿ�Ƭ��Ϣ
        tem2FusionCard.GetComponent<CardItem>().Init(newCardBean);
        tem2FusionCard.GetComponent<CardItem>()._isSelected = false;
        tem2FusionCard.GetComponent<CardItem>().CloseMenu();
        this.ResetCards();
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
        if (_temSelectedAllCardList.Count!=0)
        {
            return;
        }
        _cardDetailGO.transform.DOScale(0, 0.5f);
        _cardDetailGO.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        _cardDetailGO.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.Instance?.removeAll(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL);
        EventCenter.Instance?.removeAll(CustomEvent.BATTLE_UI_SHOW_CARD_DETAIL);
    }

    private void ShowMenu(object data)
    {
        //this._cardMenuGO.SetActive(true);
        //if (BattleSystemMgr.Instance?.BattleType == BattleType.PlayerTurn)
        //{
        //    this._cardMenuGO.transform.GetChild(1).GetComponent<Button>().interactable = true;
        //}
        //else
        //{
        //    this._cardMenuGO.transform.GetChild(1).GetComponent<Button>().interactable = false;
        //}
    }

    private void CloseMenu(object data)
    {
        //this._cardMenuGO.SetActive(false);
    }
}
