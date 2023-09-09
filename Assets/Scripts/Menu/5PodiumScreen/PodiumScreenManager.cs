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

    [SerializeField] private float minLoadTime;
    private float timer;

    public Image continueButton;

    [SerializeField] private InputAction continueAction;

    private AudioManager AM;

    void Awake()
    {
        continueAction.performed += ctx => Continue(ctx);
        continueAction.Enable();
        AM = FindObjectOfType<AudioManager>();
        AM.StopPlaying("MusicFight");
        AM.Play("PodiumMusic");
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
        if (SceneManager.GetActiveScene().name == "5PodiumScreen" && GameObject.Find("NameInput").GetComponent<SelectHandler>().IsConfirmed())
        {
            FindObjectOfType<AudioManager>().Play("SelectBeep");

            StartCoroutine(LoadLevel(6));
            continueAction.Disable();
        }
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

    }
}
