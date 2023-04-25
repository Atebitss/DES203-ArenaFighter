using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

private void Start()
{
FindObjectOfType<AudioManager>().Play("StartMenuMusic");
}

    public void PlayGame()
    {

     FindObjectOfType<AudioManager>().Play("SelectBeep");
  
     FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");

        SceneManager.LoadScene(2);
    }

    public void QuitGame() 
    {

        FindObjectOfType<AudioManager>().Play("SelectBeep");
  
        FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");

        Application.Quit();

        print("Quit");
    }


}
