using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeckSceneMgr : MonoSington<DeckSceneMgr>
{
    public int num = 10;
    private void Awake()
    {
        this.AddComponent<DeckManager>();
        this.GetComponent<DeckManager>().Init();
        this.AddComponent<DeckUIManager>();
        this.GetComponent<DeckUIManager>().Init();
        this.GetComponent<DeckUIManager>().ShowOriCardsList();
    }
}
