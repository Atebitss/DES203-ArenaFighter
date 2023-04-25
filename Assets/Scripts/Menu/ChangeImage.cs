using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    // Start is called before the first frame update
    public Image oldImage;
    public Sprite newImage;

    void Start()
    {
        //FindObjectOfType<AudioManager>().Play("PlayerMusic");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ImageChange()
    {
     FindObjectOfType<AudioManager>().Play("Beep");
        oldImage.sprite = newImage;

    }

}
