using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour
{
    public static int numOfPlayers = 0;
    public static bool gameRun, devMode;


    //player in-game
    public static GameObject[] players = new GameObject[4];             //reference player game objects
    public static PlayerController[] playerScripts = new PlayerController[4]; //reference player control scripts

    //player constants
    public static PlayerInput[] playerInputs = new PlayerInput[4];      //reference player input components
    public static InputDevice[] playerDevices = new InputDevice[4];     //reference player devices
    public static string[] playerControlScheme = new string[4];         //holds player scheme (keyboard/xbox contoller)

    //player scores
    public static int[] playerScores = new int[4];                      //holds players total kills
    public static float[] playerTSLK = new float[4];             //holds players time since last kill
    public static int[] playerPositions = new int[] { 1, 2, 3, 4 };     //holds players current podium position



    public static void SetPlayers(GameObject player, int playerNum, PlayerController playerScript)
    {
        players[playerNum] = player;
        playerScripts[playerNum] = playerScript;
    }

    public static void GetPlayers()
    {
        Debug.Log(numOfPlayers + " total players");

        for(int i = 0; i < numOfPlayers; i++)
        {
            Debug.Log("Name: " + playerInputs[i]);
            Debug.Log("Player: " + players[i]);
            Debug.Log("Device: " + playerDevices[i]);
            Debug.Log("Scheme: " + playerControlScheme[i]);
        }
    }



    public static void UpdateScores()
    {
        //Debug.Log("Updating scores in PlayerData");

        for(int player = 0; player < numOfPlayers; player++)
        {
            playerScores[player] = playerScripts[player].GetScore();
            playerTSLK[player] = playerScripts[player].GetTimeSinceLastKill();
        }
    }

    public static void ResetScores()
    {
        for (int p = 0; p < playerScores.Length; p++)
        {
            playerScores[p] = 0;
        }
    }

    public static void SortScores()
    {
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
                    float tempTimer = playerTSLK[player];
                    playerTSLK[player] = playerTSLK[player + 1];
                    playerTSLK[player + 1] = tempTimer;

                    //swap positions
                    int tempPos = playerPositions[player];
                    playerPositions[player] = playerPositions[player + 1];
                    playerPositions[player + 1] = tempPos;
                }
                else if (playerScores[player] == playerScores[player + 1])
                {
                    //if current player score equal to next player score
                    //Debug.Log(playerScores[player] + " is equal to " + playerScores[player + 1]);

                    if (playerTSLK[player] > playerTSLK[player + 1])
                    {
                        //if current player time since last kill greater than next player TSLK, swap podiums
                        //Debug.Log(playerTSLK[player] + " is greater than " + playerTSLK[player + 1]);

                        //swap timers
                        float tempTimer = playerTSLK[player];
                        playerTSLK[player] = playerTSLK[player + 1];
                        playerTSLK[player + 1] = tempTimer;

                        //swap positions
                        int tempPos = playerPositions[player];
                        playerPositions[player] = playerPositions[player + 1];
                        playerPositions[player + 1] = tempPos;
                    }
                }
            }
        }

        for (int i = 0; i < numOfPlayers; i++)
        {
            //Debug.Log("Position " + i + ": Player " + playerPositions[i] + " with score " + playerScores[i]);
        }
    }


    
    public static void ResetStats()
    {
        numOfPlayers = 0;

        for (int p = 0; p < playerScores.Length; p++)
        {
            playerInputs[p] = null;
            playerDevices[p] = null;
            playerControlScheme[p] = null;
            playerTSLK[p] = 0;
        }
    }
}
