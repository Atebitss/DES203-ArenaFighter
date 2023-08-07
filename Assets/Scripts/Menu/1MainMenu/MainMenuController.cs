using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenuController : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

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
    }

    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("StartMenuMusic");
        FindObjectOfType<AudioManager>().StopPlaying("MusicFight");

        PlayerData.ResetStats();
    }

    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().Play("SelectBeep");
        StartCoroutine(LoadLevel(2));
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

        FindObjectOfType<AudioManager>().Play("SelectBeep");
        FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");

        Application.Quit();

        //print("Quit");
    }

    public void OptionsMenu()
    {
        //Debug.Log("Options menu called");
        FindObjectOfType<AudioManager>().Play("SelectBeep");
        FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");

        //SceneManager.LoadScene(x);
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        playAction.Disable();
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
