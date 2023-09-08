using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//if (PlayerData.GetDevMode()) { Debug.Log(); }

public class CharacterSelectHandler : MonoBehaviour
{
    [SerializeField] private Material greyMaterial;
    private int[] confirmedCharacterIDs = new int[] { -1, -1, -1, -1 };
    private CharacterSelectorScript[] css = new CharacterSelectorScript[4];

    public void SelectCharcter(int playerNum, int charID) 
    {
        if (PlayerData.GetDevMode()) { Debug.Log("csh.SelectedCharacter, " + playerNum + "/" + charID); }

        //update confirmed character with provided ID
        confirmedCharacterIDs[playerNum] = charID;

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

        //if (PlayerData.GetDevMode()) { DisplayStats(); }
    }
    public void DeselectCharacter(int playerNum) 
    { 
        //for each player, check if their current sprite index is equal to provided id
        for (int check = 0; check < PlayerData.GetNumOfPlayers(); check++)
        {
            if (PlayerData.GetDevMode()) { Debug.Log("player" + check + " spite index: " + css[check].GetSpriteIndex()); }
            if (css[check].GetSpriteIndex() == confirmedCharacterIDs[playerNum] && check != playerNum)
            {
                if (PlayerData.GetDevMode()) { Debug.Log("player" + check + " is on now deselected, ungreying"); }
                //if true, change material to default
                css[check].ResetMaterial();
            }
        }

        //if (PlayerData.GetDevMode()) { DisplayStats(); } 

        confirmedCharacterIDs[playerNum] = -1;
    }
    public bool IsCharacterConfirmed(int charID)
    {
        //if(PlayerData.GetDevMode()){Debug.Log("CSH IsCharacterConfirmed()");}
        //for each player, if provided character id is equal to confirmed character id, return true otherwise false
        for (int check = 0; check < PlayerData.GetNumOfPlayers(); check++)
        {
            if (charID == confirmedCharacterIDs[check]) 
            { 
                if(PlayerData.GetDevMode()){Debug.Log(charID + " is locked");} 
                return true; 
            }
        }

        //if(PlayerData.GetDevMode()){Debug.Log(charID + " not locked");}
        return false;
    }
    public int GetConfirmedID(int playerNum) { return confirmedCharacterIDs[playerNum]; }
    public int[] GetConfirmedIDs() { return confirmedCharacterIDs; }


    public bool AreAllPlayersConfirmed()
    {
        //if (PlayerData.GetDevMode()) { DisplayStats(); }

        for (int player = 0; player < PlayerData.GetNumOfPlayers(); player++)
        {
            for(int otherPlayer = 1; otherPlayer < PlayerData.GetNumOfPlayers() - player -1; otherPlayer++)
            {
                if(confirmedCharacterIDs[player] == confirmedCharacterIDs[otherPlayer])
                {
                    if (PlayerData.GetDevMode()) 
                    { 
                        Debug.Log("ID " + confirmedCharacterIDs[player] + " appears more than once");
                        Debug.Log("player " + player + " ID: " + confirmedCharacterIDs[player]);
                        Debug.Log("other  " + otherPlayer + " ID: " + confirmedCharacterIDs[otherPlayer]);
                    }
                    return false;
                }
            }

            if(confirmedCharacterIDs[player] == -1) 
            {
                if (PlayerData.GetDevMode()) { Debug.Log("Player" + player + " has not confirmed character"); }
                return false;
            }
        }

        if (PlayerData.GetDevMode()) { Debug.Log("All players confirmed"); }
        return true;
    }


    public CharacterSelectorScript GetCSS(int scriptIndex) { return css[scriptIndex]; }
    public void AddCSS(int scriptIndex, CharacterSelectorScript script) { css[scriptIndex] = script; }
    public void DelCSS(int scriptIndex) { css[scriptIndex] = null; }


    void OnDestroy()
    {
        if (PlayerData.GetDevMode()) { Debug.Log("csh destroyed"); }
        PlayerData.SetSpriteIDs(confirmedCharacterIDs);
    }




    private void DisplayStats()
    {
        for(int i = 0; i < 4; i++)
        {
            Debug.Log("cur ConfirmedIDs\nplayer" + i + ": " + confirmedCharacterIDs[i]);
        }
    }
}
