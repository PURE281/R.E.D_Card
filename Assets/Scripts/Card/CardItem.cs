using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardItem : MonoSingleton<CardItem>, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    public int _HPMax;
    public int _HP;
    public int _MPMax;
    public int _MP;
    public string _Name;
    public string _Description;
    public int _index;

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.transform.DOScale(new Vector3(1.2f, 1.2f), 0.5f);
        this.transform.parent = GameObject.FindWithTag("MainCanvas").transform.Find("PC/CardPanel2");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.parent = GameObject.FindWithTag("MainCanvas").transform.Find("PC/CardPanel");
        this.transform.SetSiblingIndex(_index);  
        this.transform.DOScale(Vector3.one, 0.5f);
        Debug.Log("Àë¿ª");
    }

}
