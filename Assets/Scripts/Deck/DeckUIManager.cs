using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeckUIManager : MonoSington<DeckUIManager>
{
    GameObject _cardsPanelGo;
    List<GameObject> _cardsGo;

    GameObject _cardDetailPanel;
    GameObject _cardDetailImage;
    GameObject _cardDetailIntroduction;
    Button[] addAndRemCardBtns;
    DeckCardItem _temDeckCardItem;
    public void Init()
    {
        //ע�᷵���������¼�
        Button backBtn = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("Back").GetComponent<Button>();
        backBtn.onClick.AddListener(() =>
        {
            this.Back2Main();
        });
        _cardsPanelGo = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("CardPanel").gameObject;
        _cardsGo = new List<GameObject>();
        _cardDetailPanel = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("CardDetailPanel").gameObject;
        _cardDetailImage = _cardDetailPanel.transform.Find("CardPanel").GetChild(0).GetChild(0).gameObject;
        _cardDetailIntroduction = _cardDetailPanel.transform.Find("CardPanel").GetChild(1).gameObject;
        _cardDetailPanel.SetActive(false);
        _cardDetailPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            this.CloseCardDetail();
        });
        addAndRemCardBtns = this._cardDetailIntroduction.transform.GetChild(1).GetComponentsInChildren<Button>();
        //��ʼ���ײ�������ҳ��ť�����¼�
        Button[] _buttomBtns = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("ButtomBtnPanel").GetComponentsInChildren<Button>();
        _buttomBtns[0].onClick.AddListener(() =>
        {
            this.GetPreviousPageCards();
        });
        _buttomBtns[1].onClick.AddListener(() =>
        {
            this.GetNextPageCards();
        });
        //��ʼ�������л����鰴ť�����¼�
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
        //��ʼ����Ƭ����ҳ���������ť�ļ����¼�
        addAndRemCardBtns[0].onClick.AddListener(() =>
        {
            this.AddBattleCard();
        });
        addAndRemCardBtns[1].onClick.AddListener(() =>
        {
            this.RemoveBattleCard();
        });

        //����10��Ԥ����
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

    /// <summary>
    /// ����������
    /// </summary>
    public void SaveBattleCardInfo()
    {
        //CsvManager.Instance?.AddPlayerCard(new List<string>() { "1","0.5"});
    }

    public void ShowCardDetail(DeckCardItem deckCardItem)
    {
        this._temDeckCardItem = deckCardItem;
        this._cardDetailPanel.SetActive(true);
        this._cardDetailPanel.GetComponent<CanvasGroup>().interactable = true;
        this._cardDetailPanel.GetComponent<CanvasGroup>().alpha = 1;
        this._cardDetailPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        //��鵱ǰ�Ƿ�Ϊ��ҿ���,������ʾ���뿨����Ƴ������ѡ��
        this._cardDetailImage.GetComponent<Image>().sprite = deckCardItem._cardPic;
        //��ʾͼƬ����ʾ����
        PictureMgr.Instance?.AdjustImageToAspectFit(_cardDetailImage.GetComponent<Image>(), this._cardDetailImage.transform.parent.GetComponent<RectTransform>());
        string text = $"��Ƭ���ƣ�{deckCardItem._cardInfo.name}\n " +
            $"���ͣ�{deckCardItem._cardInfo.type}\n " +
            $"��ֵ��{deckCardItem._cardInfo.value}\n" +
            $"������{deckCardItem._cardInfo.description}";
        this._cardDetailIntroduction.transform.GetChild(0).GetComponentInChildren<Text>().DOText(text,1.5f);
        if (GlobalConfig.Instance?.DeckOption==1)
        {
            addAndRemCardBtns[0].interactable = false;
            addAndRemCardBtns[1].interactable = false;
        }
        else
        {
            addAndRemCardBtns[0].interactable = true;
            addAndRemCardBtns[1].interactable = true;
        }
    }


    public void CloseCardDetail()
    {
        this._cardDetailPanel.SetActive(false);
        this._cardDetailPanel.GetComponent<CanvasGroup>().interactable = false;
        this._cardDetailPanel.GetComponent<CanvasGroup>().alpha = 0;
        this._cardDetailPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        this._cardDetailIntroduction.transform.GetChild(0).GetComponentInChildren<Text>().text = null;

    }

    void AddBattleCard()
    {
        DeckCardManager.Instance?.AddBattleCard(_temDeckCardItem._cardInfo.id,0);
    }
    void RemoveBattleCard()
    {
        DeckCardManager.Instance?.RemoveBattleCard(_temDeckCardItem._cardInfo.id, 0);
    }

    void Back2Main()
    {
        SceneManager.LoadScene("MainScene");
    }
}
