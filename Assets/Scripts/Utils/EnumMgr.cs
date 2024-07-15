using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumMgr
{
    public enum CardType
    {
        Atk, AtkUp, AtkDown, DefUp, DefDown, Sleep, Cover, None
    }


    public enum BattleType
    {
        Init,
        PlayerTurn,
        EnermyTurn,
        Winner,
        Lose,
        End
    }


    public enum CardPrefabType
    {
        BattleCard, Card
    }


    //¿¨ÅÆ×´Ì¬£¬ÕýÃæ¡¢±³Ãæ
    public enum CardState
    {
        Front,
        Back
    }
}
