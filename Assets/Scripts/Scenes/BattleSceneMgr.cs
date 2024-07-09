using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleSceneMgr : MonoSingleton<BattleSceneMgr>
{
    private void Awake()
    {
        this.AddComponent<BattleSystemMgr>();
    }
}
