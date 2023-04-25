using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuController : MonoBehaviour
{
    // Start is called before the first frame update

    private void Awake()
    {
        FindObjectOfType<AudioManager>().Play("MusicMenu");
    }

    public void Level1()
    {
    FindObjectOfType<AudioManager>().Play("SelectBeep");
  //  FindObjectOfType<AudioManager>().StopPlaying("LevelMusic");
        SceneManager.LoadScene(3);
    }

    public void Level2() 
    {
    FindObjectOfType<AudioManager>().Play("SelectBeep");
  //  FindObjectOfType<AudioManager>().StopPlaying("LevelMusic");
        SceneManager.LoadScene(3);
    }

}
