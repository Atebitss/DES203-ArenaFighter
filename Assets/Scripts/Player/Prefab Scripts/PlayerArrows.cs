using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrows : MonoBehaviour
{
    public Sprite player1Arrow;
    public Sprite player2Arrow;
    public Sprite player3Arrow;
    public Sprite player4Arrow;

    public SpriteRenderer renderer;
    public Transform playerPrefab;

    public void UpdateArrow(int playerNum)
    {
        switch (playerNum)
        {
            case 0:
                renderer.sprite = player1Arrow;
                break;
            case 1:
                renderer.sprite = player2Arrow;
                break;
            case 2:
                renderer.sprite = player3Arrow;
                break;
            case 3:
                renderer.sprite = player4Arrow;
                break;
           
        }
    }
    public void HideArrow()
    {
        renderer.enabled = false;
    }
    public void ShowArrow()
    {
        renderer.enabled = true;
    }
    private void Update() //keep arrows facing the correct way regardless of the players direction
    {
        if (playerPrefab.localScale.x < 0)
        {
            transform.localScale = new Vector3(-0.1f, 0.1f, 1);
        }
        else
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 1);
        }
       
    }
}
