using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardHandler : MonoBehaviour
{
    [SerializeField] private Sprite[] playerSprites = new Sprite[4];
    private int[] playerScores = new int[PlayerData.numOfPlayers];      //hold the players current playerScores
    private float[] playerTSLKs = new float[PlayerData.numOfPlayers];   //holds the players time since last kill
    private int[] playerPositions = new int[] { 1, 2, 3, 4 };           //holds the players current playerPositions
    private int[] playerDraws = new int[PlayerData.numOfPlayers];
    private int numOfPlayers = PlayerData.numOfPlayers;
    private GameObject[] podiumImage = new GameObject[4];
    private GameObject[] podiumText = new GameObject[4];


    void Awake()
    {
        //Debug.Log("ScoreboardHandler Awake()");
        //assign variables
        for (int i = 0; i < numOfPlayers; i++)
        {
            playerScores[i] = PlayerData.playerScores[i];
            playerTSLKs[i] = PlayerData.timeSinceLastKill[i];
            //Debug.Log("Position " + (i+1) + ": Player " + playerPositions[i] + " with score " + playerScores[i] + " \tTSLK: " + playerTSLKs[i]);
        }


        for (int playerPodPos = 0; playerPodPos < 4; playerPodPos++)
        {
            string imgRef = "Image" + (playerPodPos + 1);
            podiumImage[playerPodPos] = GameObject.Find(imgRef);
            podiumImage[playerPodPos].SetActive(false);

            string textRef = "Text" + (playerPodPos + 1);
            podiumText[playerPodPos] = GameObject.Find(textRef);
            podiumText[playerPodPos].SetActive(false);
        }


        SortPlayers();
        DisplayPlayers();
        PlayerData.ResetScores();
    }



    private void SortPlayers()
    {
        //for each player in the game
        for(int check = 0; check < numOfPlayers; check++)
        {
            for(int player = 0; player < numOfPlayers - check - 1; player++)
            {
                //Debug.Log("___________________________");
                //Debug.Log("Player" + player + " v Player" + (player + 1));
                //Debug.Log(playerScores[player] + " v " + playerScores[player + 1]);

                if(playerScores[player] < playerScores[player+1])
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
                else if(playerScores[player] == playerScores[player + 1])
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

        for (int i = 0; i < numOfPlayers; i++)
        {
            Debug.Log("Position " + (i+1) + ": Player " + playerPositions[i] + " with score " + playerScores[i] + " \tTSLK: " + playerTSLKs[i]);
        }
    }


    private void DisplayPlayers()
    {
        //display winners
        //for each player, set the image in the relevant podium space to the character image
        //then raise the character for each of their kills and add a new podium block below them
        for (int playerPodPos = 0; playerPodPos < numOfPlayers; playerPodPos++)
        {
            //reference relevant game object associated with scoreboard position i and update image with player sprite in position i 
            string imgRef = "Image" + (playerPodPos + 1);
            podiumImage[playerPodPos].SetActive(true);
            podiumImage[playerPodPos].GetComponent<ChangeImage>().ImageChange(playerSprites[playerPositions[playerPodPos]-1]);
            Debug.Log("player " + playerPositions[playerPodPos] + " in position " + (playerPodPos + 1));
            Debug.Log(playerSprites[playerPositions[playerPodPos] - 1]);

            //update text
            //reference relevant game object associated with scoreboard position i and update text with the number of kills
            string textRef = "Text" + (playerPodPos + 1);
            switch (playerPodPos)
            {
                case 0:
                    podiumText[playerPodPos].SetActive(true);
                    podiumText[playerPodPos].GetComponent<TMPro.TextMeshProUGUI>().text = "1ST\nKILLS: " + playerScores[playerPodPos];
                    break;
                case 1:
                    podiumText[playerPodPos].SetActive(true);
                    podiumText[playerPodPos].GetComponent<TMPro.TextMeshProUGUI>().text = "2ND\nKILLS: " + playerScores[playerPodPos];
                    break;
                case 2:
                    podiumText[playerPodPos].SetActive(true);
                    podiumText[playerPodPos].GetComponent<TMPro.TextMeshProUGUI>().text = "3RD\nKILLS: " + playerScores[playerPodPos];
                    break;
                case 3:
                    podiumText[playerPodPos].SetActive(true);
                    podiumText[playerPodPos].GetComponent<TMPro.TextMeshProUGUI>().text = "4TH\nKILLS: " + playerScores[playerPodPos];
                    break;
            }
        }

    }
}
