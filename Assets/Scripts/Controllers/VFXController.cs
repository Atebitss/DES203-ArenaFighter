using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VFXController : MonoBehaviour
{
    public VFX[] visualEffects;
    public LevelScript levelController;

    public void PlayVFX(Transform location, string name)
    {
        VFX visualEffect = Array.Find(visualEffects, vfx => vfx.name == name);

        Instantiate(visualEffect.effect, location.position, Quaternion.identity);
    }
    public void PlayVFXwithDirection(Transform location, string name, int directionMult) //effects that are dependent on direction 
    {
        VFX visualEffect = Array.Find(visualEffects, vfx => vfx.name == name);

        GameObject newEffect = Instantiate(visualEffect.effect, location.position, Quaternion.identity);

        Vector3 effectScale = newEffect.transform.localScale;
        newEffect.transform.localScale = new Vector3(effectScale.x * directionMult, effectScale.y, effectScale.z);


    }
    public void PlayPlayerVFX(int playerNum, string name) //effects that track with the player
    {

        VFX visualEffect = Array.Find(visualEffects, vfx => vfx.name == name);

        Transform playerTransform = PlayerData.players[playerNum].transform; //gets players position

        GameObject newEffect = Instantiate(visualEffect.effect, playerTransform.position, Quaternion.identity, playerTransform);

        Vector3 particleScale = newEffect.transform.localScale;
        if (playerTransform.localScale.x < 0)
        { //invert particle direction if player is facing left
            {
                newEffect.transform.localScale = new Vector3(-particleScale.x, particleScale.y, particleScale.z);
            }


        }
    }
}
       
       


   


