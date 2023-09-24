using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrows : MonoBehaviour
{
    [SerializeField] private Sprite[] pArrow = new Sprite[4];
    [SerializeField] private SpriteRenderer outlineR, colourR;
    private bool showing = true;

    public Transform playerPrefab;

    public void StartArrow(int playerNum)
    {
        outlineR.sprite = pArrow[playerNum];
        switch (PlayerData.GetSpriteID(playerNum))
        {
            case 0:
                colourR.color = new Color(1f, 0f, 0f, 1f); //red
                break;
            case 1:
                colourR.color = new Color(0f, 1f, 0f, 1f); //green
                break;
            case 2:
                colourR.color = new Color(1f, 0f, 1f, 1f); //pink
                break;
            case 3:
                colourR.color = new Color(0.5f, 1f, 0.75f, 1f); //teal
                break;
            case 4:
                colourR.color = new Color(0.75f, 0f, 1f, 1f); //purple
                break;
            default:
                break;
        }
    }


    public void ShowArrow()
    {
        showing = true;
        outlineR.enabled = true;
        colourR.enabled = true;
    }
    public void HideArrow()
    {
        showing = false;
        outlineR.enabled = false;
        colourR.enabled = false;
    }


    private void Update() //keep arrows facing the correct way regardless of the players direction
    {
        if (showing)
        {
            if (playerPrefab.localScale.x < 0)
            {
                transform.localScale = new Vector3(-.25f, .25f, 1f);
            }
            else if(playerPrefab.localScale.x > 0)
            {
                transform.localScale = new Vector3(.25f, .25f, 1f);
            }
        }
    }
}
