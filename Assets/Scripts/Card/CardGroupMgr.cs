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
/// ������������鿨�Ŀ���Ĺ�����
/// ʵ�ֹ��ܣ����ҿ���˳���������˳�����γ��֣�Ȼ������ת�����棬���ر��ͼƬ����Ҫ���ض��Ĵ���
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
