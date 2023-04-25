using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerJoinMenuController : MonoBehaviour
{
    public void StartGame()
    {

        FindObjectOfType<AudioManager>().Play("SelectBeep");

        FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");

        SceneManager.LoadScene(3);
    }
}
