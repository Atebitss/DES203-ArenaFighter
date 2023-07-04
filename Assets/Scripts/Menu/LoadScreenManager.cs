using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScreenManager : MonoBehaviour
{
   [SerializeField] private GameObject loadingScreen;
   [SerializeField] private GameObject menuScreen;
    private bool buttonPressed = false;
   [SerializeField] private Slider loadingSlider;

    public void LoadLevel(int levelLoadNo) //disables menu UI and enables loading screen UI
    {
        menuScreen.SetActive(false);
        loadingScreen.SetActive(true);

       // StartCoroutine(LoadLevelASync(levelLoadNo));
    }

  //  IEnumerator LoadLevelASync(int levelLoadNo) //Starts loading next level asyncrousnously, updates loading bar sldier with the loading progress
  //  {
   //     AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelLoadNo);

   //     loadOperation.allowSceneActivation = false;
    //    while (!loadOperation.isDone)
      //  {
      //      float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
     //       loadingSlider.value = progressValue;
            
      //      yield return new WaitUntil(() => buttonPressed == true);
      //  }
 //   }

    //Disbaling loading and stuff for now, just pressing button will load the scene
    public void FightButton()
    {
        SceneManager.LoadScene(6);
       

    }

    //https://www.youtube.com/watch?v=NyFYNsC3H8k 
}
