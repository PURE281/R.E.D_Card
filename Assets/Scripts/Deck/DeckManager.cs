using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �����Ƶ���
/// </summary>
public class DeckManager : MonoSington<DeckManager>
{
    //��Ҫ����������
    //һ���ǲ鿴ȫ�����ƵĿ���
    Dictionary<string, CardInfoBean> _allCardInfoDicts;
    //һ���ǵ�ǰ�û��Լ�ӵ�еĿ���
    Dictionary<string, CardInfoBean> _playerCardInfoDicts;

    public Dictionary<string, CardInfoBean> PlayerCardInfoDicts { get => _playerCardInfoDicts; }
    public Dictionary<string, CardInfoBean> AllCardInfoDicts { get => _allCardInfoDicts; }

    //һ���Ǳ༭�������

    public void Init()
    {
        //����csv��,��ȡȫ������,�û���ǰ����
        _allCardInfoDicts = CsvManager.Instance?.ReadCardInfoCSVFile();

        _playerCardInfoDicts = CsvManager.Instance?.ReadPlayerCardInfoCSVFile();
        pageNum = (int)DeckSceneMgr.Instance?.num;
    }
    int curIndex = 1;
    int pageNum;
    /// <summary>
    /// ÿҳ��ʾʮ��
    /// </summary>
    public List<CardInfoBean> SelectNextPage()
    {
        if (pageNum * (curIndex - 1) > _allCardInfoDicts.Count)
        {
            ToastManager.Instance?.CreatToast("��ǰ�������һҳ");
            return null;
        }


        return GetCardsByPage(curIndex+1);
    }

    public List<CardInfoBean> SelectPreviousPage()
    {
        if (curIndex <= 1)
        {
            ToastManager.Instance?.CreatToast("��ǰ��Ϊ��һҳ");
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
