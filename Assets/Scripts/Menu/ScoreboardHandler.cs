using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreboardHandler : MonoBehaviour
{
    [SerializeField] private Sprite[] playerSprites = new Sprite[4];
    [SerializeField] private GameObject[] playerPositions = new GameObject[4];
    [SerializeField] private GameObject[] podiums = new GameObject[4];
    private GameObject[] podiumImage = new GameObject[4];
    private TextMeshProUGUI[] podiumText = new TextMeshProUGUI[4];


    void Awake()
    {
        PlayerData.SortPlayers();
        //Debug.Log("ScoreboardHandler Awake()");
        //Debug.Log(numOfPlayers);
        /*for (int i = 0; i < numOfPlayers; i++)
        {
            Debug.Log("Position " + (i+1) + ": Player " + playerPositions[i] + " with score " + playerScores[i] + "   TSLK: " + playerTSLKs[i]);
            Debug.Log(playerSprites[playerPositions[i] - 1]);
        }
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~");*/


        for (int playerPodPos = 0; playerPodPos < 4; playerPodPos++)
        {
            Debug.Log(PlayerData.playerPositions[playerPodPos]);
            Debug.Log(playerPositions[PlayerData.playerPositions[playerPodPos]]);
            Vector2 podiumLocation = playerPositions[PlayerData.playerPositions[playerPodPos]].transform.position;
            Debug.Log(podiumLocation);

            /*string imgRef = "Image" + (playerPodPos + 1);
            podiumImage[playerPodPos] = GameObject.Find(imgRef);

            string textRef = "Text" + (playerPodPos + 1);
            podiumText[playerPodPos] = GameObject.Find(textRef).GetComponent<TMPro.TextMeshProUGUI>();*/
        }

        DisplayPlayers();
        PlayerData.ResetScores();
    }



    private void DisplayPlayers()
    {
        //display winners
        //for each player, set the image in the relevant podium space to the character image
        //then raise the character for each of their kills and add a new podium block below them
        for (int playerPodPos = 0; playerPodPos < 3; playerPodPos++)
        {
            //reference relevant game object associated with scoreboard position i and update image with player sprite in position i 
            //Debug.Log("Position " + (playerPodPos + 1) + ": Player " + PlayerData.playerPositions[playerPodPos] + " with score " + PlayerData.playerScores[playerPodPos] + "   TSLK: " + PlayerData.playerTSLKs[playerPodPos]);
            //Debug.Log(playerSprites[PlayerData.playerPositions[playerPodPos]]);

            string imgRef = "Image" + (playerPodPos + 1);
            podiumImage[playerPodPos].GetComponent<ChangeImage>().ImageChange(playerSprites[PlayerData.playerPositions[playerPodPos]]);

            //update text
            //reference relevant game object associated with scoreboard position i and update text with the number of kills
            string textRef = "Text" + (playerPodPos + 1);
            podiumText[playerPodPos].color = new Color32(255, 0, 0, 255);
            switch (playerPodPos)
            {
                case 0:
                    podiumText[playerPodPos].text = "KILLS: " + PlayerData.playerScores[playerPodPos];
                    break;
                case 1:
                    podiumText[playerPodPos].text = "KILLS: " + PlayerData.playerScores[playerPodPos];
                    break;
                case 2:
                    podiumText[playerPodPos].text = "KILLS: " + PlayerData.playerScores[playerPodPos];
                    break;
                case 3:
                    podiumText[playerPodPos].text = "KILLS: " + PlayerData.playerScores[playerPodPos];
                    break;
            }
        }

    }
}
