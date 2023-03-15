using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBackTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(this.transform.parent + " back trigger");
        PlayerController pc = this.transform.parent.gameObject.GetComponent<PlayerController>();
        pc.BackTrigger(collision);
    }
}
