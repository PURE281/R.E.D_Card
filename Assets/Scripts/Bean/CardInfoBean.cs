using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumMgr;


[Serializable]
public class CardInfoBean
{
    public string id;
    public string name;
    public string cast;
    public int value;
    public CardType type;
    public string description;
    public string clipPath;
    public string spritePath;
    public string upgrade_id;
    public string combo_id;
    public string fusion_id;
    public string proficiency;
}
