using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTopTrigger : MonoBehaviour
{
    private string triggeringCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.transform);
        Debug.Log(collision.gameObject.transform.tag);
        if (collision.gameObject.transform.tag == "Player")
        {
            Debug.Log("Top Trigger");
            triggeringCollider = collision.gameObject.transform.name;
            collision.gameObject.transform.GetComponent<PlayerController>().TopTrigger();
        }
    }
}
