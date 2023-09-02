using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectHandler : MonoBehaviour
{
    private int[] characterIDConfirmed = new int[] { -1, -1, -1, -1 };

    public void SelectCharcter(int playerNum, int charID) { characterIDConfirmed[playerNum] = charID; }
    public void DeselectCharacter(int playerNum) { characterIDConfirmed[playerNum] = -1; }
    public bool IsCharacterConfirmed(int charID)
    {
        Debug.Log("CSH IsCharacterConfirmed()");
        //for each player, if provided character id is equal to confirmed character id, return true otherwise false
        for (int check = 0; check < PlayerData.GetNumOfPlayers(); check++)
        {
            if (charID == characterIDConfirmed[check]) { Debug.Log(charID + " is locked"); return true; }
        }

        Debug.Log(charID + " not locked");
        return false;
    }


    private CharacterSelectorScript[] css = new CharacterSelectorScript[4];

    public CharacterSelectorScript GetCSS(int scriptIndex) { return css[scriptIndex]; }
    public void AddCSS(int scriptIndex, CharacterSelectorScript script) { css[scriptIndex] = script; }
    public void DelCSS(int scriptIndex) { css[scriptIndex] = null; }


    void OnDestroy()
    {
        Debug.Log("csh destroyed");
        PlayerData.SetSpriteIDs(characterIDConfirmed);
    }
}
