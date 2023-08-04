using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndMenuController : MonoBehaviour
{
    private float timer;
    [SerializeField] private float gameEndCooldown = 10;
    [SerializeField] private GameObject replayButton;
    private bool canReplay = false;
    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("MusicMenu");
        FindObjectOfType<AudioManager>().StopPlaying("MusicFight");

    }
    private void FixedUpdate()
    {
        if (timer >= gameEndCooldown)
        {
            canReplay = true;
            replayButton.SetActive(true);
        }
        else
        {
            canReplay = false;
            timer += Time.deltaTime;
            replayButton.SetActive(false);
        }

    }

    

    public void Replay()
    {
        if (canReplay)
        {
            timer = 0;
            FindObjectOfType<AudioManager>().Play("SelectBeep");
            FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");
            SceneManager.LoadScene(6);
        }
        
       
    }

    public void QuitToMenu()
    {
        FindObjectOfType<AudioManager>().Play("SelectBeep");
        FindObjectOfType<AudioManager>().StopPlaying("MusicMenu");
        PlayerData.ResetStats();
        SceneManager.LoadScene(1);      
    }
}