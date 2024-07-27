using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeckSceneMgr : MonoSington<DeckSceneMgr>
{
    [SerializeField]
    private const int pageSize = 10;

    public static int PageSize => pageSize;

    private void Awake()
    {
        this.AddComponent<DeckCardManager>();
        this.GetComponent<DeckCardManager>().Init();
        this.AddComponent<DeckUIManager>();
        this.GetComponent<DeckUIManager>().Init();
    }
}
