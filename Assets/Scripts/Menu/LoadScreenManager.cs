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

    private void Awake()
    {
        startAction.performed += ctx => StartAction(ctx);
        startAction.Enable();

        if(PlayerData.devMode) { minLoadTime = 1; }
    }
    public void Start()
    {
        //FindObjectOfType<AudioManager>().Play("SelectBeep");
        //FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");

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
            FindObjectOfType<AudioManager>().Play("SelectBeep");
            FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");
            pressedStart = true;
        }
    }
    public void OnClick()
    {
        if (!isLoading)
        {
            FindObjectOfType<AudioManager>().Play("SelectBeep");
            FindObjectOfType<AudioManager>().StopPlaying("StartMenuMusic");
            pressedStart = true;
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
  
}
