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
    public int _index;

    private GameObject _mainCanvas;
    public GameObject _cardDescPanel;

    private void Awake()
    {
        _mainCanvas = GameObject.FindWithTag("MainCanvas");
        _cardDescPanel = GameObject.FindWithTag("MainCanvas").transform.GetChild(1).GetChild(2).gameObject;
        this.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log(this._Description);
            _cardDescPanel.SetActive(true);
            _cardDescPanel.GetComponentInChildren<Text>().text = this._Description;
        });
    }
    public void Init(CardInfo infos)
    {
        this._cast = infos.cast;
        this._num = infos.num;
        this._type = infos.type;
        this._Description = infos.description;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;
        this.transform.DOScale(new Vector3(1.2f, 1.2f), 0.5f);
        this.transform.parent = _mainCanvas.transform.Find("PC/CardPanel2");
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

}
