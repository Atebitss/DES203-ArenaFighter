using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTopTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Top Trigger");
        //Debug.Log(collision.gameObject.transform);
        //Debug.Log(collision.gameObject.transform.tag);
        if (collision.gameObject.transform.tag == "Player")
        {
            PlayerController colPC = collision.gameObject.transform.GetComponent<PlayerController>();
            colPC.TopTrigger();
        }
    }
}
