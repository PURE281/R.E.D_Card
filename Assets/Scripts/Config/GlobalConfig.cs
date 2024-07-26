using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalConfig:Singleton<GlobalConfig>
{
    private int _platform = 1;

    public int Platform { get => _platform; set => _platform = value; }
    /// <summary>
    /// 1代表全部卡,2代表玩家拥有的卡,3代表编辑卡
    /// </summary>
    public int DeckOption { get => _deckOption; set => _deckOption = value; }

    private int _deckOption = 1;

}
