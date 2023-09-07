using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//if (PlayerData.GetDevMode()) { Debug.Log(); }

public class CharacterSelectHandler : MonoBehaviour
{
    [SerializeField] private Material greyMaterial;
    private int[] characterIDConfirmed = new int[] { -1, -1, -1, -1 };
    private CharacterSelectorScript[] css = new CharacterSelectorScript[4];

    public void SelectCharcter(int playerNum, int charID) 
    {
        if (PlayerData.GetDevMode()) { Debug.Log("csh.SelectedCharacter, " + playerNum + "/" + charID); }

        //update confirmed character with provided ID
        characterIDConfirmed[playerNum] = charID;

        //for each player, check if their current sprite index is equal to provided id
        for (int check = 0; check < PlayerData.GetNumOfPlayers(); check++)
        {
            if (PlayerData.GetDevMode()) { Debug.Log("player" + check + " spite index: " + css[check].GetSpriteIndex()); }
            if (css[check].GetSpriteIndex() == charID && check != playerNum)
            {
                if (PlayerData.GetDevMode()) { Debug.Log("player" + check + " is on now confirmed index, greying"); }
                //if true, change material to greyed out
                css[check].GetImageComp().material = greyMaterial;
            }
        }
    }
    public void DeselectCharacter(int playerNum) { characterIDConfirmed[playerNum] = -1; }
    public bool IsCharacterConfirmed(int charID)
    {
        //if(PlayerData.GetDevMode()){Debug.Log("CSH IsCharacterConfirmed()");}
        //for each player, if provided character id is equal to confirmed character id, return true otherwise false
        for (int check = 0; check < PlayerData.GetNumOfPlayers(); check++)
        {
            if (charID == characterIDConfirmed[check]) 
            { 
                if(PlayerData.GetDevMode()){Debug.Log(charID + " is locked");} 
                return true; 
            }
        }

        //if(PlayerData.GetDevMode()){Debug.Log(charID + " not locked");}
        return false;
    }



    public CharacterSelectorScript GetCSS(int scriptIndex) { return css[scriptIndex]; }
    public void AddCSS(int scriptIndex, CharacterSelectorScript script) { css[scriptIndex] = script; }
    public void DelCSS(int scriptIndex) { css[scriptIndex] = null; }


    void OnDestroy()
    {
        if(PlayerData.GetDevMode())
        {
            Debug.Log("csh destroyed");
            for(int check = 0; check < characterIDConfirmed.Length; check++)
            {
                Debug.Log(characterIDConfirmed[check]);
            }
        }

        PlayerData.SetSpriteIDs(characterIDConfirmed);
    }
}
