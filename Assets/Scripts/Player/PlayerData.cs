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
    public static float[] playerTSLK = new float[4];                    //holds players time since last kill
    public static int[] playerPositions = new int[] { 0, 1, 2, 3 };     //holds players current podium position



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
            Debug.Log("Updating position " + player);
            Debug.Log("name: " + playerScripts[playerPositions[player]].gameObject.name + "   score: " + playerScripts[playerPositions[player]].GetScore() + "   tslk: " + playerScripts[playerPositions[player]].GetTimeSinceLastKill());
            playerScores[playerPositions[player]] = playerScripts[playerPositions[player]].GetScore();
            playerTSLK[playerPositions[player]] = playerScripts[playerPositions[player]].GetTimeSinceLastKill();
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
        //for each player in the game[player + 1]
        for (int check = 0; check < numOfPlayers; check++)
        {
            for (int position = 0; position < numOfPlayers - check - 1; position++)
            {
                Debug.Log("___________________________");
                Debug.Log("position " + position + " v position " + (position + 1));
                Debug.Log(playerScores[playerPositions[position]] + " v " + playerScores[playerPositions[position + 1]]);

                if (playerScores[playerPositions[position]] < playerScores[playerPositions[position + 1]])
                {
                    //if current position score is less than next position score, move position down podium
                    Debug.Log(playerScores[position] + " is less than " + playerScores[position + 1]);

                    //swap scores
                    int tempScore = playerScores[position];
                    playerScores[position] = playerScores[position + 1];
                    playerScores[position + 1] = tempScore;

                    //swap timers
                    float tempTimer = playerTSLK[position];
                    playerTSLK[position] = playerTSLK[position + 1];
                    playerTSLK[position + 1] = tempTimer;

                    //swap positions
                    int tempPos = playerPositions[position];
                    playerPositions[position] = playerPositions[position + 1];
                    playerPositions[position + 1] = tempPos;
                }
                else if (playerScores[playerPositions[position]] == playerScores[playerPositions[position + 1]])
                {
                    //if current position score equal to next position score
                    //Debug.Log(playerScores[playerPositions[position]] + " is equal to " + playerScores[playerPositions[position + 1]]);

                    if (playerTSLK[playerPositions[position]] > playerTSLK[playerPositions[position + 1]])
                    {
                        //if current position time since last kill greater than next position TSLK, swap podiums
                        Debug.Log(playerTSLK[position] + " is greater than " + playerTSLK[position + 1]);

                        //swap timers
                        float tempTimer = playerTSLK[position];
                        playerTSLK[position] = playerTSLK[position + 1];
                        playerTSLK[position + 1] = tempTimer;

                        //swap positions
                        int tempPos = playerPositions[position];
                        playerPositions[position] = playerPositions[position + 1];
                        playerPositions[position + 1] = tempPos;
                    }
                }
                else { Debug.Log(playerScores[playerPositions[position]] + " is the greatest score"); }
            }
        }

        for (int i = 0; i < numOfPlayers; i++)
        {
            Debug.Log("Position " + i + ": Player " + playerPositions[i] + " with score " + playerScores[i]);
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
