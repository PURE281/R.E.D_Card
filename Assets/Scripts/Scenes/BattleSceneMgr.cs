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
        ///添加出卡的监听事件
        this.AddComponent<BattleSystemMgr>();
        Button temBtn = Resources.Load<Button>("Prefabs/CommonBtn");
        Button pullCardBtn = Instantiate(temBtn);
        pullCardBtn.name = "PullCardBtn";
        pullCardBtn.GetComponentInChildren<Text>().text = "打出";
        pullCardBtn.onClick.AddListener(() =>
        {
            this.GetComponent<BattleSystemMgr>().SwitchBattleType(BattleType.EnermyTurn);
        });
        pullCardBtn.transform.SetParent(this._btnPanel);
        pullCardBtn.transform.localPosition = Vector3.zero;
        this._battleBtnList.Add(pullCardBtn);
        
        ///添加抽卡的监听事件
        Button getcardBtn = Instantiate(temBtn);
        getcardBtn.name = "GetcardBtn";
        getcardBtn.GetComponentInChildren<Text>().text = "抽卡";
        getcardBtn.onClick.AddListener(() =>
        {
            this.GetComponent<BattleSystemMgr>().GetCard();
        });
        getcardBtn.transform.SetParent(this._btnPanel);
        getcardBtn.transform.localPosition = Vector3.zero;
        this._battleBtnList.Add(getcardBtn);

    }

}
