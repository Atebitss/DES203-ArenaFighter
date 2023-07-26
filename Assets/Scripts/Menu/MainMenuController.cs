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

    void Awake()
    {
        playAction.performed += ctx => Play(ctx);
        playAction.Enable();

        AM = FindObjectOfType<AudioManager>();
        AM.StopPlaying("MusicFight");
    }

    private void Start()
    {
        AM.Play("StartMenuMusic");
        PlayerData.ResetStats();
    }



    public void PlayGame()
    {
        AM.PlayOneShot("SelectBeep");
        StartCoroutine(LoadLevel(2));
    }

    public void Play(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "1MainMenu" && this != null)
        {
            PlayGame();
        }
    }


    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);

    }
}
