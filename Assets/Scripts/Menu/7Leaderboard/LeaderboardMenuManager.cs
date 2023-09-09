using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LeaderboardMenuManager : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    [SerializeField] private InputAction continueAction;

    void Awake()
    {
        continueAction.performed += ctx => Continue(ctx);
        continueAction.Enable();
    }


    public void Continue(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "7Leaderboard" && this != null)
        {
            FindObjectOfType<AudioManager>().Play("SelectBeep");
            FindObjectOfType<AudioManager>().StopPlaying("TitleScreenMusic");
            StartCoroutine(LoadLevel(1));
        }
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        continueAction.Disable();
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
