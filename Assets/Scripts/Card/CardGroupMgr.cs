using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 这是用来管理抽卡的卡组的管理类
/// 实现功能，打乱卡牌顺序，随机按照顺序依次出现，然后依次转出正面，在特别的图片中需要有特定的处理
/// </summary>
public class CardGroupMgr : MonoSingleton<CardGroupMgr>
{
    private List<GameObject> cards = new List<GameObject>();
    private Dictionary<string, CardInfoBean> cardInfoDicts = new Dictionary<string, CardInfoBean>();

    //private List<Sprite> sprite_cards_list = new List<Sprite>();

    private bool isActive = false;

    public Button _startCardsbtn;

    void Awake()
    {

    }

    
}
