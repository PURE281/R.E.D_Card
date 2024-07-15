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



[Serializable]
public class CardInfo
{
    public string id;
    public string name;
    public string cast;
    public int value;
    public CardType type;
    public string description;
    public string clipPath;
    public string spritePath;
}
public class CardItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public CardInfo _cardInfo;//卡片信息对象
    public int _index;//此卡在卡组中的索引(顺序)

    private GameObject _mainCanvas;//主canvas
    public GameObject _cardDescPanel;//查看卡片详情
    public AudioSource _audioSource;//音频播放器
    //private RectTransform rectTransform; // 用于存储UI元素的RectTransform组件
    private Transform _orignTrans;//原卡组父级
    //private Transform _hightLightTrans;//高亮卡组父级
    private GameObject _cardGo;//卡片最父级
    private Sprite _cardPic;//卡片图

    private GameObject _menuPanel;//显示卡片选项的panel

    bool _isSelected = false;

    private void Awake()
    {
        _mainCanvas = GameObject.FindWithTag("MainCanvas");
        _cardDescPanel = GameObject.FindWithTag("MainCanvas").transform.GetChild(1).GetChild(3).gameObject;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.playOnAwake = false;
        _cardGo = this.transform.parent.gameObject;
        _orignTrans = _mainCanvas.transform.Find("PC/CardGroup/Panel");
        //_hightLightTrans = _mainCanvas.transform.Find("PC/CardGroup2");
        _menuPanel = this._cardGo.transform.GetChild(2).gameObject;
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
        _menuPanel.SetActive(false);
        //rectTransform = transform.parent.GetComponent<RectTransform>(); // 获取UI元素的RectTransform  

    }

    public void Init(CardInfo infos)
    {
        this._cardInfo = infos;
        //加载图片
        Texture texture2D = Resources.Load<Texture2D>($"UI/Cards/{infos.name}");
        if (texture2D != null)
        {
            _cardPic = Sprite.Create((Texture2D)texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(10, 10));
            this.transform.Find("Pic").GetComponent<Image>().sprite = _cardPic;
            AdjustImageToAspectFit(this.transform.Find("Pic").GetComponent<Image>(), this.GetComponent<RectTransform>());
        }

        //显示加载的数据
        this.transform.Find("CastPanel").GetComponentInChildren<Text>().text = infos.cast;
        this.transform.Find("AtkPanel/Value").GetComponent<Text>().text = infos.value.ToString();
        this.transform.Find("AtkPanel/Type").GetComponent<Text>().text = infos.type.ToString();
        this._audioSource.clip = Resources.Load<AudioClip>($"Music/clips/{infos.name}");

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _cardDescPanel.SetActive(false);
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

    }
    void ShowDesc()
    {
        _cardDescPanel.SetActive(true);
        _cardDescPanel.GetComponentInChildren<Text>().text = this._cardInfo.description;
        _cardDescPanel.transform.Find("picframe/pic").GetComponent<Image>().sprite = this._cardPic;
        AdjustImageToAspectFit(_cardDescPanel.transform.Find("picframe/pic").GetComponent<Image>(), _cardDescPanel.transform.Find("picframe").GetComponent<RectTransform>());
    }
    void CloseDesc()
    {
        _cardDescPanel.SetActive(false);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //this.Back2OriginPanel();
        //_cardDescPanel.SetActive(false);
        //_cardDescPanel.GetComponentInChildren<Text>().text = null;
    }

    #region 關於保持圖片比例的方法
    // 假设你有一个方法来获取图片的原始尺寸  
    Vector2 GetOriginalImageSize(Sprite sprite)
    {
        return new Vector2(sprite.rect.width, sprite.rect.height);
    }

    // 然后，你可以根据目标容器的宽高比来调整Image的RectTransform  
    void AdjustImageToAspectFit(Image image, RectTransform container)
    {
        Sprite sprite = image.sprite;
        if (sprite == null) return;

        Vector2 originalSize = GetOriginalImageSize(sprite);
        float aspectRatio = originalSize.x / originalSize.y;

        // 假设我们想要保持图片的宽度，根据容器的宽度来调整高度  
        float targetWidth = container.rect.width;
        float targetHeight = targetWidth / aspectRatio;

        // 现在，我们需要调整RectTransform的锚点（Anchors）和大小（SizeDelta）  
        // 这里假设容器已经设置了合适的锚点和pivot来适应内容  
        // 我们只调整SizeDelta  
        image.rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);

        // 注意：如果你想要保持高度并调整宽度，只需交换width和height的计算即可  
    }
    #endregion

    private void Back2OriginPanel()
    {
        this._cardGo.transform.DOScale(1f, 0.3f);
        this._cardGo.transform.SetParent(_orignTrans);
        this._cardGo.transform.SetSiblingIndex(_index);
        this._orignTrans.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }

    private void ToHightlightPanel()
    {
        this._cardGo.transform.DOScale(1.2f, 0.3f);
        this._orignTrans.GetComponent<HorizontalLayoutGroup>().enabled = false;
        //this._cardGo.transform.SetParent(_hightLightTrans);
    }

    public void Disappear()
    {
        StartCoroutine(DisappearIE());
    }
    IEnumerator DisappearIE()
    {
        this.CloseDesc();   
        this._orignTrans.transform.GetComponent<HorizontalLayoutGroup>().enabled = true;
        this._cardGo.transform.DOScale(1.2f, 0.3f);
        this._cardGo.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        Destroy(this._cardGo);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(123);
        _isSelected = !_isSelected;
        if (_isSelected)
        {
            this.OpenMenu();
            this.ShowDesc();
        }
        else
        {
            this.CloseMenu();
            this.CloseDesc();
        }
        if (_cardInfo.clipPath != null)
        {
            _audioSource.Play();
        }
    }


    private void OpenMenu()
    {
        _menuPanel.SetActive(true);
        //需要查看手牌中是否有可以升级或者连携或者融合的相关牌
    }
    private void CloseMenu()
    {
        _menuPanel.SetActive(false);
        //需要查看手牌中是否有可以升级或者连携或者融合的相关牌
    }

    void UseCard()
    {
        BattleSceneMgr.Instance?.GetComponent<BattleSystemMgr>().HandleCard(_cardGo);
    }
    void UpdadteCard()
    {
        Debug.Log("升级卡片");
    }

    void ComboCard()
    {
        Debug.Log("打出连携");
    }
    void FusionCard()
    {
        Debug.Log("融合卡");
    }
}
