using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public float screenSaverTime = 5;
    private float timer;
    private AudioManager AM;

    public static MainMenuController instance = null;


    void Awake()
    {
        //Debug.Log("MMC Awake");
        if (instance == null)
        {
            //Debug.Log("instance was null");
            instance = this;
        }
        else if (instance != null)
        {
            //Debug.Log("instance was not null");
            Destroy(gameObject);
            instance = this;
        }


        AM = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        if (!AM.IsSoundPlaying("TitleScreenMusic")) { AM.Play("TitleScreenMusic"); }
        AM.Play("SoundTrees");
        AM.Play("SoundZaps");
        AM.StopPlaying("MusicFight");
        AM.StopPlaying("PodiumMusic");

        PlayerData.ResetStats();
        timer=0;
    }
    private void Update()
    {
        if (timer < screenSaverTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Screensaver();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void Screensaver()
    {
        StartCoroutine(LoadLevel(8));
    }
    public void PlayGame()
    {
        AM.Play("SelectBeep");
        StartCoroutine(LoadLevel(2));
    }
    public void Leaderboard()
    {
        FindObjectOfType<AudioManager>().Play("SelectBeep");
        StartCoroutine(LoadLevel(7));
    }
    

    public void QuitGame() 
    {

        AM.Play("SelectBeep");
        AM.StopPlaying("StartMenuMusic");
        AM.StopPlaying("SoundTrees");
        AM.StopPlaying("SoundZaps");

        Application.Quit();

        print("Quit");
    }

    public void OptionsMenu()
    {
        //Debug.Log("Options menu called");
        AM.Play("SelectBeep");
        AM.StopPlaying("StartMenuMusic");
        AM.StopPlaying("SoundTrees");
        AM.StopPlaying("SoundZaps");

        //SceneManager.LoadScene(x);
    }


    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
       // AM.VolumeFadeOut("TitleScreenMusic");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
