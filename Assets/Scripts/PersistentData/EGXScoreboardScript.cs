using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EGXScoreboardScript : MonoBehaviour, EGXPersistenceInterface
{
    //character sprite references
    [SerializeField] private Sprite[] characterSprites = new Sprite[4];                     //holds possible sprites


    //scoreboard text & image references
    [SerializeField] private TextMeshProUGUI[] positionTexts = new TextMeshProUGUI[5];      //holds position text
    [SerializeField] private Image[] positionImages = new Image[5];                         //holds position image

    //scoreboard scores, time since last kills, & sprite references (tracks top 5 wins)
    private int[] playerScores = new int[5];                                                //holds players total kills
    private int[] playerSpriteIDs = new int[5];                                             //holds players sprites
    private float[] playerTSLKs = new float[5];                                             //holds players time since last kill

    //statistics scores & time since last kills (tracks all wins)
    private int[] storedScores;                                                             //holds players total kills
    private float[] storedTSLKs;                                                            //holds players time since last kill

    //current winner scoreboard text & image reference
    [SerializeField] private TextMeshProUGUI curText;                                       //holds cur player text
    [SerializeField] private Image curImage;                                                //holds cur player image

    //current winner score, time since last kill, & sprite reference (tracks current winner)
    private int playerStatsPosition = -1;                                                        //holds cur players pos in statitics array
    private int playerScore;                                                                //holds cur player total kills
    private int playerSpriteID;                                                             //holds cur player sprites
    private float playerTSLK;                                                               //holds cur player time since last kill

    //reference current EGXData script
    private EGXData egxData;



    void Start()
    {
        //Debug.Log("EGX scoreboard script awake");

        //set script vars to top player's in player data
        playerScore = PlayerData.playerScores[0];
        playerSpriteID = PlayerData.playerScores[0];
        playerTSLK = PlayerData.playerTSLKs[0];

        //Debug.Log("score: " + playerScore + ", spriteID: " + playerSpriteID + ", tslk: " + playerTSLK);
        //increase stored positions by 1 and fill array
        if (egxData.storedScores[0] != 0) { egxData.AlterStoredPositions(1); }
        UpdateStatistics();

        //update displayed scoreboard stats, display the scoreboard stats, reset round data
        UpdateScoreboard();
        DisplayScoreboard();
        PlayerData.ResetRound();
    }


    //
    public void LoadData(EGXData data)
    {
        //Debug.Log("EGX scoreboard script load data");
        //if (PlayerData.GetDevMode()) { Debug.Log("EGX scoreboard script load data"); }

        //set script reference
        egxData = data;

        //update scoreboard (top5) stats
        for(int i = 0; i < data.playerScores.Length; i++)
        {
            playerScores[i] = data.playerScores[i];
            playerSpriteIDs[i] = data.playerSpriteIDs[i];
            playerTSLKs[i] = data.playerTSLKs[i];
        }

        //update statistics (all) stats
        storedScores = new int[egxData.storedPositions];
        storedTSLKs = new float[egxData.storedPositions];

        for (int i = 0; i < data.storedScores.Length; i++)
        {
            storedScores[i] = data.storedScores[i];
            storedTSLKs[i] = data.storedTSLKs[i];
        }


        if (PlayerData.GetDevMode())
        {
            for (int i = 0; i < data.playerScores.Length; i++)
            {
                //Debug.Log("Loaded pos" + i + " - Score: " + playerScores[i] + ", TSLK: " + playerTSLKs[i] + ", spriteID: " + playerSpriteIDs[i]);
            }

            for (int i = 0; i < data.storedPositions; i++)
            {
                //Debug.Log("Loaded pos" + i + " - Score: " + storedScores[i] + ", TSLK: " + storedTSLKs[i]);
            }
        }
    }

    public void SaveData(EGXData data)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("EGX scoreboard script save data"); }

        //update data script with scoreboard (top5) stats
        for (int i = 0; i < data.playerScores.Length; i++)
        {
            data.playerScores[i] = playerScores[i];
            data.playerSpriteIDs[i] = playerSpriteIDs[i];
            data.playerTSLKs[i] = playerTSLKs[i];
        }

        //update data script with statistics (all) stats
        for (int i = 0; i < data.storedPositions; i++)
        {
            data.storedScores[i] = storedScores[i];
            data.storedTSLKs[i] = storedTSLKs[i];
        }


        if (PlayerData.GetDevMode())
        {
            for (int i = 0; i < data.playerScores.Length; i++)
            {
                //Debug.Log("Saved pos" + i + " - Score: " + playerScores[i] + ", TSLK: " + playerTSLKs[i] + ", spriteID: " + playerSpriteIDs[i]);
            }

            for (int i = 0; i < data.storedPositions; i++)
            {
                //Debug.Log("Saved pos" + i + " - Score: " + storedScores[i] + ", TSLK: " + storedTSLKs[i]);
            }
        }
    }
    //


    private void UpdateStatistics()
    {
        if (PlayerData.GetDevMode()) { Debug.Log("EGXScorebaord UpdateStatistics"); }
        //if (PlayerData.GetDevMode()) { Debug.Log("storedScores:" + storedScores.Length + ", storedTSLKs: " + storedTSLKs.Length); }

        //init temp variables to hold current stats
        int[] tempSS = new int[storedScores.Length];
        float[] tempST = new float[storedScores.Length];

        //if (PlayerData.GetDevMode()) { Debug.Log("temp storedScores: " + tempSS.Length + ", temp storedTSLKs: " + tempST.Length); }

        for (int i = 0; i < storedScores.Length; i++)
        {
            //if (PlayerData.GetDevMode()) { Debug.Log("filling pos " + i); }
            //fill temp vars
            tempSS[i] = this.storedScores[i];
            tempST[i] = this.storedTSLKs[i];
        }

        //Debug.Log("reiniting vars");
        //update num of positions & re-init store vars with new length
        this.storedScores = new int[egxData.storedPositions];
        this.storedTSLKs = new float[egxData.storedPositions];
        //if (PlayerData.GetDevMode()) { Debug.Log("storedScores:" + storedScores.Length + ", storedTSLKs: " + storedTSLKs.Length); }

        for (int i = 0; i < egxData.storedPositions - 1; i++)
        {
            //if (PlayerData.GetDevMode()) { Debug.Log("filling pos " + i); }
            //fill re-inited store vars, ignore last
            if (tempSS[i] != 0) //accounts for the last being empty
            {
                this.storedScores[i] = tempSS[i];
                this.storedTSLKs[i] = tempST[i];
            }
            else
            {
                //if (PlayerData.GetDevMode()) { Debug.Log("pos " + i + " empty"); }
            }
        }

        //add current winner stats in
        storedScores[storedScores.Length - 1] = playerScore;
        storedTSLKs[storedScores.Length - 1] = playerTSLK;

        //sort statistics
        SortStatistics(0, storedScores.Length-1);

        for(int i = 0; i < playerScores.Length; i++) { if(storedScores[i] == playerScore && storedTSLKs[i] == playerTSLK) { playerStatsPosition = i; break; } }
        if (PlayerData.GetDevMode()) { Debug.Log("position " + playerStatsPosition + " - score: " + storedScores[playerStatsPosition] + ", tslk: " + storedTSLKs[playerStatsPosition]); }
        else if (PlayerData.GetDevMode() && playerStatsPosition == -1) { Debug.Log("player not found"); }
    }

    private void SortStatistics(int low, int high)
    {
        //if low int is less than high int
        if(low < high)
        {
            //find center point of array
            int pivot = Partition(low, high);

            SortStatistics(low, pivot - 1);     //sort left half of array
            SortStatistics(pivot + 1, high);    //sort right half of array
        }
    }

    private int Partition(int low, int high)
    {
        //set last number as pivot point
        int pivotScore = storedScores[high];
        float pivotTSLK = storedTSLKs[high];
        int i = low - 1;

        //sort nums by smaller/larger
        for(int j = low; j < high; j++)
        {
            //if lower score is higher than current high score, or if lower score is equal to higher score and score related TSLK is lower than higher TSLK
            if(storedScores[j] > pivotScore || (storedScores[j] == pivotScore && storedTSLKs[j] < pivotTSLK))
            {
                i++;
                //move smaller numbers left
                Swap(i, j);
            }
        }

        //track current players position
        if(storedScores[i+1] == playerScore)
        {
            playerStatsPosition = i + 1;
        }

        //update pivot
        Swap(i + 1, high);

        //return pivot
        return i + 1;
    }

    private void Swap(int low, int high)
    {
        //swap score in positions
        int tempScore = storedScores[low];
        storedScores[low] = storedScores[high];
        storedScores[high] = tempScore;

        //swap tslk in positions
        float tempTSLK = storedTSLKs[low];
        storedTSLKs[low] = storedTSLKs[high];
        storedTSLKs[high] = tempTSLK;
    }



    private void UpdateScoreboard()
    {
        //Debug.Log("updating EGX scoreboard");
        //if new score is greater than a current score, replace lowest score and run sort
        for(int i = 0; i < playerScores.Length; i++)
        {
            if(playerScore > playerScores[i] || playerScore == playerScores[i] && playerTSLK < playerTSLKs[i])
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
        if (PlayerData.GetDevMode())
        {
            for (int position = 0; position < playerScores.Length; position++)
            {
                Debug.Log("position " + position + " - score: " + playerScores[position] + ", tslk: " + playerTSLKs[position]);
            }
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
        curText.text = "        Position " + (playerStatsPosition + 1) + ": ___     Score: " + playerScore + "     TSLK: " + playerTSLK.ToString("F2");
    }
}
