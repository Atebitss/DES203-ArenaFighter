using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] playerUI = new GameObject[4];
    [SerializeField] private Sprite[] headshotSprites = new Sprite[5];

    void Awake()
    {
        for (int playerIndex = 0; playerIndex < playerUI.Length; playerIndex++)
        {
            playerUI[playerIndex].SetActive(false);
        }
    }


    public void EnablePlayer(int playerIndex, GameObject player)
    {
        Debug.Log("Enabling player " + playerIndex + " UI - " + playerUI[playerIndex]);
        playerUI[playerIndex].SetActive(true);
        playerUI[playerIndex].GetComponent<PlayerUIScript>().SetPlayer(player, headshotSprites[PlayerData.GetSpriteID(PlayerData.playerPositions[playerIndex])]);

        string circleRef = "Player" + (playerIndex+1) + "Circle";
        Debug.Log(circleRef);
        Debug.Log(playerUI[playerIndex].transform.Find(circleRef));
        switch(playerIndex)
        {
            case 0:
                playerUI[playerIndex].transform.Find(circleRef).GetComponent<Image>().color = new Color(1f, 0f, 0f, 1f); //red
                break;
            case 1:
                playerUI[playerIndex].transform.Find(circleRef).GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f); //green
                break;
            case 2:
                playerUI[playerIndex].transform.Find(circleRef).GetComponent<Image>().color = new Color(1f, 0f, 1f, 1f); //pink
                break;
            case 3:
                playerUI[playerIndex].transform.Find(circleRef).GetComponent<Image>().color = new Color(0.5f, 1f, 0.75f, 1f); //teal
                break;
            case 4:
                playerUI[playerIndex].transform.Find(circleRef).GetComponent<Image>().color = new Color(0.75f, 0f, 1f, 1f); //purple
                break;
            default:
                playerUI[playerIndex].transform.Find(circleRef).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); //white, should never appear
                break;
        }
    }

    public void DisablePlayer(int playerIndex)
    {
        //Debug.Log("Disabling player " + playerIndex + " UI - " + playerUI[playerIndex]);
        playerUI[playerIndex].SetActive(false);
    }
}
