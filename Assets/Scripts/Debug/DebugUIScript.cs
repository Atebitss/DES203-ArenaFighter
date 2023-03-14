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
                + "\nisGrounded: " + playerScript.GetOnGround()
                + "\nisOnIce: " + playerScript.GetOnIce()
                + "\nisOnBouncy: " + playerScript.GetOnBouncy()
                + "\nisOnStickyWall: " + playerScript.GetOnStickyWall();
        }
    }


    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
        playerScript = newPlayer.GetComponent<PlayerController>();
    }
}
