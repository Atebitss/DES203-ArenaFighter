using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
//code orignally from https://www.youtube.com/watch?v=0JXVT28KCIg
{

    [SerializeField] private Transform teleportingTo;

    public Transform GetDestination()
    {
        return teleportingTo;
    }
}
