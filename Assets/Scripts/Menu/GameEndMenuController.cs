using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndMenuController : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("MusicMenu");
    }

    public void Replay()
    {

        FindObjectOfType<AudioManager>().Play("SelectBeep");

        FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");

        SceneManager.LoadScene(3);
    }

    public void QuitToMenu()
    {

        FindObjectOfType<AudioManager>().Play("SelectBeep");

        FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");

        SceneManager.LoadScene(0);

      
    }


}
