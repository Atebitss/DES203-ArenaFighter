using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTopTrigger : MonoBehaviour
{
    private string triggeringCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(this.transform.parent + " top triggered by " + collision.gameObject.transform.name);
        triggeringCollider = collision.gameObject.transform.name;

        if (collision.gameObject.transform.tag == "Player") { collision.gameObject.transform.GetComponent<PlayerController>().TopTrigger(); }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.transform.tag == "Player") { this.gameObject.transform.parent.GetComponent<PlayerController>().TopTrigger(false); }
    }

    public string GetTriggeringCollider()
    {
        return triggeringCollider;
    }
}
