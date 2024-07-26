using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckUIManager : MonoBehaviour
{
    GameObject _cardsPanelGo;
    List<GameObject> _cardsGo;
    public void Init()
    {
        _cardsPanelGo = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("CardPanel").gameObject;
        _cardsGo = new List<GameObject>();
        //初始化底部的上下页按钮监听事件
        Button[] _buttomBtns = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("ButtomBtnPanel").GetComponentsInChildren<Button>();
        _buttomBtns[0].onClick.AddListener(() =>
        {
            this.GetPreviousPageCards();
        });
        _buttomBtns[1].onClick.AddListener(() =>
        {
            this.GetNextPageCards();
        });
        //初始化左侧的切换卡组按钮监听事件
        Button[] _lefttns = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("LeftBtnPanel").GetComponentsInChildren<Button>();
        _lefttns[0].onClick.AddListener(() =>
        {
            DeckCardManager.Instance?.SetTemDicts(1);
            this.GetNextPageCards();
        });
        _lefttns[1].onClick.AddListener(() =>
        {
            DeckCardManager.Instance?.SetTemDicts(2);
            this.GetNextPageCards();
        });
        _lefttns[2].onClick.AddListener(() =>
        {
            DeckCardManager.Instance?.SetTemDicts(3);
            this.GetNextPageCards();
        });

        //生成10个预制体
        GameObject tem = Resources.Load<GameObject>("Prefabs/DeckCard");
        for (int i = 0; i < DeckSceneMgr.PageSize; i++)
        {
            GameObject tem1 = Instantiate(tem);
            tem1.transform.SetParent(_cardsPanelGo.transform);
            tem1.AddComponent<DeckCardItem>();
            _cardsGo.Add(tem1);
        }
        DeckCardManager.Instance?.SetTemDicts(1);
        this.GetNextPageCards();
    }

    void GetNextPageCards()
    {
        List<CardInfoBean> cardInfoBeans = DeckCardManager.Instance?.NextPage();
        if (cardInfoBeans == null) return;
        int minLength = Mathf.Min(_cardsGo.Count,cardInfoBeans.Count);
        if (minLength != DeckSceneMgr.PageSize)
        {
            for (int i = minLength; i < _cardsGo.Count; i++)
            {
                _cardsGo[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _cardsGo.Count; i++)
            {
                _cardsGo[i].SetActive(true);
            }
        }
        for (int i = 0; i < cardInfoBeans.Count; i++)
        {
            _cardsGo[i].GetComponent<DeckCardItem>().Init(cardInfoBeans[i]);
            _cardsGo[i].GetComponent<DeckCardItem>().StartFront();
        }
    }
    void GetPreviousPageCards()
    {
        List<CardInfoBean> cardInfoBeans = DeckCardManager.Instance?.PreviousPage();
        if (cardInfoBeans == null) return;
        int minLength = Mathf.Min(_cardsGo.Count, cardInfoBeans.Count);
        if (minLength != DeckSceneMgr.PageSize)
        {
            for (int i = minLength; i < _cardsGo.Count; i++)
            {
                _cardsGo[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < _cardsGo.Count; i++)
            {
                _cardsGo[i].SetActive(true);
            }
        }
        for (int i = 0; i < cardInfoBeans.Count; i++)
        {
            _cardsGo[i].GetComponent<DeckCardItem>().Init(cardInfoBeans[i]);
        }
    }
}
