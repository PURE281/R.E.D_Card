using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumMgr;


[Serializable]
public class CharacterBean
{
    public int id;
    public string name;
    public int level;
    public int type;
    public float maxHP;//这是最大血量
    public float curHP;
    public float oriAtk;
    public float curAtk;
    public float oriDef;
    public float curDef;
    public string desc;
}
