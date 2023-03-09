using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DebugUIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    private PlayerController playerScript;
    private GameObject activePlayer;

    void Start()
    {
        //ex. debugText.text = "---THIS IS EMPTY DEBUG TEXT---";
        debugText.text = "X: \nY: \nVelocity:";
    }


    void Update()
    {
        activePlayer = GameObject.FindWithTag("Player");

        if (activePlayer != null)
        {
            playerScript = activePlayer.GetComponent<PlayerController>();
        }

        if (playerScript != null)
        {
            debugText.text =
                  "X: " + playerScript.GetPlayerX().ToString("F2")
                + "\tY: " + playerScript.GetPlayerY().ToString("F2")
                + "\nVelocity X: " + playerScript.GetPlayerXVelocity().ToString("F2")
                + "\tVelocity Y: " + playerScript.GetPlayerYVelocity().ToString("F2")
                + "\nisGrounded: " + playerScript.IsOnGround()
                + "\nisOnIce: " + playerScript.IsOnIce()
                + "\nisOnBouncy: " + playerScript.IsOnBouncy()
                + "\nisOnStickyWall: " + playerScript.IsOnStickyWall();
        }
    }
}
