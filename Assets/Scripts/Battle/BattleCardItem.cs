using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static EnumMgr;
using static UnityEditor.PlayerSettings;
using Sequence = DG.Tweening.Sequence;



public class BattleCardItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public GameObject _mFront;//卡牌正面
    public GameObject _mBack;//卡牌背面
    public CardState mCardState = CardState.Front;//卡牌当前的状态，是正面还是背面？
    public float mTime = 0.3f;
    private bool isActive = false;//true代表正在执行翻转，不许被打断

    public CardInfoBean _cardInfo;//卡片信息对象
    public int _index;//此卡在卡组中的索引(顺序)

    public AudioClip _cardClip;//音频播放器
    public Sprite _cardPic;//卡片图
    private GameObject _menuPanel;//显示卡片选项的panel
    public bool _isSelected = false;
    public bool _isDestroy = false;

    private void Awake()
    {
        _mFront = this.transform.GetChild(0).gameObject;
        _mBack = this.transform.GetChild(1).gameObject;
        _menuPanel = this.transform.GetChild(2).gameObject;
        if (_menuPanel != null)
        {
            _menuPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                this.ComboCard();
            });
            _menuPanel.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() =>
            {
                this.FusionCard();
            });
            _menuPanel.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() =>
            {
                this.UpdadteCard();
            });
            _menuPanel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() =>
            {
                this.UseCard();
            });
            _menuPanel.transform.GetChild(0).GetComponent<Button>().interactable = false;
            _menuPanel.transform.GetChild(1).GetComponent<Button>().interactable = false;
            _menuPanel.transform.GetChild(2).GetComponent<Button>().interactable = false;
            _menuPanel.SetActive(false);
        }

        //rectTransform = transform.parent.GetComponent<RectTransform>(); // 获取UI元素的RectTransform  

    }
    /// <summary>
    /// 留给外界调用的接口
    /// </summary>
    public void StartBack()
    {
        if (isActive)
            return;
        StartCoroutine(ToBack());
    }
    /// <summary>
    /// 留给外界调用的接口
    /// </summary>
    public void StartFront()
    {
        if (isActive)
            return;
        StartCoroutine(ToFront());
    }
    /// <summary>
    /// 翻转到背面
    /// </summary>
    public IEnumerator ToBack()
    {
        isActive = true;
        _mFront.transform.DORotate(new Vector3(0, 90, 0), mTime);
        for (float i = mTime; i >= 0; i -= Time.deltaTime)
            yield return 0;
        _mBack.transform.DORotate(new Vector3(0, 0, 0), mTime);
        isActive = false;

    }
    /// <summary>
    /// 翻转到正面
    /// </summary>
    public IEnumerator ToFront()
    {
        isActive = true;
        _mBack.transform.DORotate(new Vector3(0, 90, 0), mTime);
        MusicManager.Instance?.PlayClipByIndex(0);
        for (float i = mTime; i >= 0; i -= Time.deltaTime)
            yield return 0;
        _mFront.transform.DORotate(new Vector3(0, 0, 0), mTime);
        //不同卡的效果，如果是ssr和ur进行震动效果
        Sequence sequence = DOTween.Sequence();
        switch (this._cardInfo.probability)
        {
            case "r":
                //_mFront.transform.DOShakePosition(2, new Vector3(5, 5, 0));

                //sequence.AppendCallback(() =>
                //{
                //    _mFront.transform.DOScale(1.2f, 1.5f);
                //}).AppendInterval(2).AppendCallback(() =>
                //{
                //    _mFront.transform.DOScale(1f, 0.5f);
                //});
                break;
            case "sr":
                //_mFront.transform.DOShakePosition(2, new Vector3(5, 5, 0));
                sequence.AppendCallback(() =>
                {
                    _mFront.transform.DOScale(1.1f, 1f);
                }).AppendInterval(1).AppendCallback(() =>
                {
                    _mFront.transform.DOScale(1f, 0.5f);
                });
                break;
            case "ssr":
                _mFront.transform.DOShakePosition(2, new Vector3(5, 5, 0));

                sequence.AppendCallback(() =>
                {
                    _mFront.transform.DOScale(1.2f, 1.5f);
                }).AppendInterval(1.5f).AppendCallback(() =>
                {
                    _mFront.transform.DOScale(1f, 0.5f);
                });
                break;
            case "ur":
                _mFront.transform.DOShakePosition(3, new Vector3(10, 10, 0));

                sequence.AppendCallback(() =>
                {
                    _mFront.transform.DOScale(1.2f, 1.5f);
                }).AppendInterval(2).AppendCallback(() =>
                {
                    _mFront.transform.DOScale(1f, 0.1f);
                });
                break;
        }
        
        isActive = false;
    }
    public void Init(CardInfoBean infos)
    {
        this._cardInfo = infos;
        //加载图片
        Texture texture2D = Resources.Load<Texture2D>($"UI/Cards/{infos.name}");
        if (texture2D != null)
        {
            _cardPic = Sprite.Create((Texture2D)texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(10, 10));
            this._mFront.transform.Find("Pic").GetComponent<Image>().sprite = _cardPic;
            PictureMgr.Instance?.AdjustImageToAspectFit(this._mFront.transform.Find("Pic").GetComponent<Image>(), this.GetComponent<RectTransform>());
        }

        //显示加载的数据
        this._mFront.transform.Find("ProPanel").GetComponentInChildren<Text>().text = HightlightPro(infos.probability);
        this._mFront.transform.Find("AtkPanel/Value").GetComponent<Text>().text = infos.value.ToString();
        this._mFront.transform.Find("AtkPanel/Type").GetComponent<Text>().text = infos.type.ToString();
        this._cardClip = Resources.Load<AudioClip>($"Music/clips/{infos.name}");

    }
    /// <summary>
    /// 根据不同的稀有度改变富文本的格式
    /// </summary>
    /// <param name="probability"></param>
    /// <returns></returns>
    string HightlightPro(string probability)
    {
        string richText = "";
        switch (probability)
        {
            case "r":
                richText = $"<b><i>{probability.ToUpper()}</i></b>";
                break;
            case "sr":
                richText = $"<b><color=brown><i>{probability.ToUpper()}</i></color></b>";
                break;
            case "ssr":
                richText = $"<b><color=orange><i>{probability.ToUpper()}</i></color></b>";
                break;
            case "ur":
                richText = $"<b><color=red><i>{probability.ToUpper()}</i></color></b>";
                break;
        }
        return richText;
    }
    #region  原计划模仿炉石相关的代码
    public void OnBeginDrag(PointerEventData eventData)
    {
        //_cardDescPanel.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Vector3 globalMousePos;
        //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position,

        //eventData.pressEventCamera, out globalMousePos))

        //{
        //    // 限制位置  
        //    globalMousePos.x = Mathf.Clamp(globalMousePos.x, 0, _mainCanvas.GetComponent<RectTransform>().rect.width);
        //    globalMousePos.y = Mathf.Clamp(globalMousePos.y, 0, _mainCanvas.GetComponent<RectTransform>().rect.height);

        //    rectTransform.position = globalMousePos;
        //}

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 转换当前UI元素的RectTransform到Canvas的坐标系中  
        //将选中的点转换为Image区域内的本地点
        //判断是否在手牌区域，是则恢复，不做处理
        //Vector2 localPoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(_orignTrans.GetComponent<RectTransform>()
        //    , eventData.position, null, out localPoint);

        //Vector2 pivot = _orignTrans.GetComponent<RectTransform>().pivot;
        //Vector2 normalizedLocal =
        //    new Vector2(pivot.x + localPoint.x / _orignTrans.GetComponent<RectTransform>().sizeDelta.x
        //    , pivot.y + localPoint.y / _orignTrans.GetComponent<RectTransform>().sizeDelta.y);
        //if ((normalizedLocal.x >= 0 && normalizedLocal.x <= 1) && ((normalizedLocal.y >= 0 && normalizedLocal.y <= 1)))
        //{
        //    // 假设rectTransform是包含需要刷新布局的UI元素的RectTransform
        //    this.Back2OriginPanel();
        //    //LayoutRebuilder.ForceRebuildLayoutImmediate(_orignTrans.GetComponent<RectTransform>());
        //    return;
        //}
        //能到这里意味着玩家打出了牌，开始进行判定

        //LayoutRebuilder.ForceRebuildLayoutImmediate(_orignTrans.GetComponent<RectTransform>());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //_index = _cardGo.transform.GetSiblingIndex();
        //this.ToHightlightPanel();

        //EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_CARD_DETAIL, this.gameObject);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //this.Back2OriginPanel();
        //_cardDescPanel.SetActive(false);
        //_cardDescPanel.GetComponentInChildren<Text>().text = null;
        //EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL, this);
    }
    private void Back2OriginPanel()
    {
        //this._cardGo.transform.DOScale(1f, 0.3f);
        //this._cardGo.transform.SetParent(_orignTrans);
        //this._cardGo.transform.SetSiblingIndex(_index);
        //this._orignTrans.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    private void ToHightlightPanel()
    {
        //this._cardGo.transform.DOScale(1.2f, 0.3f);
        //this._orignTrans.GetComponent<HorizontalLayoutGroup>().enabled = false;
        //this._cardGo.transform.SetParent(_hightLightTrans);
    }
    #endregion


    /// <summary>
    /// 这是让卡片销毁的方法
    /// </summary>
    public void Disappear()
    {
        _isDestroy = true;
        BattleSystemMgr.Instance?.RemoveCardInHand(this.transform.gameObject);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            this.transform.DOLocalMoveX(-600, 0.3f);
            this.transform.DOLocalMoveY(300, 0.3f);
            this.GetComponent<CanvasGroup>().DOFade(0, 0.2f);
        }).AppendInterval(0.2f).AppendCallback(() =>
        {
            EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_RESET_CARDS);
            EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL);
            Destroy(this.gameObject);
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isSelected = !_isSelected;
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_REFRESH_CARDS, this.gameObject);
        if (_isSelected)
        {
            this.OpenMenu();
            EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_CARD_DETAIL, this.gameObject);
        }
        else
        {
            this.CloseMenu();
            EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL, this.gameObject);
        }
        if (_cardClip != null)
        {
            MusicManager.Instance?.PlayClipByClip(this._cardClip);
        }
    }


    public void OpenMenu()
    {
        this.transform.DOLocalMoveY(30, 0.2f);
        _menuPanel.SetActive(true);
        //需要查看手牌中是否有可以升级或者连携或者融合的相关牌
    }
    public void CloseMenu()
    {
        this.transform.DOLocalMoveY(0, 0.2f);
        _menuPanel.SetActive(false);
        //需要查看手牌中是否有可以升级或者连携或者融合的相关牌
    }

    void UseCard()
    {
        BattleSceneMgr.Instance?.GetComponent<BattleSystemMgr>().HandleCard(this.gameObject);
    }
    public void ShowUpdadteCard()
    {
        _menuPanel.transform.GetChild(2).GetComponent<Button>().interactable = true;
        //升级的话需要更新熟练度
    }

    public void CloseUpdadteCard()
    {
        _menuPanel.transform.GetChild(2).GetComponent<Button>().interactable = false;
    }
    void UpdadteCard()
    {
        //Debug.Log("升级卡片");
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            this.transform.DOScale(1.3f, 0.5f);
            MusicManager.Instance?.PlayClipByIndex(2);


        }).AppendInterval(0.5f).AppendCallback(() =>
        {
            this.transform.DOScale(0.8f, 0.2f);
        });
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_UPDATE_CARDS, this.gameObject);
    }

    public void ShowComboCard()
    {
        _menuPanel.transform.GetChild(0).GetComponent<Button>().interactable = true;
    }

    public void CloseComboCard()
    {
        _menuPanel.transform.GetChild(0).GetComponent<Button>().interactable = false;
    }
    void ComboCard()
    {
        Debug.Log("打出连携");
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_COMBO_CARDS, this.gameObject);
    }
    public void ShowFusionCard()
    {
        _menuPanel.transform.GetChild(1).GetComponent<Button>().interactable = true;
    }

    public void CloseFusionCard()
    {
        _menuPanel.transform.GetChild(1).GetComponent<Button>().interactable = false;
    }
    void FusionCard()
    {
        Debug.Log("融合卡");
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_FUSION_CARDS, this.gameObject);
    }
}
