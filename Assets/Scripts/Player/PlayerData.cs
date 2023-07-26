using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour
{
    public static int numOfPlayers = 4;
    public static bool gameRun, devMode;


    //player in-game
    public static GameObject[] players = new GameObject[4];             //reference player game objects
    public static PlayerController[] playerScripts = new PlayerController[4]; //reference player control scripts

    //player constants
    public static PlayerInput[] playerInputs = new PlayerInput[4];      //reference player input components
    public static InputDevice[] playerDevices = new InputDevice[4];     //reference player devices
    public static string[] playerControlScheme = new string[4];         //holds player scheme (keyboard/xbox contoller)

    //player scores
    public static int[] playerScores = new int[] { 2, 4, 3, 1 };                      //holds players total kills
    public static float[] playerTSLKs = new float[4];                    //holds players time since last kill
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

        for(int position = 0; position < numOfPlayers; position++)
        {
            Debug.Log("Updating position " + position + ": " + playerScripts[playerPositions[position]].gameObject.name + ", score: " + playerScripts[playerPositions[position]].GetScore() + ", tslk: " + playerScripts[playerPositions[position]].GetTimeSinceLastKill());
            playerScores[position] = playerScripts[playerPositions[position]].GetScore();
            playerTSLKs[position] = playerScripts[playerPositions[position]].GetTimeSinceLastKill();
        }
    }

    public static void ResetScores()
    {
        for (int p = 0; p < playerScores.Length; p++)
        {
            playerScores[p] = 0;
        }
    }

    public static void SortPlayers()
    {
        bool swapped;

        //Debug.Log("____________");
        for(int check = 0; check < numOfPlayers; check++)
        {
            swapped = false;

            //Debug.Log("_____");
            //for (int i = 0; i < numOfPlayers; i++) { Debug.Log("BEFORE position" + i + ": player " + playerPositions[i] + " - score " + playerScores[i] + ", tslk " + playerTSLKs[i]); }
            //Debug.Log("_____");
            for (int position = 0; position < numOfPlayers - check - 1; position++)
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

                    int tempPosition = playerPositions[position];
                    playerPositions[position] = playerPositions[position + 1];
                    playerPositions[position + 1] = tempPosition;

                    swapped = true;
                }
                else if(playerScores[position] == playerScores[position + 1] && playerTSLKs[position] > playerTSLKs[position + 1])
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

                    int tempPosition = playerPositions[position];
                    playerPositions[position] = playerPositions[position + 1];
                    playerPositions[position + 1] = tempPosition;

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
        for (int position = 0; position < numOfPlayers; position++)
        {
            //Debug.Log("position " + position + ": " + playerScripts[playerPositions[position]].gameObject.name + ", score: " + playerScripts[playerPositions[position]].GetScore() + ", tslk: " + playerScripts[playerPositions[position]].GetTimeSinceLastKill());
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
            playerTSLKs[p] = 0;
        }
    }
}
