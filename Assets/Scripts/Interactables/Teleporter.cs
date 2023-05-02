using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
//code adapted from https://www.youtube.com/watch?v=0JXVT28KCIg
{
    [SerializeField] private Transform[] canTeleportTo;
    
    public Transform GetDestination()
    {
        if (canTeleportTo != null)
        {
            int noOfPossibleDestinations = canTeleportTo.Length;

            int Destination = Random.Range(0, noOfPossibleDestinations );

            return canTeleportTo[Destination];
        }
        
        return transform; //if no values assigned, return own transform to stop null errors, should only fire if YOU BROKE SOMETHING 
    }
}
