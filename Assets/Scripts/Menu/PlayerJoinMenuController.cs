using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerJoinMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pjm;

    public void StartGame()
    {
        //Debug.Log("Starting game");
        //PlayerData.GetPlayers();
        FindObjectOfType<AudioManager>().Play("SelectBeep");

        FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");

        SceneManager.LoadScene(4);
    }
}
