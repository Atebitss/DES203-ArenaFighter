using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    private AudioManager AM;

    [SerializeField] private InputAction playAction;

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

        playAction.performed += ctx => Play(ctx);
        playAction.Enable();

        AM = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        AM.Play("TitleScreenMusic");
        AM.Play("SoundTrees");
        AM.StopPlaying("MusicFight");

        PlayerData.ResetStats();
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

    public void Play(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "1MainMenu")
        {
            PlayGame();
        }
    }

    public void QuitGame() 
    {

        AM.Play("SelectBeep");
        AM.StopPlaying("StartMenuMusic");
        AM.StopPlaying("SoundTrees");
        AM.StopPlaying("SoundZaps");

        Application.Quit();

        //print("Quit");
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
        playAction.Disable();
        transition.SetTrigger("Start");
        AM.VolumeFadeOut("TitleScreenMusic");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
