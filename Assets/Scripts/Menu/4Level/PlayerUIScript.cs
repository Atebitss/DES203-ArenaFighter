using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerUIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI UIText;
    [SerializeField] private Image UIImage;
    [SerializeField] private GameObject UIX;
    [SerializeField] private Material originMat;
    [SerializeField] private Material invincibilityMat;
    private GameObject player;
    private PlayerController playerScript;

    void Start()
    {
        UIText.text = "---THIS IS EMPTY UI TEXT---";
        UIX.SetActive(false);
        UIText.color = new Color32(0, 0, 0, 255);
    }


    void Update()
    {
        if (playerScript != null)
        {
            UIText.text =
                "Kills " + playerScript.GetScore();
                //+ "\nTSLK: " + playerScript.GetTimeSinceLastKill().ToString("F2");

            if (playerScript.GetIsDying()) 
            {
                UIX.SetActive(true); 
            }
            else if (UIX.activeSelf && !playerScript.GetIsDying() && playerScript.GetIsInvincible()) 
            { 
                UIX.SetActive(false); 
                StartCoroutine(InvincibilityFlash());
            }
        }
    }


    public void SetPlayer(GameObject newPlayer, Sprite headshotSprite)
    {
        //Debug.Log("Setting new player - " + newPlayer.name);
        player = newPlayer;
        playerScript = player.GetComponent<PlayerController>();
        UIImage.sprite = headshotSprite;
    }



    private IEnumerator InvincibilityFlash()
    {
        //Debug.Log("InvincibilityFlash");
        //flashing duration and interval
        float flashDuration = .75f;
        float flashInterval = 1f / 5f;
        //Debug.Log("Dur: " + flashDuration + "/Int: " + flashInterval);

        while (flashDuration > 0)
        {
            //set renderer material to flash, wait, set back

            //Debug.Log("on");
            UIImage.material = invincibilityMat;
            yield return new WaitForSeconds(flashInterval);
            flashDuration -= flashInterval;

            //Debug.Log("off");
            UIImage.material = originMat;
            yield return new WaitForSeconds(flashInterval);
            flashDuration -= flashInterval;
        }

        //set renderer material to origin on flash end
        UIImage.material = originMat;
        //Debug.Log("finished");
    }
}
