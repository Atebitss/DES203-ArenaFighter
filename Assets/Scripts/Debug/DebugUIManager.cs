using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUIManager : MonoBehaviour
{
    private GameObject[] playerDebug = new GameObject[4];
    private bool[] playerEnabled = new bool[4];

    void Awake()
    {
        for(int playerIndex = 0; playerIndex < playerDebug.Length; playerIndex++)
        {
            playerEnabled[playerIndex] = false;

            switch (playerIndex)
            {
                case 0:
                    playerDebug[playerIndex] = this.gameObject.transform.Find("PlayerOneDebugText").gameObject;
                    playerDebug[playerIndex].SetActive(false);
                    break;
                case 1:
                    playerDebug[playerIndex] = this.gameObject.transform.Find("PlayerTwoDebugText").gameObject;
                    playerDebug[playerIndex].SetActive(false);
                    break;
                case 2:
                    playerDebug[playerIndex] = this.gameObject.transform.Find("PlayerThreeDebugText").gameObject;
                    playerDebug[playerIndex].SetActive(false);
                    break;
                case 3:
                    playerDebug[playerIndex] = this.gameObject.transform.Find("PlayerFourDebugText").gameObject;
                    playerDebug[playerIndex].SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }


    public void EnablePlayer(int playerIndex, GameObject player)
    {
        Debug.Log("Enabling player " + playerIndex + " UI - " + playerDebug[playerIndex]);
        playerDebug[playerIndex].SetActive(true);
        playerDebug[playerIndex].GetComponent<DebugUIScript>().SetPlayer(player);
    }

    public void DisablePlayer(int playerIndex)
    {
        Debug.Log("Disabling player " + (playerIndex-1) + " UI - " + playerDebug[playerIndex]);
        playerDebug[playerIndex-1].SetActive(false);
    }
}
