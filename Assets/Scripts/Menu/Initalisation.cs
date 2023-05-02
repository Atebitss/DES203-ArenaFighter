using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initalisation : MonoBehaviour
{
   //instantly loads up the main menu scene, this scene exits to instantiate the audio manager and thats it
    void Start()
    {
        SceneManager.LoadScene(1);
    }    
}
