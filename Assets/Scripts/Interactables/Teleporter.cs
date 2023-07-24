using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
//code orignally from https://www.youtube.com/watch?v=0JXVT28KCIg
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

        return null;
    }
}
