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
        debugText.text = "---THIS IS EMPTY DEBUG TEXT---";
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
                //+ "\n\njumpBufferCounter: " + playerScript.GetJumpBufferCounter()
                + "\n\nisDeflecting: " + playerScript.IsDeflecting()
                + "\n\ninvincibilityTimer: " + playerScript.GetInvincibilityTimer()
                + "\ntimeSinceLastKill: " + playerScript.GetTimeSinceLastKill();
        }
    }


    public void SetPlayer(GameObject newPlayer)
    {
        //Debug.Log("Setting new player - " + newPlayer.name);
        player = newPlayer;
        playerScript = player.GetComponent<PlayerController>();
    }
}
