using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreboardHandler : MonoBehaviour
{
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


        for (int playerCheck = 0; playerCheck < PlayerData.GetNumOfPlayers(); playerCheck++)
        {
            //Debug.Log("podium: " + playerCheck + ", player: " + (PlayerData.playerPositions[playerCheck]+1) + ", position: " + playerPositions[PlayerData.playerPositions[playerCheck]]);
            Vector2 podiumLocation = playerPositions[PlayerData.playerPositions[playerCheck]].transform.position;
            Transform podiumTransform = playerPositions[PlayerData.playerPositions[playerCheck]].transform;

            //Debug.Log(playerPositions[PlayerData.playerPositions[playerCheck]]);
            //Debug.Log(podiums[playerCheck]);
            GameObject refPodium = Instantiate(podiums[playerCheck], Vector2.zero, Quaternion.identity,podiumTransform);
            refPodium.transform.SetParent(playerPositions[PlayerData.playerPositions[playerCheck]].transform, false);
            refPodium.transform.localPosition = Vector2.zero;
        }

        for(int playerCheck = 0; playerCheck < PlayerData.GetNumOfPlayers(); playerCheck++)
        {
            string imgRef = "Image" + (playerCheck + 1);
            podiumImage[playerCheck] = GameObject.Find(imgRef);

            string textRef = "Text" + (playerCheck + 1);
            podiumText[playerCheck] = GameObject.Find(textRef).GetComponent<TMPro.TextMeshProUGUI>();
        }

        DisplayPlayers();
    }



    private void DisplayPlayers()
    {
        //display winners
        //for each player, set the image in the relevant podium space to the character image
        //then raise the character for each of their kills and add a new podium block below them
        for (int playerCheck = 0; playerCheck < PlayerData.GetNumOfPlayers(); playerCheck++)
        {
            podiumImage[playerCheck].GetComponent<ChangeImage>().ImageChange(PlayerData.GetSprite(PlayerData.GetSpriteID(PlayerData.playerPositions[playerCheck])));

            //podiumText[playerCheck].color = new Color32(1, 1, 1, 1);
            podiumText[playerCheck].text = "KILLS " + PlayerData.playerScores[playerCheck];
        }

    }
}
