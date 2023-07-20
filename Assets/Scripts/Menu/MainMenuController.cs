using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;


    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("StartMenuMusic");
        FindObjectOfType<AudioManager>().StopPlaying("MusicFight");

        PlayerData.ResetStats();
    }

    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().Play("SelectBeep");
        FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");

        StartCoroutine(LoadLevel(2));
    }

    public void QuitGame() 
    {

        FindObjectOfType<AudioManager>().Play("SelectBeep");
        FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");

        Application.Quit();

        print("Quit");
    }

    public void OptionsMenu()
    {
        Debug.Log("Options menu called");
        FindObjectOfType<AudioManager>().Play("SelectBeep");
        FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");

        //SceneManager.LoadScene(x);
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
