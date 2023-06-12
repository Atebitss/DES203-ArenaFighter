using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{

   //Timer and unit conversion code from https://www.youtube.com/watch?v=hxpUk0qiRGs

    
    [SerializeField] private float countdownTime = 180f;
    private float countdownTimeAtStart;
    public TextMeshProUGUI timer;
    public bool countdownActive;
    [SerializeField] private LevelScript levelScript;
    public bool runningOutOfTime;
    
    // Start is called before the first frame update
    void Start()
    {

        countdownActive = true;
        runningOutOfTime = false;
        countdownTimeAtStart = countdownTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (countdownActive == true)
        {
            if (countdownTime > 0)
            {
                countdownTime -= Time.deltaTime;
                UpdateTimer(countdownTime);
            }
            else
            {
                countdownTime = 0;
                print("Time up");

                levelScript.GetComponent<LevelScript>().TimeUp();
            }
            
            if (countdownTime < countdownTimeAtStart / 6) 
            {
                runningOutOfTime = true;
            }
        }
    
    }
    void UpdateTimer(float currentTime)
    {
        //converts the countdowntime float into exact minutes and seconds to display on screen
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timer.text = string.Format("{0:0}:{1:00}", minutes, seconds);

        //changes text colour to red when timer hits under a 6th of its starting value (3 minutes -> 30 seconds | 1 minute -> 10 seconds)
        if ( runningOutOfTime == true)
        {
            timer.color = Color.red;
        }
        else
        {
            timer.color = Color.white;
        }
    }
   

}
