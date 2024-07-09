using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneMgr : MonoSingleton<BattleSceneMgr>
{
    private void Awake()
    {
        this.AddComponent<BattleSystemMgr>();
        GameObject.Find("Getcard").GetComponent<Button>().onClick.AddListener(() =>
        {
            this.GetComponent<BattleSystemMgr>().GetCard();
        });
    }
}
