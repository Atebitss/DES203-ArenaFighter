using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    private AudioManager AM;

    void Awake() { AM = FindObjectOfType<AudioManager>(); }


    public void MainMenu()
    {
        if (SceneManager.GetActiveScene().name == "6GameEnd" || SceneManager.GetActiveScene().name == "7Leaderboard")
        {
            AM.Play("SelectBeep");
            StartCoroutine(LoadLevel(1));
        }
    }

    public void Replay()
    {
        if (SceneManager.GetActiveScene().name == "6GameEnd")
        {
            AM.Play("SelectBeep");
            //if (AM.IsSoundPlaying("FightMusic")) { AM.StopPlaying("FightMusic"); }
            StartCoroutine(LoadLevel(3)); //can be changed to 4 to go straight into the game
        }
    }


    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
