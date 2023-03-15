using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DebugUIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    private GameObject player;
    private PlayerController playerScript;

    void Start()
    {
        //ex. debugText.text = "---THIS IS EMPTY DEBUG TEXT---";
        debugText.text = "X: \nY: \nVelocity:";
    }


    void Update()
    {
        if (playerScript != null)
        {
            debugText.text =
                    player.name
                + "\nX: " + playerScript.GetPlayerX().ToString("F2")
                + "\tY: " + playerScript.GetPlayerY().ToString("F2")
                + "\nVelocity X: " + playerScript.GetPlayerXVelocity().ToString("F2")
                + "\tVelocity Y: " + playerScript.GetPlayerYVelocity().ToString("F2")
                + "\n\nisGrounded: " + playerScript.IsGrounded()
                + "\nisOnIce: " + playerScript.IsOnIce()
                + "\nisOnBouncy: " + playerScript.IsOnBouncy()
                + "\nisOnStickyWall: " + playerScript.OnStickyWall()
                + "\n\nisDeflecting: " + playerScript.IsDeflecting();
        }
    }


    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
        playerScript = player.GetComponent<PlayerController>();
    }
}
