using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStunTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.transform);
        //Debug.Log(collision.gameObject.transform.tag);
       // if (collision.gameObject.transform.tag == "Player")
       // {
       //     PlayerController colPC = collision.gameObject.transform.GetComponent<PlayerController>();
       //     float colYVel = colPC.GetPlayerYVelocity();
       //
       //     if (PlayerData.GetDevMode())
       //     {
       //         Debug.Log("YVel of " + collision.gameObject.name + ": " + colYVel);
       //     }
       //
       //     //if collider is player and their current falling velocity is below x, stun the player being landed on
       //     if (colYVel <= -30f)
       //     {
       //         this.gameObject.transform.parent.GetComponent<PlayerController>().Stun();
       //     }
       // } 
    }
}