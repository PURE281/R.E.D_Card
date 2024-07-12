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
    private RectTransform rectTransform; // ���ڴ洢UIԪ�ص�RectTransform���  

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
        rectTransform = GetComponent<RectTransform>(); // ��ȡUIԪ�ص�RectTransform  

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

    #region �P춱��ֈDƬ�����ķ���
    // ��������һ����������ȡͼƬ��ԭʼ�ߴ�  
    Vector2 GetOriginalImageSize(Sprite sprite)
    {
        return new Vector2(sprite.rect.width, sprite.rect.height);
    }

    // Ȼ������Ը���Ŀ�������Ŀ�߱�������Image��RectTransform  
    void AdjustImageToAspectFit(Image image, RectTransform container)
    {
        Sprite sprite = image.sprite;
        if (sprite == null) return;

        Vector2 originalSize = GetOriginalImageSize(sprite);
        float aspectRatio = originalSize.x / originalSize.y;

        // ����������Ҫ����ͼƬ�Ŀ�ȣ����������Ŀ���������߶�  
        float targetWidth = container.rect.width;
        float targetHeight = targetWidth / aspectRatio;

        // ���ڣ�������Ҫ����RectTransform��ê�㣨Anchors���ʹ�С��SizeDelta��  
        // ������������Ѿ������˺��ʵ�ê���pivot����Ӧ����  
        // ����ֻ����SizeDelta  
        image.rectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);

        // ע�⣺�������Ҫ���ָ߶Ȳ�������ȣ�ֻ�轻��width��height�ļ��㼴��  
    }
    #endregion
}
