using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnumMgr;


[Serializable]
public class CharacterBean
{
    public int _id;
    public string _name;
    public int _level;
    public int _type;
    public float _maxHP;
    public float _curHP;
    public float _oriAtk;
    public float _curAtk;
    public float _oriDef;
    public float _curDef;
    public string _description;
}
