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
            ToastManager.Instance?.CreatToast("������Ϣ����ʧ��");
            return;
        }
    }


    public List<CardInfoBean> NextPage()
    {
        var totalPages = (int)Mathf.Ceil((float)_temCardDicts.Count / pageSize); // ������ҳ��  
        if (curPageInex >= totalPages) {

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
                _temCardDicts = _allCardDicts;
                break;
            case 2:
                //����ҿ����н��з�ҳ��ѯ
                _temCardDicts = _playerCardDicts;
                break;
            case 3:
                //��ȫ�������н��з�ҳ��ѯ
                _temCardDicts = _playerCardDicts;
                break;
        }
        this.curPageInex = 0;
        GlobalConfig.Instance.DeckOption = deckOption;

    }
}
