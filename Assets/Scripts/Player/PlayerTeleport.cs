using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour

//Code adapted from https://www.youtube.com/watch?v=0JXVT28KCIg

{
    private GameObject currentTeleporter;
    
    private bool canTeleport = true;

    private Rigidbody2D body;

    private float collisionForce;
    

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
       
    }

    //gets which teleporter the player just entered
    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.CompareTag("Teleporter"))
        {
            if (canTeleport == true)
            {
                collisionForce = body.velocity.magnitude;

                currentTeleporter = collision.gameObject;
                teleport();
                Debug.Log("Teleporting");
            }

        }
    }
    //deassigns the current teleporter as the player exits it
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            if (currentTeleporter == collision.gameObject)
            {
                currentTeleporter = null;

            }
        }
    }
    //teleports player to the current teleporters destination
    private void teleport()
    {
       
        transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;

        Quaternion destinationRotation = currentTeleporter.GetComponent<Teleporter>().GetDestination().rotation;
        Quaternion currentRotation = currentTeleporter.GetComponent<Transform>().rotation;

        if (destinationRotation != currentRotation)
        {
            print("rotation is different, adding force");
            //will need to fix later
            //body.AddForce(new Vector2((collisionForce * 2000), currentTeleporter.transform.right.magnitude));
           



        }


        StartCoroutine(teleportdelay());

    }

    //delays telportation for 0.1 seconds so the player isnt endlessly telporting
    IEnumerator teleportdelay()
    {
        canTeleport = false;
        yield return new WaitForSeconds(0.1f);
        canTeleport = true;

    }
}

