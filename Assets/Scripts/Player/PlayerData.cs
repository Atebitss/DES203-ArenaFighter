using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerData : MonoBehaviour
{
    public static GameObject[] players = new GameObject[4];
    public static PlayerInput[] playerInputs = new PlayerInput[4];
    public static InputDevice[] playerDevices = new InputDevice[4];
    public static string[] playerControlScheme = new string[4];
    public static int[] playerScores = new int[4];// { 4, 4};
    public static float[] timeSinceLastKill = new float[4];// { 1.1f, 0.75f};
    public static int numOfPlayers = 0;
    public static bool gameRun, devMode;



    public static void SetPlayers(GameObject player, int playerNum)
    {
        players[playerNum] = player;
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


    public static void ResetScores()
    {
        for(int p = 0; p < playerScores.Length; p++)
        {
            playerScores[p] = 0;
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
            timeSinceLastKill[p] = 0;
        }
    }
}
