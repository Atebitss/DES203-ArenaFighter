using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Transform playerPrefab;
    public Sprite[] breakFreeAnimSprites;
    private int counter = 0;

     public void HideIce()
    {
        renderer.enabled = false;
    }
    public void ShowIce()
    {
        renderer.enabled = true;
    }
    public void BreakIce()
    {
        renderer.sprite = breakFreeAnimSprites[counter];
        counter++;
    }
    private void Update() //keep buttons facing the correct way regardless of the players direction
    {
        if (playerPrefab.localScale.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1);
        }

    }
}

