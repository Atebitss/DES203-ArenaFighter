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
        if (PlayerData.GetDevMode()){ countdownTime = PlayerData.GetDevRoundTime(); }
        countdownTimeAtStart = countdownTime;
    }

    void FixedUpdate()
    {
        if (!countdownActive && countdownTime > 0) { countdownActive = levelScript.introIsOver; } //activates countdown when intro countdown is over

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
                countdownActive = false;
                levelScript.TimeUp();

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
