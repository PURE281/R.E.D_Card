using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalConfig:Singleton<GlobalConfig>
{
    private int _platform;

    public int Platform { get => _platform; set => _platform = value; }
}
