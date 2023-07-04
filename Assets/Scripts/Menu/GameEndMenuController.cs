using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndMenuController : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("MusicMenu");
        FindObjectOfType<AudioManager>().StopPlaying("MusicFight");
    }

    public void Replay()
    {
        FindObjectOfType<AudioManager>().Play("SelectBeep");
        FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");
        SceneManager.LoadScene(6);
    }

    public void QuitToMenu()
    {
        FindObjectOfType<AudioManager>().Play("SelectBeep");
        FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");
        PlayerData.ResetStats();
        SceneManager.LoadScene(1);      
    }
}