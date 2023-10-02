using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LoadScreenManager : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public Image pressStart;
    public Image loading;
    private bool begun = false;
    private bool isLoading = true;
    private bool skipping = false;
    private bool pressedStart;
    [SerializeField] private float minLoadTime;
    private float timer;
    [SerializeField] private InputAction startAction;
    [SerializeField] private InputAction skipAction;
    [SerializeField] private GameObject boxAnimator;

    private AudioManager AM;



    private void Awake()
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("LSM Awake"); }
        startAction.performed += ctx => StartAction(ctx);
        startAction.Enable();

        skipAction.performed += ctx => SkipAction(ctx);
        skipAction.Enable();

        AM = FindObjectOfType<AudioManager>();

        if (PlayerData.GetDevMode()) { minLoadTime = 1; }

        AM.StopPlaying("PodiumMusic");
        pressStart.gameObject.SetActive(false);
        Invoke(nameof(Begin), transitionTime);
    }

    private void Begin()
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("LSM Begin"); }
        boxAnimator.GetComponent<BoxAnimator>().AnimateBoxes();
        StartCoroutine(LoadLevelASync(4));
        begun = true;
    }

    void Update()
    {
        if (begun)
        {
            if (isLoading)
            {
                loading.gameObject.SetActive(true);
            }
            else
            {
                loading.gameObject.SetActive(false);
                pressStart.gameObject.SetActive(true);
            }

            timer += Time.deltaTime;

            if (timer > minLoadTime)
            {
                isLoading = false;
            }
        }
    }
    void StartAction(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("LSM StartAction"); }
        if (!isLoading)
        {
            AM.Play("SelectBeep");
          //  AM.StopPlaying("StartMenuMusic");
            AM.StopPlaying("SoundTrees");
            AM.StopPlaying("SoundZaps");
            
             StartCoroutine(LoadLevel());
        }
    }

    private void SkipAction(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("LSM SkipAction"); }
        if (ctx.performed && !skipping && SceneManager.GetActiveScene().name == "3LoadingScreen") { minLoadTime = 2; skipping = true; boxAnimator.GetComponent<BoxAnimator>().SetWaitTime(0.5f); }
    }

    public void OnClick()
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("LSM OnClick"); }
        if (!isLoading)
        {
            AM.Play("SelectBeep");
            AM.StopPlaying("StartMenuMusic");
            AM.StopPlaying("SoundTrees");
            AM.StopPlaying("SoundZaps");
           
            StartCoroutine(LoadLevel());
        }
    }
    IEnumerator LoadLevelASync(int levelIndex)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("LSM LoadLevelASync"); }
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            if (timer > minLoadTime && pressedStart)
            { 
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
    IEnumerator LoadLevel()
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("LSM LoadLevel"); }
        startAction.Disable();
        skipAction.Disable();

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        pressedStart = true;
    }

}
