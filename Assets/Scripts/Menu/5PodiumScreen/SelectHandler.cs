using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectHandler : MonoBehaviour
{
    private LetterSelect[] selectors = new LetterSelect[3];
    private int selectorIndex = 0;
    private string[] profanities = new string[]
        {"ASS",
        "COC","COK","COQ","CUM",
        "DCK","DIC","DIK","DIQ",
        "FAG","FGT","FAP","FCK","FUC","FUK","FUQ","FKU",
        "JEW","JIZ",
        "KKK","KUM",
        "NGR","NIG",
        "SEX",
        "TIT"};
    private string[] extraProfanities = new string[]
        {"ASS","A$S","AS$","A$$",
        "COC","COK","COQ","CUM","C0C","C0K","C0Q",
        "DCK","DIC","DIK","DIQ","D1C","D1K","D1Q",
        "FAG","FGT","FAP","FCK","FUC","FUK","FUQ","FKU",
        "JEW","JIZ","J3W","J1Z",
        "KKK","KUM",
        "NGR","NIG","N1G",
        "SEX","S3X","$EX","$3X",
        "TIT","T1T"};
        


    void Start()
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler Start"); }
        for(int i = 0; i < selectors.Length; i++) { selectors[i] = GameObject.Find("Input" + (i + 1)).GetComponent<LetterSelect>(); selectors[i].WakeDisplay(); }
    }


    public void OnUp(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnUp"); }
        //if (PlayerData.GetDevMode()) { Debug.Log("selector: " + selectors[selectorIndex]); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[0]) { selectors[selectorIndex].LetterInc(ctx); }
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnDown"); }
        //if (PlayerData.GetDevMode()) { Debug.Log("selector: " + selectors[selectorIndex]); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[0]) { selectors[selectorIndex].LetterDec(ctx); }
    }


    public void OnForward(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnForward"); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[0] && selectorIndex < (selectors.Length - 1)) { selectorIndex++; }
    }

    public void OnBack(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnBack"); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[0] && selectorIndex > 0) { selectorIndex--; }
    }


    public void OnContinue(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnContinue"); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[0]) 
        {
            string selectedChars = "" + selectors[0].GetChar() + selectors[1].GetChar() + selectors[2].GetChar();

            foreach(string x in extraProfanities) { if (selectedChars.Equals(x)) { selectedChars = "UwU"; }}

            PlayerData.playerName = selectedChars;
        }
    }
}