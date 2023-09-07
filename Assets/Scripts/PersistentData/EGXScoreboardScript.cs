using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EGXScoreboardScript : MonoBehaviour, EGXPersistenceInterface
{
    //scoreboard text & image references
    [SerializeField] private TextMeshProUGUI[] positionTexts = new TextMeshProUGUI[5];      //holds position text
    [SerializeField] private Image[] positionImages = new Image[5];                         //holds position image

    //scoreboard scores, time since last kills, & sprite references (tracks top 5 wins)
    private int[] playerScores = new int[5];                                                //holds players total kills
    private int[] playerSpriteIDs = new int[5];                                             //holds players sprites
    private float[] playerTSLKs = new float[5];                                             //holds players time since last kill
    private string[] playerNames = new string[5];                                           //holds players name

    //statistics scores & time since last kills (tracks all wins)
    private int[] storedScores;                                                             //holds players total kills
    private int[] storedSpriteIDs;                                                          //holds players sprites
    private float[] storedTSLKs;                                                            //holds players time since last kill
    private string[] storedNames;                                                           //holds players name

    //current winner scoreboard text & image reference
    [SerializeField] private TextMeshProUGUI curText;                                       //holds cur player text
    [SerializeField] private Image curImage;                                                //holds cur player image

    //current winner score, time since last kill, & sprite reference (tracks current winner)
    private int playerStatsPosition = -1;                                                   //holds cur players pos in statitics array
    private int playerScore;                                                                //holds cur player total kills
    private int playerSpriteID;                                                             //holds cur player sprites
    private float playerTSLK;                                                               //holds cur player time since last kill
    private string playerName;                                                              //holds cur player name

    //reference current EGXData script
    private EGXData egxData;



    void Awake()
    {
        for (int pos = 0; pos < positionTexts.Length; pos++) { positionTexts[pos].color = new Color32(0, 0, 0, 255); }
        curText.color = new Color32(0, 0, 0, 255);
    }

    void Start()
    {
        //Debug.Log("EGX scoreboard script awake");
        if(PlayerData.GetDevMode()) { PlayerData.GetTopPlayer(); }

        //set script vars to top player's in player data
        playerScore = PlayerData.playerScores[0];
        playerTSLK = PlayerData.playerTSLKs[0];
        playerSpriteID = PlayerData.GetSpriteID(PlayerData.playerPositions[0]);
        playerName = PlayerData.playerName;

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
            playerTSLKs[i] = data.playerTSLKs[i];
            playerSpriteIDs[i] = data.playerSpriteIDs[i];
            playerNames[i] = data.playerNames[i];
        }

        //update statistics (all) stats
        storedScores = new int[egxData.storedPositions];
        storedSpriteIDs = new int[egxData.storedPositions];
        storedTSLKs = new float[egxData.storedPositions];
        storedNames = new string[egxData.storedPositions];

        for (int i = 0; i < data.storedScores.Length; i++)
        {
            storedScores[i] = data.storedScores[i];
            storedTSLKs[i] = data.storedTSLKs[i];
            storedSpriteIDs[i] = data.storedSpriteIDs[i];
            storedNames[i] = data.storedNames[i];
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
            data.playerTSLKs[i] = playerTSLKs[i];
            data.playerSpriteIDs[i] = playerSpriteIDs[i];
            data.playerNames[i] = playerNames[i];
        }

        //update data script with statistics (all) stats
        for (int i = 0; i < data.storedPositions; i++)
        {
            data.storedScores[i] = storedScores[i];
            data.storedTSLKs[i] = storedTSLKs[i];
            data.storedSpriteIDs[i] = storedSpriteIDs[i];
            data.storedNames[i] = storedNames[i];
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
        int[] tempSSID = new int[storedScores.Length];
        float[] tempST = new float[storedScores.Length];
        string[] tempSN = new string[storedScores.Length];

        //if (PlayerData.GetDevMode()) { Debug.Log("temp storedScores: " + tempSS.Length + ", temp storedTSLKs: " + tempST.Length); }

        for (int i = 0; i < storedScores.Length; i++)
        {
            //if (PlayerData.GetDevMode()) { Debug.Log("filling pos " + i); }
            //fill temp vars
            tempSS[i] = this.storedScores[i];
            tempST[i] = this.storedTSLKs[i];
            tempSSID[i] = this.storedSpriteIDs[i];
            tempSN[i] = this.storedNames[i];
        }

        //Debug.Log("reiniting vars");
        //update num of positions & re-init store vars with new length
        this.storedScores = new int[egxData.storedPositions];
        this.storedSpriteIDs = new int[egxData.storedPositions];
        this.storedTSLKs = new float[egxData.storedPositions];
        this.storedNames = new string[egxData.storedPositions];
        //if (PlayerData.GetDevMode()) { Debug.Log("storedScores:" + storedScores.Length + ", storedTSLKs: " + storedTSLKs.Length); }

        for (int i = 0; i < egxData.storedPositions - 1; i++)
        {
            //if (PlayerData.GetDevMode()) { Debug.Log("filling pos " + i); }
            //fill re-inited store vars, ignore last
            if (tempSS[i] != 0) //accounts for the last being empty
            {
                this.storedScores[i] = tempSS[i];
                this.storedTSLKs[i] = tempST[i];
                this.storedSpriteIDs[i] = tempSSID[i];
                this.storedNames[i] = tempSN[i];
            }
            else
            {
                //if (PlayerData.GetDevMode()) { Debug.Log("pos " + i + " empty"); }
            }
        }

        //add current winner stats in
        storedScores[storedScores.Length - 1] = playerScore;
        storedTSLKs[storedScores.Length - 1] = playerTSLK;
        storedSpriteIDs[storedScores.Length - 1] = playerSpriteID;
        storedNames[storedScores.Length - 1] = playerName;

        //sort statistics
        SortStatistics(0, storedScores.Length-1);

        for(int i = 0; i < playerScores.Length; i++) { if(storedScores[i] == playerScore && storedTSLKs[i] == playerTSLK) { playerStatsPosition = i; break; } }
        if (PlayerData.GetDevMode()) { Debug.Log("position " + playerStatsPosition + " - score: " + storedScores[playerStatsPosition] + ", tslk: " + storedTSLKs[playerStatsPosition] + ", spriteID: " + storedSpriteIDs[playerStatsPosition] + ", name: " + storedNames[playerStatsPosition]); }
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

        //swap score in positions
        int tempSpriteID = storedSpriteIDs[low];
        storedSpriteIDs[low] = storedSpriteIDs[high];
        storedSpriteIDs[high] = tempSpriteID;

        //swap tslk in positions
        float tempTSLK = storedTSLKs[low];
        storedTSLKs[low] = storedTSLKs[high];
        storedTSLKs[high] = tempTSLK;

        //swap score in positions
        string tempName = storedNames[low];
        storedNames[low] = storedNames[high];
        storedNames[high] = tempName;
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
                playerNames[4] = playerName;
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

                    string tempName = playerNames[position];
                    playerNames[position] = playerNames[position + 1];
                    playerNames[position + 1] = tempName;

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

                    string tempName = playerNames[position];
                    playerNames[position] = playerNames[position + 1];
                    playerNames[position + 1] = tempName;

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
                Debug.Log("position " + position + " - score: " + playerScores[position] + ", tslk: " + playerTSLKs[position] + ", spriteID: " + playerSpriteIDs[position] + ", name: " + playerNames[position]);
            }
        }
    }



    private void DisplayScoreboard()
    {
        for (int position = 0; position < playerScores.Length; position++)
        {
            //update relevant position image with sprite related to sprite ID of current position
            //update relevant position text with position and score of current position

            Debug.Log(position);
            Debug.Log(playerSpriteIDs[position]);
            positionImages[position].sprite = PlayerData.GetSprite(playerSpriteIDs[position]);
            positionTexts[position].text = "        Position " + (position+1) + ": " + playerNames[position] + "     Score: " + playerScores[position] + "     TSLK: " + playerTSLKs[position].ToString("F2");
        }


        curImage.sprite = PlayerData.GetSprite(playerSpriteID);
        curText.text = "        Position " + (playerStatsPosition + 1) + ": " + playerName + "     Score: " + playerScore + "     TSLK: " + playerTSLK.ToString("F2");
    }
}
