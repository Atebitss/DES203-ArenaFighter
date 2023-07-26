using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    public Image imageComponent;
    public Sprite newImage;
    public Sprite emptyImage;
    private AudioManager AM;

    void Awake()
    {
        AM = FindObjectOfType<AudioManager>();
    }

    public void ImageChange()
    {
        AM.Play("Beep");
        imageComponent.sprite = newImage;
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 1);
        //Debug.Log("updating image");
    }

    public void ImageChange(Sprite newSpirte)
    {
        imageComponent.sprite = newSpirte;
        //Debug.Log("updating image with " + newSpirte);
    }

    public void ResetImage()
    {
        AM.Play("Beep");
        imageComponent.sprite = emptyImage;
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0);
        //Debug.Log("resetting image");
    }
}
