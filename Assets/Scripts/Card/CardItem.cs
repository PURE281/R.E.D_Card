using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AssetsBundlesMgr;

public class CardItem : MonoSingleton<CardItem>, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public string _cast;
    public string _num;
    public string _type;
    public string _Description;
    public string _clipPath;
    public int _index;
    public Sprite _sprite;
    public bool _isSelect;

    private GameObject _mainCanvas;
    public GameObject _cardDescPanel;
    public AudioSource _audioSource;
    private RectTransform rectTransform; // 用于存储UI元素的RectTransform组件  

    private void Awake()
    {
        _mainCanvas = GameObject.FindWithTag("MainCanvas");
        _cardDescPanel = GameObject.FindWithTag("MainCanvas").transform.GetChild(1).GetChild(5).gameObject;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.playOnAwake = false;
        this.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            _isSelect = !_isSelect;
            this.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
            this.transform.DOScale(new Vector3(1.2f, 1.2f), 0.5f);
            this.transform.parent = _mainCanvas.transform.Find("PC/CardPanel2");
            if (_clipPath != null)
            {
                StartCoroutine(playClip());
            }
        });
        rectTransform = GetComponent<RectTransform>(); // 获取UI元素的RectTransform  

    }

    IEnumerator playClip()
    {

        Debug.Log(this._Description);
        WWW www = new WWW(this._clipPath);
        yield return www;
        _audioSource.clip = www.GetAudioClip();
        _audioSource.Play();
    }
    public void Init(CardInfo infos)
    {
        this._cast = infos.cast;
        this._num = infos.num;
        this._type = infos.type;
        this._Description = infos.description;
        this._clipPath = infos.clipPath;
        this._sprite = infos.sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position,

        eventData.pressEventCamera, out globalMousePos))

        {
            rectTransform.position = globalMousePos;
        }   

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {


        _cardDescPanel.SetActive(true);
        _cardDescPanel.GetComponentInChildren<Text>().text = this._Description;
        _cardDescPanel.transform.Find("picframe/pic").GetComponent<Image>().sprite = _sprite;
        AdjustImageToAspectFit(_cardDescPanel.transform.Find("picframe/pic").GetComponent<Image>(), _cardDescPanel.transform.Find("picframe").GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.parent = _mainCanvas.transform.Find("PC/CardPanel");
        this.transform.SetSiblingIndex(_index);
        this.transform.DOScale(Vector3.one, 0.5f);
        this.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
        _cardDescPanel.SetActive(false);
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
}
