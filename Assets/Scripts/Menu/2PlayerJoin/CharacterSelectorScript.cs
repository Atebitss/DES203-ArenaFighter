using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterSelectorScript : MonoBehaviour
{
    private int playerNum;
    private int spriteIndex = 0;                            //sprite id
    private bool interactable = false;                      //allow inputs
    private PlayerJoinHandler pjh;
    private CharacterSelectHandler csh;

    //image components
    [SerializeField] private Image imageComponent;
    [SerializeField] private Sprite newImage;
    [SerializeField] private Sprite emptyImage;

    //arrows
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject downArrow;



    void Awake()
    {
        //ref joining script & select manager
        pjh = GameObject.Find("PlayerJoinManager").GetComponent<PlayerJoinHandler>();
        csh = GameObject.Find("CharacterSelectManager").GetComponent<CharacterSelectHandler>();

        //set players num & disable 'press a'
        playerNum = pjh.CurrentPlayer();
        GameObject.Find("A" + (playerNum + 1)).SetActive(false);

        //update name and position in heirarchy
        this.transform.name = "CharacterSelector" + playerNum;
        this.transform.SetParent(GameObject.Find("Box" + (playerNum + 1)).transform, false);

        //update image & csh
        csh.AddCSS(playerNum, this);
        ImageChange();
    }

    void OnDestroy()
    {
        csh.DelCSS(playerNum);
    }



    public void ImageChange()
    {
        FindObjectOfType<AudioManager>().Play("Beep");
        imageComponent.sprite = newImage;
        //imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 1);
    }

    public void ImageChange(Sprite newSpirte)
    {
        imageComponent.sprite = newSpirte;
        //Debug.Log("updating image with " + newSpirte);
    }

    public void ResetImage()
    {
        FindObjectOfType<AudioManager>().Play("Beep");
        imageComponent.sprite = emptyImage;
        imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0);
        //Debug.Log("resetting image");
    }


    //up/down function
    //  increase/decrease sprite num to temp int
    //  check if temp int isnt already claimed
    //  if so, set sprite num to temp int
    //  play arrow flash animation
    //  update display
    public void OnUp(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("OnUp");
        }
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("OnDown");
        }
    }


    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("OnConfirm");
        }
    }

    public void OnBack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("OnBack");
        }
    }


    public void OnContinue(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("OnContinue");
        }
    }


    public void EnableInput(bool b)
    {
        Debug.Log(this.gameObject.name + " enabled: " + b);
        interactable = b;
    }
}
