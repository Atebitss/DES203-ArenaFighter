using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("awake");
    }

    //~~~~~~~OVERLAPPING~~~~~~~\\
    void OnCollisionEnter(Collision obj)
    {
        Debug.Log(obj);
    }
}
