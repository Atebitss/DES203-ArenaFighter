using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SelectHandler : MonoBehaviour
{
    private bool confirmed = false;

    private LetterSelect[] selectors = new LetterSelect[3];
    private int selectorIndex = 0;

    [SerializeField] private GameObject[] upArrows = new GameObject[3];
    [SerializeField] private GameObject[] downArrows = new GameObject[3];
    [SerializeField] private GameObject[] confirms = new GameObject[3];

    [SerializeField] private ParticleSystem confetti;


    private string[] profanities = new string[]
        {"ASS",
        "COC","COK","COQ","CUM",
        "DCK","DIC","DIK","DIQ",
        "FAG","FGT","FAP","FCK","FUC","FUK","FUQ","FCU","FKU","FQU",
        "JEW","JIZ",
        "KKK","KUM",
        "NGR","NIG",
        "PIS",
        "SEX",
        "TIT",
        "VAJ", "VAG"};
    private string[] extraProfanities = new string[]
        {"ASS","A$S","AS$","A$$",
        "COC","COK","COQ","CUM","C0C","C0K","C0Q",
        "DCK","DIC","DIK","DIQ","D1C","D1K","D1Q",
        "FAG","FGT","FAP","FCK","FUC","FUK","FUQ","FCU","FKU","FQU",
        "JEW","JIZ","J3W","J1Z",
        "KKK","KUM",
        "NGR","NIG","N1G",
        "PIS", "P1S", "PI5", "P15",
        "SEX","S3X","$EX","$3X",
        "TIT","T1T",
        "VAJ", "VAG", "V4J", "V4G"};
        


    void Start()
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler Start"); }
        for(int i = 0; i < selectors.Length; i++) 
        { 
            selectors[i] = GameObject.Find("Input" + (i + 1)).GetComponent<LetterSelect>(); 
            selectors[i].WakeDisplay();

            upArrows[i].SetActive(false);
            downArrows[i].SetActive(false);
            confirms[i].SetActive(false);
        }

        upArrows[0].SetActive(true);
        downArrows[0].SetActive(true);
        confirms[0].SetActive(true);

        Invoke("TransitionWait", 1f);
    }

    private void TransitionWait() { confetti.Play(); }


    public void OnUp(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnUp"); }
        //if (PlayerData.GetDevMode()) { Debug.Log("selector: " + selectors[selectorIndex]); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[PlayerData.playerPositions[0]]) 
        { 
            selectors[selectorIndex].LetterInc(ctx);
            upArrows[selectorIndex].GetComponent<Image>().color = new Color(1.0f, 0f, 0.875f);
            Invoke("DeHighlight", 0.1f);
        }
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnDown"); }
        //if (PlayerData.GetDevMode()) { Debug.Log("selector: " + selectors[selectorIndex]); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[PlayerData.playerPositions[0]]) 
        { 
            selectors[selectorIndex].LetterDec(ctx);
            downArrows[selectorIndex].GetComponent<Image>().color = new Color(1.0f, 0f, 0.875f);
            Invoke("DeHighlight", 0.1f);
        }
    }

    private void DeHighlight()
    {
        upArrows[selectorIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f);
        downArrows[selectorIndex].GetComponent<Image>().color = new Color(1f, 1f, 1f);
    }


    public void OnForward(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnForward"); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[PlayerData.playerPositions[0]] && selectorIndex < (selectors.Length - 1))
        {
            upArrows[selectorIndex].SetActive(false);
            downArrows[selectorIndex].SetActive(false);
            confirms[selectorIndex].SetActive(false);

            selectorIndex++;

            upArrows[selectorIndex].SetActive(true);
            downArrows[selectorIndex].SetActive(true);
            confirms[selectorIndex].SetActive(true);
        }
    }

    public void OnBack(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnBack"); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[PlayerData.playerPositions[0]] && selectorIndex > 0)
        {
            upArrows[selectorIndex].SetActive(false);
            downArrows[selectorIndex].SetActive(false);
            confirms[selectorIndex].SetActive(false);

            selectorIndex--;

            upArrows[selectorIndex].SetActive(true);
            downArrows[selectorIndex].SetActive(true);
            confirms[selectorIndex].SetActive(true);
        }
    }


    public void OnContinue(InputAction.CallbackContext ctx)
    {
        //if (PlayerData.GetDevMode()) { Debug.Log("SelectHandler OnContinue"); }
        if (ctx.started && ctx.control.device == PlayerData.playerDevices[PlayerData.playerPositions[0]]) 
        {
            //Debug.Log(ctx.control.device + " == " + PlayerData.playerDevices[PlayerData.playerPositions[0]);
            string selectedChars = "" + selectors[0].GetChar() + selectors[1].GetChar() + selectors[2].GetChar();

            foreach(string x in extraProfanities) { if (selectedChars.Equals(x)) { selectedChars = "UwU"; }}

            PlayerData.playerName = selectedChars;
            confirmed = true;
        }
    }
    public bool IsConfirmed() { return confirmed; if (PlayerData.GetDevMode()) { Debug.Log("player confirmed: " + confirmed); } }
}