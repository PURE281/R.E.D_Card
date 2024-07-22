using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static EnumMgr;

public class BattleSceneMgr : MonoSingleton<BattleSceneMgr>
{
    private void Awake()
    {
        //≥ı ºªØΩ≈±æ
        this.AddComponent<BattleSystemMgr>();
        this.AddComponent<EventCenter>().Init();
        this.AddComponent<BattleUIMgr>();
        Application.targetFrameRate = 120;
    }
}
