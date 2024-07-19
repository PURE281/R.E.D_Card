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
using Sequence = DG.Tweening.Sequence;



public class CardItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public CardInfoBean _cardInfo;//��Ƭ��Ϣ����
    public int _index;//�˿��ڿ����е�����(˳��)

    //private GameObject _mainCanvas;//��canvas
    //public GameObject _cardDescPanel;//�鿴��Ƭ����
    public AudioSource _audioSource;//��Ƶ������
    //private RectTransform rectTransform; // ���ڴ洢UIԪ�ص�RectTransform���
    //private Transform _orignTrans;//ԭ���鸸��
    //private Transform _hightLightTrans;//�������鸸��
    private GameObject _cardGo;//��Ƭ���
    public Sprite _cardPic;//��Ƭͼ

    private GameObject _menuPanel;//��ʾ��Ƭѡ���panel

    public bool _isSelected = false;

    private void Awake()
    {
        //_mainCanvas = GameObject.FindWithTag("MainCanvas");
        //_cardDescPanel = GameObject.FindWithTag("MainCanvas").transform.GetChild(1).GetChild(3).gameObject;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.playOnAwake = false;
        _cardGo = this.transform.parent.gameObject;
        //_orignTrans = _mainCanvas.transform.Find("PC/CardGroup/Panel");
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
        _menuPanel.transform.GetChild(0).GetComponent<Button>().interactable = false;
        _menuPanel.transform.GetChild(1).GetComponent<Button>().interactable = false;
        _menuPanel.transform.GetChild(2).GetComponent<Button>().interactable = false;
        _menuPanel.SetActive(false);
        //rectTransform = transform.parent.GetComponent<RectTransform>(); // ��ȡUIԪ�ص�RectTransform  

    }

    public void Init(CardInfoBean infos)
    {
        this._cardInfo = infos;
        //����ͼƬ
        Texture texture2D = Resources.Load<Texture2D>($"UI/Cards/{infos.name}");
        if (texture2D != null)
        {
            _cardPic = Sprite.Create((Texture2D)texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(10, 10));
            this.transform.Find("Pic").GetComponent<Image>().sprite = _cardPic;
            PictureMgr.Instance?.AdjustImageToAspectFit(this.transform.Find("Pic").GetComponent<Image>(), this.GetComponent<RectTransform>());
        }

        //��ʾ���ص�����
        this.transform.Find("CastPanel").GetComponentInChildren<Text>().text = infos.cast;
        this.transform.Find("AtkPanel/Value").GetComponent<Text>().text = infos.value.ToString();
        this.transform.Find("AtkPanel/Type").GetComponent<Text>().text = infos.type.ToString();
        this._audioSource.clip = Resources.Load<AudioClip>($"Music/clips/{infos.name}");

    }

    #region  ԭ�ƻ�ģ��¯ʯ��صĴ���
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
        //    // ����λ��  
        //    globalMousePos.x = Mathf.Clamp(globalMousePos.x, 0, _mainCanvas.GetComponent<RectTransform>().rect.width);
        //    globalMousePos.y = Mathf.Clamp(globalMousePos.y, 0, _mainCanvas.GetComponent<RectTransform>().rect.height);

        //    rectTransform.position = globalMousePos;
        //}

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ת����ǰUIԪ�ص�RectTransform��Canvas������ϵ��  
        //��ѡ�еĵ�ת��ΪImage�����ڵı��ص�
        //�ж��Ƿ���������������ָ�����������
        //Vector2 localPoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(_orignTrans.GetComponent<RectTransform>()
        //    , eventData.position, null, out localPoint);

        //Vector2 pivot = _orignTrans.GetComponent<RectTransform>().pivot;
        //Vector2 normalizedLocal =
        //    new Vector2(pivot.x + localPoint.x / _orignTrans.GetComponent<RectTransform>().sizeDelta.x
        //    , pivot.y + localPoint.y / _orignTrans.GetComponent<RectTransform>().sizeDelta.y);
        //if ((normalizedLocal.x >= 0 && normalizedLocal.x <= 1) && ((normalizedLocal.y >= 0 && normalizedLocal.y <= 1)))
        //{
        //    // ����rectTransform�ǰ�����Ҫˢ�²��ֵ�UIԪ�ص�RectTransform
        //    this.Back2OriginPanel();
        //    //LayoutRebuilder.ForceRebuildLayoutImmediate(_orignTrans.GetComponent<RectTransform>());
        //    return;
        //}
        //�ܵ�������ζ����Ҵ�����ƣ���ʼ�����ж�

        //LayoutRebuilder.ForceRebuildLayoutImmediate(_orignTrans.GetComponent<RectTransform>());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //_index = _cardGo.transform.GetSiblingIndex();
        //this.ToHightlightPanel();

        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_SHOW_CARD_DETAIL, this.gameObject);

    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //this.Back2OriginPanel();
        //_cardDescPanel.SetActive(false);
        //_cardDescPanel.GetComponentInChildren<Text>().text = null;
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL, this);
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


    public void Disappear()
    {
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_CLOSE_CARD_DETAIL);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            this._cardGo.transform.DOScale(1.2f, 0.3f);
            this._cardGo.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        }).AppendInterval(0.5f).AppendCallback(() =>
        {
            Destroy(this._cardGo);
        });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isSelected = !_isSelected;
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_REFRESH_CARDS, this.gameObject);
        if (_isSelected)
        {
            this.OpenMenu();
            this.transform.DOLocalMoveY(30, 0.2f);
        }
        else
        {
            this.transform.DOLocalMoveY(0, 0.2f);
            this.CloseMenu();
        }
        if (_cardInfo.clipPath != null)
        {
            _audioSource.Play();
        }
    }


    private void OpenMenu()
    {
        _menuPanel.SetActive(true);
        //��Ҫ�鿴�������Ƿ��п�������������Я�����ںϵ������
    }
    private void CloseMenu()
    {
        _menuPanel.SetActive(false);
        //��Ҫ�鿴�������Ƿ��п�������������Я�����ںϵ������
    }

    void UseCard()
    {
        BattleSceneMgr.Instance?.GetComponent<BattleSystemMgr>().HandleCard(_cardGo);
    }
    public void ShowUpdadteCard()
    {
        _menuPanel.transform.GetChild(2).GetComponent<Button>().interactable = true;
        //�����Ļ���Ҫ����������
    }

    public void CloseUpdadteCard()
    {
        _menuPanel.transform.GetChild(2).GetComponent<Button>().interactable = false;
    }
    void UpdadteCard()
    {
        //Debug.Log("������Ƭ");
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() =>
        {
            this.transform.DOScale(1.2f, 0.2f);

        }).AppendInterval(0.2f).AppendCallback(() =>
        {
            this.transform.DOScale(1f, 0.2f);
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
        Debug.Log("�����Я");
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
        Debug.Log("�ںϿ�");
        EventCenter.Instance?.dispatch(CustomEvent.BATTLE_UI_FUSION_CARDS, this.gameObject);
    }
}
