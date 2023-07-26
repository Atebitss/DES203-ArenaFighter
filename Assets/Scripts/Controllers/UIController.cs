using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    //Timer and unit conversion code from https://www.youtube.com/watch?v=hxpUk0qiRGs
    [Header("Refrences")]
    [SerializeField] private LevelScript levelScript;
    public TextMeshProUGUI timer;
    public Image introCountdown;
    [Header("Level Timer")]
    [SerializeField] private float countdownTime = 180f;
    private float countdownTimeAtStart;
    public bool countdownActive;
    public bool runningOutOfTime;
    
    void Start()
    {
        runningOutOfTime = false;
        if (PlayerData.devMode){ countdownTime = 5; }
        
        countdownTimeAtStart = countdownTime;
    }

    void FixedUpdate()
    {
        countdownActive = levelScript.GetComponent<LevelScript>().introIsOver; //activates countdown when intro countdown is over
      
        if (countdownActive == true)
        {
            introCountdown.gameObject.SetActive(false); // disbales intro timer

            if (countdownTime > 0)
            {
                countdownTime -= Time.deltaTime;
                UpdateTimer(countdownTime);
            }
            else
            {
                countdownTime = 0;
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
