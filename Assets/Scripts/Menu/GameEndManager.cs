using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameEndManager : MonoBehaviour
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
        FindObjectOfType<AudioManager>().Play("SelectBeep");

        StartCoroutine(LoadLevel(1));
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
