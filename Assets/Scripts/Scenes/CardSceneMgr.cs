using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSceneMgr : MonoSingleton<CardSceneMgr>
{
    private void Awake()
    {
        StartCoroutine(CardGroupMgr.Instance?.InitCarddScene());
    }
}
