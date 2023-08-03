using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EGXScoreboardScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] positionTexts = new TextMeshProUGUI[8];      //holds position text
    [SerializeField] private Image[] positionImages = new Image[8];                         //holds position image
    [SerializeField] private Sprite[] characterSprites = new Sprite[4];                        //holds possible sprites

    public int[] playerScores = new int[8];                                  //holds players total kills
    public float[] playerTSLKs = new float[8];                               //holds players time since last kill
    public int[] playerSpriteIDs = new int[8];                                 //holds players time since last kill



    public void UpdateScoreboard(int newScore, float newTSLK, int newSpriteID)
    {
        //if new score is greater than a current score, replace lowest score and run sort
        for(int i = 0; i < playerScores.Length; i++)
        {
            if(newScore > playerScores[i]) { playerScores[9] = newScore; playerTSLKs[9] = newTSLK; playerSpriteIDs[9] = newSpriteID; SortScoreboard(); break; }
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
            //for (int i = 0; i < numOfPlayers; i++) { Debug.Log("BEFORE position" + i + ": player " + playerPositions[i] + " - score " + playerScores[i] + ", tslk " + playerTSLKs[i]); }
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

                //if (swapped) { Debug.Log("UPDATE position" + position + ": player " + playerPositions[position] + " - score " + playerScores[position] + ", tslk " + playerTSLKs[position]); }
                //else if(!swapped) { Debug.Log("NO UPDATE"); }
            }
            //Debug.Log("_____");
            //for (int i = 0; i < numOfPlayers; i++) { Debug.Log("AFTER position" + i + ": player " + playerPositions[i] + " - score " + playerScores[i] + ", tslk " + playerTSLKs[i]); }
            if (!swapped) { break; }
        }


        //Debug.Log("~~~~~~~");
        for (int position = 0; position < playerScores.Length; position++)
        {
            Debug.Log("position " + position + " - score: " + playerScores[position] + ", tslk: " + playerTSLKs[position]);
        }
    }


    private void DisplayScoreboard()
    {
        for (int position = 0; position < playerScores.Length; position++)
        {
            //update relevant position image with sprite related to sprite ID of current position
            //update relevant position text with position and score of current position
            positionImages[position].sprite = characterSprites[playerSpriteIDs[position]];
            positionTexts[position].text = "        Position " + position + ": ___     Score: " + playerScores[position];
        }
    }
}
