using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EGXData
{
    public int[] playerScores = new int[8];                                  //holds players total kills
    public int[] playerSpriteIDs = new int[8];                               //holds players sprites
    public float[] playerTSLKs = new float[8];                               //holds players time since last kill

    public EGXData()
    {
        this.playerScores = null;
        this.playerSpriteIDs = null;
        this.playerTSLKs = null;
    }
}
