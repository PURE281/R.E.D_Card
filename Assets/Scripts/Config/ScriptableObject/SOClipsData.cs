using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "_Data", menuName = "Music/Clips", order = 1)]
public class SOClipsData : ScriptableObject
{
    public AudioClip[] clips;
    public string[] desc;
}
