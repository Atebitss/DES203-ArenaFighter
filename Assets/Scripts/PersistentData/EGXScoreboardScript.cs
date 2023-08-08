using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EGXScoreboardScript : MonoBehaviour, EGXPersistenceInterface
{
    [SerializeField] private Sprite[] characterSprites = new Sprite[4];                     //holds possible sprites


    [SerializeField] private TextMeshProUGUI[] positionTexts = new TextMeshProUGUI[5];      //holds position text
    [SerializeField] private Image[] positionImages = new Image[5];                         //holds position image

    private int[] playerScores = new int[5];                                                //holds players total kills
    private int[] playerSpriteIDs = new int[5];                                             //holds players sprites
    private float[] playerTSLKs = new float[5];                                             //holds players time since last kill

    private int[] storedScores;                                                             //holds players total kills
    private float[] storedTSLKs;                                                            //holds players time since last kill


    [SerializeField] private TextMeshProUGUI curText;                                       //holds cur player text
    [SerializeField] private Image curImage;                                                //holds cur player image

    private int playerScore;                                                                //holds cur player total kills
    private int playerSpriteID;                                                             //holds cur player sprites
    private float playerTSLK;                                                               //holds cur player time since last kill



    void Start()
    {
        //Debug.Log("EGX scoreboard script awake");

        playerScore = PlayerData.playerScores[0];
        playerSpriteID = PlayerData.playerScores[0];
        playerTSLK = PlayerData.playerTSLKs[0];

        //Debug.Log("score: " + playerScore + ", spriteID: " + playerSpriteID + ", tslk: " + playerTSLK);

        UpdateScoreboard();
        DisplayScoreboard();
        PlayerData.ResetScores();
    }


    //
    public void LoadData(EGXData data)
    {
        //Debug.Log("EGX scoreboard script load data");
        storedScores = new int[data.storedPositions];

        for(int i = 0; i < data.playerScores.Length; i++)
        {
            playerScores[i] = data.playerScores[i];
            playerSpriteIDs[i] = data.playerSpriteIDs[i];
            playerTSLKs[i] = data.playerTSLKs[i];
        }
    }

    public void SaveData(EGXData data)
    {
        //Debug.Log("EGX scoreboard script save data");
        for (int i = 0; i < data.playerScores.Length; i++)
        {
            data.playerScores[i] = playerScores[i];
            data.playerSpriteIDs[i] = playerSpriteIDs[i];
            data.playerTSLKs[i] = playerTSLKs[i];
        }
    }
    //


    public void UpdateScoreboard()
    {
        //Debug.Log("updating EGX scoreboard");
        //if new score is greater than a current score, replace lowest score and run sort
        for(int i = 0; i < playerScores.Length; i++)
        {
            if(playerScore > playerScores[i])
            { 
                //Debug.Log("players score greater than scoreboard position " + i); 
                playerScores[4] = playerScore; 
                playerTSLKs[4] = playerTSLK; 
                playerSpriteIDs[4] = playerSpriteID; 
                SortScoreboard(); 
                break; 
            }
        }
    }


    private void SortScoreboard()
    {
        bool swapped;

        //Debug.Log("____________");
        for (int check = 0; check < playerScores.Length; check++)
        {
            swapped = false;

            //Debug.Log("_____");
            //for (int i = 0; i < numOfPlayers; i++) { //Debug.Log("BEFORE position" + i + ": player " + playerPositions[i] + " - score " + playerScores[i] + ", tslk " + playerTSLKs[i]); }
            //Debug.Log("_____");
            for (int position = 0; position < playerScores.Length - check - 1; position++)
            {
                //Debug.Log("CURRENT position" + position + ": player " + playerPositions[position] + " - score " + playerScores[position] + ", tslk " + playerTSLKs[position]);

                if (playerScores[position] < playerScores[position + 1])
                {
                    //Debug.Log("player " + playerPositions[position] + " score " + playerScores[position] + "  <  " + "player " + playerPositions[position + 1] + " score " + playerScores[position + 1]);

                    int tempScore = playerScores[position];
                    playerScores[position] = playerScores[position + 1];
                    playerScores[position + 1] = tempScore;

                    float tempTSLK = playerTSLKs[position];
                    playerTSLKs[position] = playerTSLKs[position + 1];
                    playerTSLKs[position + 1] = tempTSLK;

                    int tempSprite = playerSpriteIDs[position];
                    playerSpriteIDs[position] = playerSpriteIDs[position + 1];
                    playerSpriteIDs[position + 1] = tempSprite;

                    swapped = true;
                }
                else if (playerScores[position] == playerScores[position + 1] && playerTSLKs[position] > playerTSLKs[position + 1])
                {
                    //Debug.Log("player " + playerPositions[position] + " score " + playerScores[position] + "  ==  " + "player " + playerPositions[position + 1] + " score " + playerScores[position + 1]);
                    //Debug.Log("scores equal");
                    //Debug.Log("player " + playerPositions[position] + " tslk " + playerTSLKs[position] + "  v  " + "player " + playerPositions[position + 1] + " tslk " + playerTSLKs[position + 1]);

                    int tempScore = playerScores[position];
                    playerScores[position] = playerScores[position + 1];
                    playerScores[position + 1] = tempScore;

                    float tempTSLK = playerTSLKs[position];
                    playerTSLKs[position] = playerTSLKs[position + 1];
                    playerTSLKs[position + 1] = tempTSLK;

                    int tempSprite = playerSpriteIDs[position];
                    playerSpriteIDs[position] = playerSpriteIDs[position + 1];
                    playerSpriteIDs[position + 1] = tempSprite;

                    swapped = true;
                }

                //if (swapped) { //Debug.Log("UPDATE position" + position + ": player " + playerPositions[position] + " - score " + playerScores[position] + ", tslk " + playerTSLKs[position]); }
                //else if(!swapped) { //Debug.Log("NO UPDATE"); }
            }
            //Debug.Log("_____");
            //for (int i = 0; i < numOfPlayers; i++) { //Debug.Log("AFTER position" + i + ": player " + playerPositions[i] + " - score " + playerScores[i] + ", tslk " + playerTSLKs[i]); }
            if (!swapped) { break; }
        }


        //Debug.Log("~~~~~~~");
        for (int position = 0; position < playerScores.Length; position++)
        {
            //Debug.Log("position " + position + " - score: " + playerScores[position] + ", tslk: " + playerTSLKs[position]);
        }
    }


    private void DisplayScoreboard()
    {
        for (int position = 0; position < playerScores.Length; position++)
        {
            //update relevant position image with sprite related to sprite ID of current position
            //update relevant position text with position and score of current position

            //positionImages[position].sprite = characterSprites[playerSpriteIDs[position]];
            positionTexts[position].text = "        Position " + (position+1) + ": ___     Score: " + playerScores[position] + "     TSLK: " + playerTSLKs[position].ToString("F2");
        }


        //curImage.sprite = characterSprites[playerSpriteID];
        curText.text = "        Position " + "" + ": ___     Score: " + playerScore + "     TSLK: " + playerTSLK.ToString("F2");
    }
}
