using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumMgr;


[Serializable]
public class CardInfoBean
{
    public int id;
    public string name;
    public int cast;
    public int value;
    public int type;
    public string description;
    public string clipPath;
    public string spritePath;
    public int upgrade_id;
    public int combo_id;
    public int fusion_id;
    public int proficiency;
    public string probability;
}
