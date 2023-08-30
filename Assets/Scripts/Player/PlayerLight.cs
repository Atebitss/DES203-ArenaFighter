using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    public Light2D light;
    private Vector2 position;

    private void Start()
    {
        position = transform.localPosition;
    }
    public void HideLight()
    {
        light.enabled = false;
    }
    public void ShowLight()
    {
        light.enabled = true;
    }
    public void FlipLight(bool flipRight)
    {
        if (flipRight)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 270);
            transform.localPosition = new Vector2(-position.x, position.y);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
            transform.localPosition = position;
        }
        

    }
}
