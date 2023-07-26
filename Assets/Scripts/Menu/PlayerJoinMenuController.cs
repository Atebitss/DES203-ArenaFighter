using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerJoinMenuController : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public Button startButton;

    private AudioManager AM;

    void Awake()
    {
        AM = FindObjectOfType<AudioManager>();
    }

    public void Update()
    {
        if (PlayerData.numOfPlayers >= 2)
        {
            startButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }
    }


    public void StartGame()
    {
        AM.Play("SelectBeep");
        StartCoroutine(LoadLevel(3));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);

    }
}
