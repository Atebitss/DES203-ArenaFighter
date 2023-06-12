using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJoinScript : MonoBehaviour
{
    private PlayerJoinHandler pjh;
    private int playerNum;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        pjh = GameObject.Find("PlayerJoinManager").GetComponent<PlayerJoinHandler>();
        playerNum = pjh.CurrentPlayer();
        gameObject.name = "PlayerJoin" + playerNum;
        //Debug.Log("New player awake, " + gameObject.name);
    }
}
