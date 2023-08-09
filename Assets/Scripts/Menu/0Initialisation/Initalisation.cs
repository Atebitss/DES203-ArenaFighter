using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initalisation : MonoBehaviour
{
    [SerializeField] private bool devMode;
    [SerializeField] private float devRoundTime = 1;

    //instantly loads up the main menu scene, this scene exits to instantiate the audio manager
    void Start()
    {
        PlayerData.SetDevMode(devMode);
        PlayerData.SetDevRoundTime(devRoundTime);

        SceneManager.LoadScene(1);
    }    
}
