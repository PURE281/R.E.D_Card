using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static EnumMgr;

public class BattleSceneMgr : MonoSington<BattleSceneMgr>
{
    private void Awake()
    {
        //≥ı ºªØΩ≈±æ
        this.AddComponent<BattleSystemMgr>();
        this.GetComponent<BattleSystemMgr>().global = false;
        this.AddComponent<EventCenter>().Init();
        this.GetComponent<EventCenter>().global = false;
        this.AddComponent<BattleUIMgr>();
        this.GetComponent<BattleUIMgr>().global = false;
        Application.targetFrameRate = 120;
    }
}
