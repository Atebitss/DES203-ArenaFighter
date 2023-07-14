using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Haptics 
{
    public string name;
    [Range(0f, 1f)]
    public float leftRumble;
    [Range(0f, 1f)]
    public float rightRumble;

    public float duration;
   
}
