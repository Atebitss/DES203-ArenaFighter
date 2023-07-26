using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initalisation : MonoBehaviour
{
    [SerializeField] private bool devMode;

   //instantly loads up the main menu scene, this scene exits to instantiate the audio manager and thats it
    void Start()
    {
        PlayerData.devMode = devMode;
        SceneManager.LoadScene(1);
    }    
}
