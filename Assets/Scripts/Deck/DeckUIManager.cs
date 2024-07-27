using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������չʾUI�Ľű�
/// </summary>
public class DeckUIManager : MonoSington<DeckUIManager>
{
    //һ���ǲ鿴ȫ�����ƵĿ���
    Dictionary<string, CardInfoBean> _allCardInfoDicts;
    //һ���ǵ�ǰ�û��Լ�ӵ�еĿ���
    Dictionary<string, CardInfoBean> _playerCardInfoDicts;

    //ui���
    GameObject _allCardInfoUIPanel;

    GameObject _playerCardInfoUIPanel;

    List<GameObject> _cardGOList;
    public void Init()
    {
        _allCardInfoDicts = DeckManager.Instance?.AllCardInfoDicts;
        _playerCardInfoDicts = DeckManager.Instance?.PlayerCardInfoDicts;
        if (_allCardInfoDicts == null)
        {
            ToastManager.Instance?.CreatToast("���ؿ�����Ϣʧ��");
            return;
        }
        //��ӵ���¼�
        Button[] buttons = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("ButtomBtnPanel").GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() =>
        {
            this.ShowPreviousCardsList();
        });
        buttons[1].onClick.AddListener(() =>
        {
            this.ShowNextCardsList();
        });
        //����Ԥ����
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
                //����5����Ƭ��Ԥ����
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
                //����5����Ƭ��Ԥ����
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
