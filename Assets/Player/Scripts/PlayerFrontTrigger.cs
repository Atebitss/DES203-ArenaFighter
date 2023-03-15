using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrontTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(this.transform.parent + " front trigger");
        PlayerController pc = this.transform.parent.gameObject.GetComponent<PlayerController>();
        pc.FrontTrigger(collision);
    }
}
