using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static EnumMgr;

public class BattleSceneMgr : MonoSingleton<BattleSceneMgr>
{
    public List<Button> _battleBtnList;
    public Transform _btnPanel;
    private void Awake()
    {
        ///��ӳ����ļ����¼�
        this.AddComponent<BattleSystemMgr>();
        Button temBtn = Resources.Load<Button>("Prefabs/CommonBtn");
        Button pullCardBtn = Instantiate(temBtn);
        pullCardBtn.name = "PullCardBtn";
        pullCardBtn.GetComponentInChildren<Text>().text = "���";
        pullCardBtn.onClick.AddListener(() =>
        {
            this.GetComponent<BattleSystemMgr>().SwitchBattleType(BattleType.EnermyTurn);
        });
        pullCardBtn.transform.SetParent(this._btnPanel);
        pullCardBtn.transform.localPosition = Vector3.zero;
        this._battleBtnList.Add(pullCardBtn);
        
        ///��ӳ鿨�ļ����¼�
        Button getcardBtn = Instantiate(temBtn);
        getcardBtn.name = "GetcardBtn";
        getcardBtn.GetComponentInChildren<Text>().text = "�鿨";
        getcardBtn.onClick.AddListener(() =>
        {
            this.GetComponent<BattleSystemMgr>().GetCard();
        });
        getcardBtn.transform.SetParent(this._btnPanel);
        getcardBtn.transform.localPosition = Vector3.zero;
        this._battleBtnList.Add(getcardBtn);

    }

}
