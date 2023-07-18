using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PodiumScreenManager : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    [SerializeField ]private float minLoadTime;
    private float timer;

    public Image continueButton;

    [SerializeField] private InputAction continueAction;

    void Awake()
    {
        continueAction.performed += ctx => Continue(ctx);
        continueAction.Enable();
    }

    
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > minLoadTime)
        {
            continueButton.gameObject.SetActive(true);
        }
        else
        {
            continueButton.gameObject.SetActive(false);
        }
    }
    public void Continue(InputAction.CallbackContext ctx)
    {
        FindObjectOfType<AudioManager>().Play("SelectBeep");

        StartCoroutine(LoadLevel(6));
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
