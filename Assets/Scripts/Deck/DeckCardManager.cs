using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckCardManager : MonoSington<DeckCardManager>
{

    Dictionary<string, CardInfoBean> _playerCardDicts;
    Dictionary<string, CardInfoBean> _allCardDicts;
    Dictionary<string, CardInfoBean> _temCardDicts;

    int curPageInex = 0;

    int pageSize;

    public void Init()
    {
        pageSize = DeckSceneMgr.PageSize;
        _playerCardDicts = new Dictionary<string, CardInfoBean>();
        _allCardDicts = CsvManager.Instance?.ReadCardInfoCSVFile();
        _playerCardDicts = CsvManager.Instance?.ReadPlayerCardInfoCSVFile();
        if (_allCardDicts.Count == 0)
        {
            ToastManager.Instance?.CreatToast("卡牌信息加载失败");
            return;
        }
    }


    public List<CardInfoBean> NextPage()
    {
        var totalPages = (int)Mathf.Ceil((float)_temCardDicts.Count / pageSize); // 计算总页数  
        if (curPageInex >= totalPages) {

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
        List<KeyValuePair<string, CardInfoBean>> list = _temCardDicts.Skip(pageSize*(pageIndex-1)).Take(pageSize).ToList();
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
                _temCardDicts = _allCardDicts;
                break;
            case 2:
                //从玩家卡组中进行分页查询
                _temCardDicts = _playerCardDicts;
                break;
            case 3:
                //从全部卡组中进行分页查询
                _temCardDicts = _playerCardDicts;
                break;
        }
        this.curPageInex = 0;
        GlobalConfig.Instance.DeckOption = deckOption;

    }
}
