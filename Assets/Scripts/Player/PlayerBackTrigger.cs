using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBackTrigger : MonoBehaviour
{
    private string triggeringCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(this.transform.parent + " back triggered by " + collision.gameObject.transform.name);
        triggeringCollider = collision.gameObject.transform.name;
    }

    public string GetTriggeringCollider()
    {
        return triggeringCollider;
    }
}
