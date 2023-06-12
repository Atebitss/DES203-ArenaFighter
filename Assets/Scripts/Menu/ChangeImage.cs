using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    public Image imageComponent;
    public Sprite newImage;
    public Sprite emptyImage;

    public void ImageChange()
    {
        FindObjectOfType<AudioManager>().Play("Beep");
        imageComponent.sprite = newImage;
        Debug.Log("updating image");
    }

    public void ImageChange(Sprite newSpirte)
    {
        imageComponent.sprite = newSpirte;
        Debug.Log("updating image with " + newSpirte);
    }

    public void ResetImage()
    {
        FindObjectOfType<AudioManager>().Play("Beep");
        imageComponent.sprite = emptyImage;
    }
}
