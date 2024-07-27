using CSVToolKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using zFramework.Extension;

public class DeckCardManager : MonoSington<DeckCardManager>
{

    Dictionary<int, CardInfoBean> _allCardsDicts = new Dictionary<int, CardInfoBean>();
    Dictionary<int, CardInfoBean> _playerCardDicts = new Dictionary<int, CardInfoBean>();
    Dictionary<int, CardInfoBean> _battleCardDicts = new Dictionary<int, CardInfoBean>();
    Dictionary<int, CardInfoBean> _temCardDicts = new Dictionary<int, CardInfoBean>();

    int curPageInex = 0;

    int pageSize;

    public void Init()
    {
        pageSize = DeckSceneMgr.PageSize;
        List<CardInfoBean> allCardInfoBeans = CsvManager.Instance?.GetAllCards();
        //�������п�����Ϣ

        foreach (var cardInfoBean in allCardInfoBeans)
        {
            _allCardsDicts.Add(cardInfoBean.id, cardInfoBean);
        }

        List<PlayerCardBean> playerCardBeans = CsvManager.Instance?.GetPlayerCards();
        foreach (var cardInfoBean in playerCardBeans)
        {
            if (_allCardsDicts.ContainsKey(cardInfoBean.cid))
            {
                _playerCardDicts.Add(cardInfoBean.cid, _allCardsDicts[cardInfoBean.cid]);
            }
        }

        List<PlayerCardBean> battleCardBeans = CsvManager.Instance?.GetBattleCards();
        foreach (var cardInfoBean in battleCardBeans)
        {
            if (_allCardsDicts.ContainsKey(cardInfoBean.cid))
            {
                _battleCardDicts.Add(cardInfoBean.cid, _allCardsDicts[cardInfoBean.cid]);
            }
        }
        if (_allCardsDicts.Count == 0)
        {
            ToastManager.Instance?.CreatToast("������Ϣ����ʧ��");
            return;
        }
    }

    public List<CardInfoBean> NextPage()
    {
        var totalPages = (int)Mathf.Ceil((float)_temCardDicts.Count / pageSize); // ������ҳ��  
        if (totalPages == 0) return new List<CardInfoBean>();
        if (curPageInex >= totalPages)
        {

            ToastManager.Instance?.CreatToast("��ǰ�Ѿ������һҳ");
            return null; // ��������һҳ����û����һҳ  
        }
        return GetCards(curPageInex + 1);
    }

    public List<CardInfoBean> PreviousPage()
    {
        if (curPageInex <= 1)
        {
            ToastManager.Instance?.CreatToast("��ǰ�Ѿ��ǵ�һҳ");
            return null;
        }
        return GetCards(curPageInex - 1);
    }
    List<CardInfoBean> GetCards(int pageIndex)
    {
        List<KeyValuePair<int, CardInfoBean>> list = _temCardDicts.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
        List<CardInfoBean> values = new List<CardInfoBean>();
        foreach (var item in list)
        {
            values.Add(item.Value);
        }
        curPageInex = pageIndex;
        return values;
    }
    /// <summary>
    /// 1��������Ϊȫ������
    /// 2��������Ϊ��ҿ���
    /// 3��������Ϊս������
    /// </summary>
    /// <param name="deckOption"></param>
    public void SetTemDicts(int deckOption)
    {
        switch (deckOption)
        {
            case 1:
                //��ȫ�������н��з�ҳ��ѯ
                _temCardDicts = _allCardsDicts;
                ToastManager.Instance?.CreatToast("���л���ȫ������");
                break;
            case 2:
                //����ҿ����н��з�ҳ��ѯ
                _temCardDicts = _playerCardDicts;
                ToastManager.Instance?.CreatToast("���л�����ҿ���");
                break;
            case 3:
                //��ȫ�������н��з�ҳ��ѯ
                _temCardDicts = _battleCardDicts;
                ToastManager.Instance?.CreatToast("���л�����ս����");
                break;
        }
        this.curPageInex = 0;
        GlobalConfig.Instance.DeckOption = deckOption;

    }


    public void AddPlayerCard(int cid, int prof)
    {
        CsvManager.Instance?.AddPlayerCard(cid);
    }
    public void AddBattleCard(int cid, int prof)
    {
        CsvManager.Instance?.AddBattleCard(cid);
    }
    public void RemoveBattleCard(int cid, int prof)
    {
        List<int> tem = new List<int>() { cid, prof };
        //CSVParser.Instance.RemovePlayerCardData("/StreamingAssets/BattleCardData.csv", tem);
    }

}
