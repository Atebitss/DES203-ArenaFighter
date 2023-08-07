using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EGXData
{
    public int[] playerScores = new int[5];                                  //holds players total kills
    public int[] playerSpriteIDs = new int[5];                               //holds players sprites
    public float[] playerTSLKs = new float[5];                               //holds players time since last kill

    public EGXData()
    {
        for (int i = 0; i < playerScores.Length; i++)
        {
            this.playerScores[i] = 0;
            this.playerSpriteIDs[i] = 0;
            this.playerTSLKs[i] = 0;
        }
    }
}
