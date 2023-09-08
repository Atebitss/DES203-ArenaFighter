using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterSelectorScript : MonoBehaviour
{
    private int playerNum;                                  //player num attached to game object
    private int spriteIndex = 0;                            //sprite id
    private bool confirmed = false;                         //allow inputs
    private PlayerJoinHandler pjh;
    private CharacterSelectHandler csh;

    //image components
    [SerializeField] private Image imageComponent;
    [SerializeField] private Sprite emptyImage;
    [SerializeField] private Material defaultMaterial;

    //arrows
    [SerializeField] private Image upArrow;
    [SerializeField] private Image downArrow;

    //confirms
    [SerializeField] private GameObject confirmA;
    [SerializeField] private GameObject confirmTick;



    public void Wake(int num)
    {
        //ref joining script & select manager
        pjh = GameObject.Find("PlayerJoinManager").GetComponent<PlayerJoinHandler>();
        csh = GameObject.Find("CharacterSelectManager").GetComponent<CharacterSelectHandler>();

        //set players num & disable 'press a'
        playerNum = num;
        if (csh.IsCharacterConfirmed(spriteIndex)) { spriteIndex = FindNextIndex(spriteIndex, 1); }
        confirmTick.SetActive(false);

        //update image & csh
        csh.AddCSS(playerNum, this);
        ImageChange(PlayerData.GetSprite(spriteIndex));
    }



    public void ImageChange(Sprite newSprite)
    {
        imageComponent.sprite = newSprite;
        if (imageComponent.color.a != 1) { imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 1); }
        if (!imageComponent.material.Equals("None")) { imageComponent.material = defaultMaterial; }   //if cur material isnt none, reset it
        //Debug.Log("updating image with " + newSpirte);
    }

    public void ResetImage()
    {
        imageComponent.sprite = emptyImage;
        if (imageComponent.color.a != 0) { imageComponent.color = new Color(imageComponent.color.r, imageComponent.color.g, imageComponent.color.b, 0); }
        //Debug.Log("resetting image");
    }

    public void ResetMaterial() { if (!imageComponent.material.Equals("None")) { imageComponent.material = defaultMaterial; } }   //if cur material isnt none, reset it


    //up/down function
    //  increase/decrease sprite num to temp int
    //  check if temp int isnt already claimed
    //  if so, set sprite num to temp int
    //  play arrow flash animation
    //  update display
    public void OnUp(InputAction.CallbackContext ctx)
    {
        //if up pressed & player has not confirmed character & not on last character
        if (ctx.started && !confirmed)
        {
            //Debug.Log("OnUp");
            //find next availible character up
            spriteIndex = FindNextIndex(spriteIndex, 1);

            //run arrow flash
            //if animator isnt empty
            //set bool 'upRun' to true - this sets the colour of the arrow & makes it jump
            //start coroutine(animation length, 1 sec or smthn)
            //after 1sec set bool to false - this sets the colour of the arrow and brings it back down
            Highlight("up");

            //update image with newly selected character
            ImageChange(PlayerData.GetSprite(spriteIndex));
        }
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {
        if (ctx.started && !confirmed)
        {
            //Debug.Log("OnDown");
            //find next availible character down
            spriteIndex = FindNextIndex(spriteIndex, -1);

            //run arrow flash
            //if animator isnt empty
            //set bool 'downRun' to true - this sets the colour of the arrow & makes it fall down
            //start coroutine(animation length, 1 sec or smthn)
            //after 1sec set bool to false - this sets the colour of the arrow and brings it back up
            Highlight("down");

            //update image with newly selected character
            ImageChange(PlayerData.GetSprite(spriteIndex));
        }
    }


    private void Highlight(string dir)
    {
        switch (dir) 
        {
            case "up":
                upArrow.color = new Color(1.0f, 0f, 0.875f);
                Invoke("DeHighlight", 0.1f);
                break;
            case "down":
                downArrow.color = new Color(1.0f, 0f, 0.875f);
                Invoke("DeHighlight", 0.1f);
                break;
            default: 
                if (PlayerData.GetDevMode()) { Debug.Log("no direction"); } 
                break;
        }
    }

    private void DeHighlight()
    {
        upArrow.color = new Color(1f, 1f, 1f);
        downArrow.color = new Color(1f, 1f, 1f);
    }


    private int FindNextIndex(int start, int dir)
    {
        //Debug.Log(this.transform.name + " FindNextIndex()");
        //temp int is scripts current sprite index
        int tempIndex = start;

        //for each player, set temp int +/-1 & check if temp int is locked in csh
        for (int check = 0; check < PlayerData.GetNumOfPlayers(); check++)
        {
            tempIndex = tempIndex + dir;
            //Debug.Log("checking index " + tempIndex);

            if (tempIndex < 0) 
            { 
                //Debug.Log("index at min, looping to max"); 
                tempIndex = PlayerData.GetSprites().Length - 1; 
            }
            else if (tempIndex >= PlayerData.GetSprites().Length) 
            { 
                //Debug.Log("index at max, looping to min"); 
                tempIndex = 0; 
            }

            if (!csh.IsCharacterConfirmed(tempIndex)) 
            { 
                //Debug.Log(tempIndex + " availible"); 
                return tempIndex; 
            }
        }

        //Debug.Log("all charcters locked");
        return -1;
    }



    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !confirmed)
        {
            //Debug.Log("OnConfirm");
            //if ID not already confirmed
            if (!csh.IsCharacterConfirmed(spriteIndex))
            {
                //update csh with character id
                csh.SelectCharcter(playerNum, spriteIndex);
                //set bool confirmed to true
                confirmed = true;
                confirmA.SetActive(false);
                confirmTick.SetActive(true);
            }
        }
    }

    public void OnBack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && confirmed)
        {
            //Debug.Log("OnBack");
            //update csh with -1
            csh.DeselectCharacter(playerNum);
            //set bool confirmed to false
            confirmed = false;
            confirmA.SetActive(true);
            confirmTick.SetActive(false);
        }
    }



    void OnDestroy()
    {
        if (PlayerData.GetDevMode()) { Debug.Log(this.transform.name + " destroyed"); }
        ResetImage();
        csh.DelCSS(playerNum);
    }



    public int GetSpriteIndex() { return spriteIndex; }
    public Image GetImageComp() { return imageComponent; }
}
