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


        for (int playerCheck = 0; playerCheck < PlayerData.numOfPlayers; playerCheck++)
        {
            Debug.Log("podium: " + playerCheck + ", player: " + (PlayerData.playerPositions[playerCheck]+1) + ", position: " + playerPositions[PlayerData.playerPositions[playerCheck]]);
            Vector2 podiumLocation = playerPositions[PlayerData.playerPositions[playerCheck]].transform.position;

            Debug.Log(playerPositions[PlayerData.playerPositions[playerCheck]]);
            Debug.Log(podiums[playerCheck]);
            GameObject refPodium = Instantiate(podiums[playerCheck], podiumLocation, Quaternion.identity);
           // refPodium.transform.
                //GetComponent<Animator>().SetInteger("PlayerNum", playerCheck);
            refPodium.transform.SetParent(playerPositions[PlayerData.playerPositions[playerCheck]].transform, false);
        }

        for(int playerCheck = 0; playerCheck < PlayerData.numOfPlayers; playerCheck++)
        {
            string imgRef = "Image" + (playerCheck + 1);
            podiumImage[playerCheck] = GameObject.Find(imgRef);

            string textRef = "Text" + (playerCheck + 1);
            podiumText[playerCheck] = GameObject.Find(textRef).GetComponent<TMPro.TextMeshProUGUI>();
        }

        DisplayPlayers();
        PlayerData.ResetScores();
    }



    private void DisplayPlayers()
    {
        //display winners
        //for each player, set the image in the relevant podium space to the character image
        //then raise the character for each of their kills and add a new podium block below them
        for (int playerCheck = 0; playerCheck < PlayerData.numOfPlayers; playerCheck++)
        {
            //reference relevant game object associated with scoreboard position i and update image with player sprite in position i 
            //Debug.Log("Position " + (playerCheck + 1) + ": Player " + PlayerData.playerPositions[playerCheck] + " with score " + PlayerData.playerScores[playerCheck] + "   TSLK: " + PlayerData.playerTSLKs[playerCheck]);
            //Debug.Log(playerSprites[PlayerData.playerPositions[playerCheck]]);

            string imgRef = "Image" + (playerCheck + 1);
            podiumImage[playerCheck].GetComponent<ChangeImage>().ImageChange(playerSprites[PlayerData.playerPositions[playerCheck]]);

            //update text
            //reference relevant game object associated with scoreboard position i and update text with the number of kills
            string textRef = "Text" + (playerCheck + 1);
            podiumText[playerCheck].color = new Color32(255, 0, 0, 255);
            switch (playerCheck)
            {
                case 0:
                    podiumText[playerCheck].text = "KILLS: " + PlayerData.playerScores[playerCheck];
                    break;
                case 1:
                    podiumText[playerCheck].text = "KILLS: " + PlayerData.playerScores[playerCheck];
                    break;
                case 2:
                    podiumText[playerCheck].text = "KILLS: " + PlayerData.playerScores[playerCheck];
                    break;
                case 3:
                    podiumText[playerCheck].text = "KILLS: " + PlayerData.playerScores[playerCheck];
                    break;
            }
        }

    }
}
