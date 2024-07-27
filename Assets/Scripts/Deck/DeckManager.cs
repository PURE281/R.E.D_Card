using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 管理卡牌的类
/// </summary>
public class DeckManager : MonoSington<DeckManager>
{
    //需要有三个卡组
    //一个是查看全部卡牌的卡组
    Dictionary<string, CardInfoBean> _allCardInfoDicts;
    //一个是当前用户自己拥有的卡组
    Dictionary<string, CardInfoBean> _playerCardInfoDicts;

    public Dictionary<string, CardInfoBean> PlayerCardInfoDicts { get => _playerCardInfoDicts; }
    public Dictionary<string, CardInfoBean> AllCardInfoDicts { get => _allCardInfoDicts; }

    //一个是编辑卡组面板

    public void Init()
    {
        //加载csv表,获取全部卡组,用户当前卡组
        _allCardInfoDicts = CsvManager.Instance?.ReadCardInfoCSVFile();

        _playerCardInfoDicts = CsvManager.Instance?.ReadPlayerCardInfoCSVFile();
        pageNum = (int)DeckSceneMgr.Instance?.num;
    }
    int curIndex = 1;
    int pageNum;
    /// <summary>
    /// 每页显示十个
    /// </summary>
    public List<CardInfoBean> SelectNextPage()
    {
        if (pageNum * (curIndex - 1) > _allCardInfoDicts.Count)
        {
            ToastManager.Instance?.CreatToast("当前已是最后一页");
            return null;
        }


        return GetCardsByPage(curIndex+1);
    }

    public List<CardInfoBean> SelectPreviousPage()
    {
        if (curIndex <= 1)
        {
            ToastManager.Instance?.CreatToast("当前已为第一页");
            return null;
        }
        return GetCardsByPage(curIndex-1);
    }

    public List<CardInfoBean> GetCardsByPage(int pageIndex = 1)
    {
        var list = _allCardInfoDicts.Skip(pageNum * (pageIndex - 1)).Take(pageNum).ToList();
        List<CardInfoBean> tem = new List<CardInfoBean>();
        foreach (var cardinfo in list)
        {
            tem.Add(cardinfo.Value);
        }
        curIndex = pageIndex;
        return tem;
    }
}
