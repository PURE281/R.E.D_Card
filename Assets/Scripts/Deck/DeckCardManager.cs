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
        //加载所有卡牌信息

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
            ToastManager.Instance?.CreatToast("卡牌信息加载失败");
            return;
        }
    }

    public List<CardInfoBean> NextPage()
    {
        var totalPages = (int)Mathf.Ceil((float)_temCardDicts.Count / pageSize); // 计算总页数  
        if (totalPages == 0) return new List<CardInfoBean>();
        if (curPageInex >= totalPages)
        {

            ToastManager.Instance?.CreatToast("当前已经是最后一页");
            return null; // 如果是最后一页，则没有下一页  
        }
        return GetCards(curPageInex + 1);
    }

    public List<CardInfoBean> PreviousPage()
    {
        if (curPageInex <= 1)
        {
            ToastManager.Instance?.CreatToast("当前已经是第一页");
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
    /// 1代表设置为全部卡组
    /// 2代表设置为玩家卡组
    /// 3代表设置为战斗卡组
    /// </summary>
    /// <param name="deckOption"></param>
    public void SetTemDicts(int deckOption)
    {
        switch (deckOption)
        {
            case 1:
                //从全部卡组中进行分页查询
                _temCardDicts = _allCardsDicts;
                ToastManager.Instance?.CreatToast("已切换至全部卡组");
                break;
            case 2:
                //从玩家卡组中进行分页查询
                _temCardDicts = _playerCardDicts;
                ToastManager.Instance?.CreatToast("已切换至玩家卡组");
                break;
            case 3:
                //从全部卡组中进行分页查询
                _temCardDicts = _battleCardDicts;
                ToastManager.Instance?.CreatToast("已切换至对战卡组");
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
