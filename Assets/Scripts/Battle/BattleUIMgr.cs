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
        //��ʼ��UI������
        GameObject temGO = Resources.Load<GameObject>("Prefabs/BattleCardDetailGO");
        _cardDetailGO = Instantiate(temGO);
        Vector3 _temCardDetailGO = _cardDetailGO.transform.localPosition;
        _cardDetailGO.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("PC/BattleCardDetailPanel"));
        _cardDetailGO.transform.localPosition = _temCardDetailGO;
        _cardDetailGO.name = "BattleCardDetailGO";
        _cardDetailGO.SetActive(false);
        //��ʼ��UI��ص�ί���¼�
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
    /// ������ÿһ�ε�����Ƴ���ʱ��Ҫִ�еķ���
    /// ���������еĿ������Ƿ������������ģ�������������������صķ���
    /// �統�������ſ�����ͬʱ���Խ������ǣ������ǵİ�ť����
    /// </summary>
    void RefreshCards(object card)
    {
        //��ÿ��ѡ�еĿ���ȡ��,Ȼ����ӵ�������,�Լ��Ͻ��д���
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
        //��ѡ��Ŀ��ƽ����ж�
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
        //������ѡ�еĿ�Ƭ�����׼����٣����µĿ�Ƭ���ǵ�������������ſ�Ƭ��
        _temSelectedAllCardList.Remove(_temSelectedAllCardList[0]);
        _temSelectedUpgradeCardList[0].Disappear();
        BattleSystemMgr.Instance?.RemoveCardInHand(_temSelectedUpgradeCardList[0].transform.parent.gameObject);
        _temSelectedUpgradeCardList.Remove(_temSelectedUpgradeCardList[0]);
        //���ݸ���id��battlesystem�ű��ж�ȡ����Ӧ�Ŀ�Ƭ��Ϣ
        CardInfoBean newCardBean = BattleSystemMgr.Instance?.LoadCardItemById(upgradeid);
        //����ԭ�еĿ�Ƭ��Ϣ
        tem2UpgradeCard.GetComponent<CardItem>().Init(newCardBean);
        tem2UpgradeCard.GetComponent<CardItem>().CloseUpdadteCard();
        this.RefreshCards(tem2UpgradeCard);
    }
    void ComboCards(object card)
    {
        Debug.Log("��Я");
    }

    void FusionCards(object card)
    {
        Debug.Log("�ں�");
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
