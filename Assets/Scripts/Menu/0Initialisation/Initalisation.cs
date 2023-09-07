using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initalisation : MonoBehaviour
{
    [Header("Developer Mode")]
    [SerializeField] private bool devMode;
    [SerializeField] private float devRoundSecs = 15f;

    [Header("Debug")]


    [Header("Sprite Refs")]
    //character sprite references
    [SerializeField] private Sprite[] characterSprites = new Sprite[5];      //holds possible sprites


    //instantly loads up the main menu scene, this scene exits to instantiate the audio manager
    void Start()
    {
        PlayerData.SetDevMode(devMode);
        PlayerData.SetDevRoundTime(devRoundSecs);
        PlayerData.SetSprites(characterSprites);

        SceneManager.LoadScene(1);
    }    
}
