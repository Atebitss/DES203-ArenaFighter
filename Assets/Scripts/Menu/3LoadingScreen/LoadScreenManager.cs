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
        startAction.performed += ctx => StartAction(ctx);
        startAction.Enable();

        skipAction.performed += ctx => SkipAction(ctx);
        skipAction.Enable();

        AM = FindObjectOfType<AudioManager>();

        if (PlayerData.GetDevMode()) { minLoadTime = 1; }
    }
    public void Start()
    {

    AM.StopPlaying("PodiumMusic");

        StartCoroutine(LoadLevelASync(4));
        boxAnimator.GetComponent<BoxAnimator>().AnimateBoxes();
    }
    public void Update()
    {
       
        if (isLoading)
        {
            loading.gameObject.SetActive(true);
            pressStart.gameObject.SetActive(false);
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
        //print(pressedStart);
    }
    void StartAction(InputAction.CallbackContext ctx)
    {
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
        //Debug.Log("Skip Action called");
        if(ctx.performed && !skipping && SceneManager.GetActiveScene().name == "3LoadingScreen") { minLoadTime = 2; skipping = true; boxAnimator.GetComponent<BoxAnimator>().SetWaitTime(0.5f); }
    }

    public void OnClick()
    {
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
        startAction.Disable();
        skipAction.Disable();

        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        pressedStart = true;
    }

}
