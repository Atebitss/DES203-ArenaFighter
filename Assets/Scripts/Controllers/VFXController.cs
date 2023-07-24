using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VFXController : MonoBehaviour
{
    public VFX[] visualEffects;
    public LevelScript levelController;
    private Transform playerTransform;
    private GameObject spawnedEffect;

    public void PlayVFX(Transform location, string name)
    {
        VFX visualEffect = Array.Find(visualEffects, vfx => vfx.name == name);

        Instantiate(visualEffect.effect, location.position, Quaternion.identity);
    }
    public void PlayPlayerVFX(int playerNum, string name)
    {
        VFX visualEffect = Array.Find(visualEffects, vfx => vfx.name == name);

        playerTransform = PlayerData.players[playerNum].transform; //gets players position
        spawnedEffect = Instantiate(visualEffect.effect, playerTransform.position, Quaternion.identity, playerTransform);

    }
    public void DeleteVFX(int playerNum, string name)
    {
        VFX visualEffect = Array.Find(visualEffects, vfx => vfx.name == name);
        playerTransform = PlayerData.players[playerNum].transform;
        if (spawnedEffect != null)
        {
            Destroy(spawnedEffect);
            //getting close but it only desyroys the last spawned effect, need to make an array and index each one or tmh idk
        }
     
    }

}
