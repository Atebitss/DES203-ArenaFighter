using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreSort : MonoBehaviour
{
    static private int[] playerScores = new int[PlayerData.numOfPlayers];      //hold the players current playerScores
    static private float[] playerTSLKs = new float[PlayerData.numOfPlayers];   //holds the players time since last kill
    static private int[] playerPositions = new int[] { 1, 2, 3, 4 };           //holds the players current playerPositions
    static private int[] playerDraws = new int[PlayerData.numOfPlayers];
    static private int numOfPlayers = PlayerData.numOfPlayers;

    public static void SortPlayers()
    {
        PlayerData.GetPlayers();   
        for (int i = 0; i < numOfPlayers; i++)
        {
            playerScores[i] = PlayerData.playerScores[i];
            //playerTSLKs[i] = ;
        }

        //for each player in the game
        for (int check = 0; check < numOfPlayers; check++)
        {
            for (int player = 0; player < numOfPlayers - check - 1; player++)
            {
                //Debug.Log("___________________________");
                //Debug.Log("Player" + player + " v Player" + (player + 1));
                //Debug.Log(playerScores[player] + " v " + playerScores[player + 1]);

                if (playerScores[player] < playerScores[player + 1])
                {
                    //if current player score is less than next player score, move player down podium
                    //Debug.Log(playerScores[player] + " is less than " + playerScores[player + 1]);

                    //swap scores
                    int tempScore = playerScores[player];
                    playerScores[player] = playerScores[player + 1];
                    playerScores[player + 1] = tempScore;

                    //swap timers
                    float tempTimer = playerTSLKs[player];
                    playerTSLKs[player] = playerTSLKs[player + 1];
                    playerTSLKs[player + 1] = tempTimer;

                    //swap positions
                    int tempPos = playerPositions[player];
                    playerPositions[player] = playerPositions[player + 1];
                    playerPositions[player + 1] = tempPos;
                }
                else if (playerScores[player] == playerScores[player + 1])
                {
                    //if current player score equal to next player score
                    //Debug.Log(playerScores[player] + " is equal to " + playerScores[player + 1]);

                    if (playerTSLKs[player] > playerTSLKs[player + 1])
                    {
                        //if current player time since last kill greater than next player TSLK, swap podiums
                        //Debug.Log(playerTSLKs[player] + " is greater than " + playerTSLKs[player + 1]);

                        //swap timers
                        float tempTimer = playerTSLKs[player];
                        playerTSLKs[player] = playerTSLKs[player + 1];
                        playerTSLKs[player + 1] = tempTimer;

                        //swap positions
                        int tempPos = playerPositions[player];
                        playerPositions[player] = playerPositions[player + 1];
                        playerPositions[player + 1] = tempPos;
                    }
                }
            }
        }
        Debug.Log("Player with most kills is Player" + playerPositions[0]);
    }


    public static int GetHighestPlayer(){ return playerPositions[0]; }
}
