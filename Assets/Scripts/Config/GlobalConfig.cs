using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalConfig:Singleton<GlobalConfig>
{
    private int _platform = 1;

    public int Platform { get => _platform; set => _platform = value; }
    /// <summary>
    /// 1����ȫ����,2�������ӵ�еĿ�,3����༭��
    /// </summary>
    public int DeckOption { get => _deckOption; set => _deckOption = value; }

    private int _deckOption = 1;

}
