using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initalisation : MonoBehaviour
{
    [SerializeField] private bool devMode;
    [SerializeField] private float devRoundTime = 1;

    //character sprite references
    [SerializeField] private Sprite[] characterSprites = new Sprite[4];      //holds possible sprites

    //instantly loads up the main menu scene, this scene exits to instantiate the audio manager
    void Start()
    {
        PlayerData.SetDevMode(devMode);
        PlayerData.SetDevRoundTime(devRoundTime);
        PlayerData.SetSprites(characterSprites);

        SceneManager.LoadScene(1);
    }    
}
