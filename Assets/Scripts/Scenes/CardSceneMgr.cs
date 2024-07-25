using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardSceneMgr : MonoSington<CardSceneMgr>
{
    private GameObject _getCardGo;
    private void Awake()
    {
        //StartCoroutine(CardGroupMgr.Instance?.InitCarddScene());
        this.AddComponent<CardGroupMgr>();
        //初始化功能键
        GameObject tem = Resources.Load<GameObject>("Prefabs/CommonBtn");
        _getCardGo = Instantiate(tem);
        _getCardGo.transform.parent = GameObject.FindGameObjectWithTag("MainCanvas").transform.Find("PC/BtnPanel");
        _getCardGo.name = tem.name;
        _getCardGo.GetComponent<Button>().onClick.AddListener(() =>
        {
            this.GetComponent<CardGroupMgr>().GetCard();
        });
        this.GetComponent<CardGroupMgr>()._startCardsbtn = _getCardGo.GetComponent<Button>();
        this.GetComponent<CardGroupMgr>().Init();
        this._getCardGo.GetComponentInChildren<Text>().text = "幸运小卡";
    }
}
