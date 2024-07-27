using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理卡组展示UI的脚本
/// </summary>
public class DeckUIManager : MonoSington<DeckUIManager>
{
    //一个是查看全部卡牌的卡组
    Dictionary<string, CardInfoBean> _allCardInfoDicts;
    //一个是当前用户自己拥有的卡组
    Dictionary<string, CardInfoBean> _playerCardInfoDicts;

    //ui组件
    GameObject _allCardInfoUIPanel;

    GameObject _playerCardInfoUIPanel;

    List<GameObject> _cardGOList;
    public void Init()
    {
        _allCardInfoDicts = DeckManager.Instance?.AllCardInfoDicts;
        _playerCardInfoDicts = DeckManager.Instance?.PlayerCardInfoDicts;
        if (_allCardInfoDicts == null)
        {
            ToastManager.Instance?.CreatToast("加载卡组信息失败");
            return;
        }
        //添加点击事件
        Button[] buttons = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("ButtomBtnPanel").GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() =>
        {
            this.ShowPreviousCardsList();
        });
        buttons[1].onClick.AddListener(() =>
        {
            this.ShowNextCardsList();
        });
        //加载预制体
        _cardGOList = new List<GameObject>();
    }
    public void ShowNextCardsList()
    {
        List<CardInfoBean> cardInfoBeans = DeckManager.Instance.SelectNextPage();
        if (cardInfoBeans == null) return;
        if (_cardGOList.Count == 0)
        {
            for (int i = 0; i < DeckSceneMgr.Instance?.num; i++)
            {
                //生成5个卡片的预制体
                GameObject tem = Resources.Load<GameObject>("Prefabs/DeckCard");
                GameObject tem1 = Instantiate(tem);
                tem1.transform.parent = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("CardPanel");
                tem1.AddComponent<DeckCardItem>();
                _cardGOList.Add(tem1);
            }
        }
        if (_cardGOList.Count != cardInfoBeans.Count)
        {
            for (int i = cardInfoBeans.Count; i < _cardGOList.Count; i++)
            {
                _cardGOList[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _cardGOList.Count; i++)
            {
                _cardGOList[i].SetActive(true);
            }
        }
        for (int i = 0; i < cardInfoBeans.Count; i++)
        {
            _cardGOList[i].GetComponent<DeckCardItem>().Init(cardInfoBeans[i]);
            _cardGOList[i].GetComponent<DeckCardItem>().StartFront();
        }
    }
    public void ShowOriCardsList()
    {
        List<CardInfoBean> cardInfoBeans = DeckManager.Instance.GetCardsByPage();
        if (cardInfoBeans == null) return;
        if (_cardGOList.Count == 0)
        {
            for (int i = 0; i < DeckSceneMgr.Instance?.num; i++)
            {
                //生成5个卡片的预制体
                GameObject tem = Resources.Load<GameObject>("Prefabs/DeckCard");
                GameObject tem1 = Instantiate(tem);
                tem1.transform.parent = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("CardPanel");
                tem1.AddComponent<DeckCardItem>();
                _cardGOList.Add(tem1);
            }
        }
        if (_cardGOList.Count != cardInfoBeans.Count)
        {
            for (int i = cardInfoBeans.Count; i < _cardGOList.Count; i++)
            {
                _cardGOList[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _cardGOList.Count; i++)
            {
                _cardGOList[i].SetActive(true);
            }
        }
        for (int i = 0; i < cardInfoBeans.Count; i++)
        {
            _cardGOList[i].GetComponent<DeckCardItem>().Init(cardInfoBeans[i]);
            _cardGOList[i].GetComponent<DeckCardItem>().StartFront();
        }
    }
    public void ShowPreviousCardsList()
    {
        List<CardInfoBean> cardInfoBeans = DeckManager.Instance.SelectPreviousPage();
        if(cardInfoBeans==null) return;
        for (int i = 0; i < _cardGOList.Count; i++)
        {
            _cardGOList[i].GetComponent<DeckCardItem>().Init(cardInfoBeans[i]);
            _cardGOList[i].GetComponent<DeckCardItem>().StartFront();
        }
    }
}
