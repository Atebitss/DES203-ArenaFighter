using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

private void Start()
{
FindObjectOfType<AudioManager>().Play("MusicMenu");
}

    public void PlayGame()
    {

     FindObjectOfType<AudioManager>().Play("SelectBeep");
  
    FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");

        SceneManager.LoadScene(1);
    }

    public void QuitGame() 
    {

     FindObjectOfType<AudioManager>().Play("SelectBeep");
  
    FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");

        Application.Quit();
    }


}
