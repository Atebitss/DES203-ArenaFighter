using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//code origionally by Trevor Mock, How to make a Save & Load System in Unity - https://youtu.be/aUi9aijvpgs
[System.Serializable]
public class EGXData
{
    public int[] playerScores = new int[5];                                 //holds players total kills
    public int[] playerSpriteIDs = new int[5];                              //holds players sprites
    public float[] playerTSLKs = new float[5];                              //holds players time since last kill

    public int storedPositions = 1;                                         //holds number of positions being tracked
    public int[] storedScores = new int[1];                                 //holds players total kills
    public float[] storedTSLKs = new float[1];                              //holds players time since last kill

    public EGXData()
    {
        //init store vars
        for (int i = 0; i < playerScores.Length; i++)
        {
            this.playerScores[i] = 0;
            this.playerSpriteIDs[i] = 0;
            this.playerTSLKs[i] = 0;
        }

        for (int i = 0; i < storedPositions; i++)
        {
            this.storedScores[i] = 0;
            this.storedTSLKs[i] = 0;
        }
    }

    public void AlterStoredPositions(int inc)
    {
        if (PlayerData.GetDevMode()) { Debug.Log("EGXData AlterStoredPositions, increment: " + inc); }
        if (PlayerData.GetDevMode()) { Debug.Log("storedScores:" + storedScores.Length + ", storedTSLKs: " + storedTSLKs.Length); }

        //init temp variables to hold current stats
        int[] tempSS = new int[storedPositions];
        float[] tempST = new float[storedPositions];

        if (PlayerData.GetDevMode()) { Debug.Log("temp storedScores: " + tempSS.Length + ", temp storedTSLKs: " + tempST.Length); }

        for (int i = 0; i < storedPositions; i++)
        {
            if (PlayerData.GetDevMode()) { Debug.Log("filling pos " + i); }
            //fill temp vars
            tempSS[i] = this.storedScores[i];
            tempST[i] = this.storedTSLKs[i];
        }

        //update num of positions & re-init store vars with new length
        this.storedPositions += inc;
        this.storedScores = new int[storedPositions];
        this.storedTSLKs = new float[storedPositions];

        for (int i = 0; i < storedPositions-1; i++)
        {
            //fill re-inited store vars, ignore last
            if (tempSS[i] != 0) //accounts for the last being empty
            {
                this.storedScores[i] = tempSS[i];
                this.storedTSLKs[i] = tempST[i];
            }
        }
    }
}
