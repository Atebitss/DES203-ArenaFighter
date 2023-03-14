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
        playerDebug[playerIndex-1].SetActive(true);
        playerDebug[playerIndex-1].GetComponent<DebugUIScript>().SetPlayer(player);
    }
}
