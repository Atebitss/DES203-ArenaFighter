using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    //Timer and unit conversion code from https://www.youtube.com/watch?v=hxpUk0qiRGs
    [Header("Refrences")]
    [SerializeField] private LevelScript levelScript;
    public Image introCountdown;
    public Image finalCountdown;
    public Image roundOver;

    [Header("Level Timer")]
    [SerializeField] private float countdownTime = 180f;
    private float countdownTimeAtStart;
    public bool countdownActive;
    public bool runningOutOfTime;
    
    void Start()
    {
        runningOutOfTime = false;
        finalCountdown.gameObject.SetActive(false);
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

            }
            else
            {
                countdownTime = 0;
                levelScript.GetComponent<LevelScript>().TimeUp();

                finalCountdown.gameObject.SetActive(false);
                roundOver.gameObject.SetActive(true);

            }

            if (countdownTime <= 10f && countdownTime > 0)
            {
                finalCountdown.gameObject.SetActive(true);
            }
        }
    }
}
