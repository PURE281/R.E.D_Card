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
        //注册返回主场景事件
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
        //初始化卡片详情页面的两个按钮的监听事件
        addAndRemCardBtns[0].onClick.AddListener(() =>
        {
            this.AddBattleCard();
        });
        addAndRemCardBtns[1].onClick.AddListener(() =>
        {
            this.RemoveBattleCard();
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

    /// <summary>
    /// 这是用来将
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
        //检查当前是否为玩家卡组,是则显示加入卡组和移出卡组的选项
        this._cardDetailImage.GetComponent<Image>().sprite = deckCardItem._cardPic;
        //显示图片，显示文字
        PictureMgr.Instance?.AdjustImageToAspectFit(_cardDetailImage.GetComponent<Image>(), this._cardDetailImage.transform.parent.GetComponent<RectTransform>());
        string text = $"卡片名称：{deckCardItem._cardInfo.name}\n " +
            $"类型：{deckCardItem._cardInfo.type}\n " +
            $"数值：{deckCardItem._cardInfo.value}\n" +
            $"描述：{deckCardItem._cardInfo.description}";
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
