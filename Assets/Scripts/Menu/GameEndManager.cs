using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameEndManager : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    private AudioManager AM;

    [SerializeField] private InputAction continueAction;

    void Awake()
    {
        continueAction.performed += ctx => Continue(ctx);
        continueAction.Enable();
        AM = FindObjectOfType<AudioManager>();
    }


    public void Continue(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "6GameEnd" && this != null)
        {
            AM.Play("SelectBeep");
            StartCoroutine(LoadLevel(1));
        }
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
