using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static EnumMgr;
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
    }

    /// <summary>
    /// ������ÿһ�ε�����Ƴ���ʱ��Ҫִ�еķ���
    /// ���������еĿ������Ƿ������������ģ�������������������صķ���
    /// �統�������ſ�����ͬʱ���Խ������ǣ������ǵİ�ť����
    /// </summary>
    void RefreshCards(object card)
    {
        List<GameObject> cardsInHand = BattleSystemMgr.Instance?.CardsInHand;
        List<CardItem> cardsSelected = new List<CardItem>();

        CardItem tem = null;
        foreach (var item in cardsInHand)
        {
            tem = item.GetComponentInChildren<CardItem>();
            if (tem._isSelected)
            {
                cardsSelected.Add(item.GetComponentInChildren<CardItem>());
            }
        }
        //��ѡ��Ŀ��ƽ����ж�
        //foreach (var item in cardsSelected)
        //{
        //    tem = item.GetComponentInChildren<CardItem>();
        //    if ()
        //    {

        //    }
        //}
        var duplicates = cardsSelected.GroupBy(x => x._cardInfo.id);
        int _sameCardNum = 0;
        foreach (var group in duplicates)
        {
            foreach (var item in group)
            {
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
