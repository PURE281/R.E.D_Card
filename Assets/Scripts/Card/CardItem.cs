using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static AssetsBundlesMgr;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public enum CardType
{
    Atk, AtkUp, AtkDown, DefUp, DefDown, Sleep, Cover, None
}

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
public class CardItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardInfo _cardInfo;
    public int _index;

    private GameObject _mainCanvas;
    public GameObject _cardDescPanel;
    public AudioSource _audioSource;
    private RectTransform rectTransform; // ���ڴ洢UIԪ�ص�RectTransform���
    private Transform _orignTrans;
    private Transform _hightLightTrans;
    private GameObject _cardFront;
    private Sprite _cardPic;
    

    private void Awake()
    {
        _mainCanvas = GameObject.FindWithTag("MainCanvas");
        _cardDescPanel = GameObject.FindWithTag("MainCanvas").transform.GetChild(1).GetChild(5).gameObject;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.playOnAwake = false;
        _cardFront = this.transform.Find("CardFront").gameObject;
        _orignTrans = this.transform.parent;
        _hightLightTrans = this.transform.parent.parent.Find("CardPanel2");
        this.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            if (_cardInfo.clipPath != null)
            {
                _audioSource.Play();
            }
        });
        rectTransform = GetComponent<RectTransform>(); // ��ȡUIԪ�ص�RectTransform  

    }

    public void Init(CardInfo infos)
    {
        this._cardInfo = infos;
        //����ͼƬ
        Texture texture2D = Resources.Load<Texture2D>($"UI/Cards/{infos.name}");
        if (texture2D != null)
        {
            _cardPic = Sprite.Create((Texture2D)texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(10, 10));
            this._cardFront.transform.Find("Pic").GetComponent<Image>().sprite = _cardPic;
            AdjustImageToAspectFit(this._cardFront.transform.Find("Pic").GetComponent<Image>(), this._cardFront.GetComponent<RectTransform>());
        }

        //��ʾ���ص�����
        this._cardFront.transform.Find("CastPanel").GetComponentInChildren<Text>().text = infos.cast;
        this._cardFront.transform.Find("AtkPanel/Value").GetComponent<Text>().text = infos.value.ToString();
        this._cardFront.transform.Find("AtkPanel/Type").GetComponent<Text>().text = infos.type.ToString();
        this._audioSource.clip = Resources.Load<AudioClip>($"Music/clips/{infos.name}");

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
        // ת����ǰUIԪ�ص�RectTransform��Canvas������ϵ��  
        //��ѡ�еĵ�ת��ΪImage�����ڵı��ص�
        //�ж��Ƿ���������������ָ�����������
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_orignTrans.GetComponent<RectTransform>()
            , eventData.position, null, out localPoint);

        Vector2 pivot = _orignTrans.GetComponent<RectTransform>().pivot;
        Vector2 normalizedLocal =
            new Vector2(pivot.x + localPoint.x / _orignTrans.GetComponent<RectTransform>().sizeDelta.x
            , pivot.y + localPoint.y / _orignTrans.GetComponent<RectTransform>().sizeDelta.y);
        if ((normalizedLocal.x >= 0 && normalizedLocal.x <= 1) && ((normalizedLocal.y >= 0 && normalizedLocal.y <= 1)))
        {
            // ����rectTransform�ǰ�����Ҫˢ�²��ֵ�UIԪ�ص�RectTransform
            this.Back2OriginPanel();
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_orignTrans.GetComponent<RectTransform>());
            return;
        }
        //�ܵ�������ζ����Ҵ�����ƣ���ʼ�����ж�
        BattleSceneMgr.Instance?.GetComponent<BattleSystemMgr>().HandleCard(this._cardInfo);

        //LayoutRebuilder.ForceRebuildLayoutImmediate(_orignTrans.GetComponent<RectTransform>());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cardDescPanel.SetActive(true);
        _cardDescPanel.GetComponentInChildren<Text>().text = this._cardInfo.description;
        _cardDescPanel.transform.Find("picframe/pic").GetComponent<Image>().sprite = this._cardPic;
        AdjustImageToAspectFit(_cardDescPanel.transform.Find("picframe/pic").GetComponent<Image>(), _cardDescPanel.transform.Find("picframe").GetComponent<RectTransform>());
        this.ToHightlightPanel();
    
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.Back2OriginPanel();
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

    private void Back2OriginPanel()
    {
        this.transform.DOScale(1f, 0.3f);
        this.transform.SetParent(_orignTrans);
        this.transform.SetSiblingIndex(_index);
        this.GetComponentInParent<HorizontalLayoutGroup>().enabled = true;
    }

    private void ToHightlightPanel()
    {
        this.transform.DOScale(1.2f, 0.3f);
        this.GetComponentInParent<HorizontalLayoutGroup>().enabled = false;
        this.transform.SetParent(_hightLightTrans);
    }
}
