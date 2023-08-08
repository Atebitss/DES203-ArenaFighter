using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//code by Trevor Mock, How to make a Save & Load System in Unity - https://youtu.be/aUi9aijvpgs
[System.Serializable]
public class EGXData
{
    public int[] playerScores = new int[5];                                 //holds players total kills
    public int[] playerSpriteIDs = new int[5];                              //holds players sprites
    public float[] playerTSLKs = new float[5];                              //holds players time since last kill

    public int storedPositions;                                             //holds number of positions being tracked
    public int[] storedScores;                                              //holds players total kills
    public float[] storedTSLKs;                                             //holds players time since last kill

    public EGXData()
    {
        this.storedPositions = 5;
        this.storedScores = new int[storedPositions];
        this.storedTSLKs = new float[storedPositions];

        for (int i = 0; i < playerScores.Length; i++)
        {
            this.playerScores[i] = 0;
            this.playerSpriteIDs[i] = 0;
            this.playerTSLKs[i] = 0;
        }
    }
}
