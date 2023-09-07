using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [Header("Player Materials")]
    public Material fireKnight;
    public Material hornKnight;
    public Material blossomKnight;
    public Material copperKnight;
    public Material purpleKnight;
    [Header("Particle System")]
    public ParticleSystem particleSys;
    private int playerNum;

    private void Start()
    {
        playerNum = transform.parent.gameObject.GetComponent<PlayerController>().playerNum;

        switch (playerNum)
        {
            case 0:
                particleSys.GetComponent<Renderer>().material = fireKnight;
                break;
            case 1:
                particleSys.GetComponent<Renderer>().material = hornKnight;
                break;
            case 2:
                particleSys.GetComponent<Renderer>().material = blossomKnight;
                break;
            case 3:
                particleSys.GetComponent<Renderer>().material = copperKnight;
                break;
            case 4:
                particleSys.GetComponent<Renderer>().material = purpleKnight;
                break;

        }

    }
}
