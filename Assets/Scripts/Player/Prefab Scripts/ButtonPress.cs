using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public SpriteRenderer renderer;
    public Transform playerPrefab;

    
    public void HideButtonPress()
    {
        renderer.enabled = false;
    }
    public void ShowButtonPress()
    {
        renderer.enabled = true;
    }
    private void Update() //keep buttons facing the correct way regardless of the players direction
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
