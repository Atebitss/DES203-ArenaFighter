using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LoadScreenManager : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    public Button pressStart;
    public Image loading;
    private bool isLoading = true;
    private bool pressedStart;
    [SerializeField] private float minLoadTime;
    private float timer;
    [SerializeField] private InputAction startAction;
    [SerializeField] private GameObject boxAnimator;

    private AudioManager AM;

    private void Awake()
    {
        startAction.performed += ctx => StartAction(ctx);
        startAction.Enable();

        AM = FindObjectOfType<AudioManager>();

        if (PlayerData.devMode) { minLoadTime = 1; }
    }
    public void Start()
    {
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
            AM.StopPlaying("StartMenuMusic");
            pressedStart = true;
        }
    }
    public void OnClick()
    {
        if (!isLoading)
        {
            AM.Play("SelectBeep");
            AM.StopPlaying("StartMenuMusic");
            pressedStart = true;
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
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        pressedStart = true;
    }

}
