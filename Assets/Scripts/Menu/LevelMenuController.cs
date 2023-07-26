using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuController : MonoBehaviour
{
    private AudioManager AM;

    private void Awake()
    {
        AM = FindObjectOfType<AudioManager>();
    }

    private void Start()
    { 
        AM.Play("MusicMenu");
        AM.Play("SpookyNoise");
    }

    public void Back()
    {
        AM.Play("SelectBeep");
        SceneManager.LoadScene(1);
    }

    public void Level1()
    {
        AM.Play("SelectBeep");
  //  AM.StopPlaying("LevelMusic");
        SceneManager.LoadScene(3);
    }

    public void Level2() 
    {
        AM.Play("SelectBeep");
        //AM.StopPlaying("LevelMusic");
        //SceneManager.LoadScene(x);
    }

}
