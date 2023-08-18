using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectHandler : MonoBehaviour
{
    private LetterSelect[] selectors = new LetterSelect[3];
    private int selectorIndex = 0;


    void Start()
    {
        Debug.Log("SelectHandler Start");
        for(int i = 0; i < selectors.Length; i++) { selectors[i] = GameObject.Find("Input" + (i + 1)).GetComponent<LetterSelect>(); selectors[i].WakeDisplay(); }
    }


    public void OnUp(InputAction.CallbackContext ctx)
    {
        Debug.Log("SelectHandler OnUp");
        Debug.Log("selector: " + selectors[selectorIndex]);

        if (ctx.started) { selectors[selectorIndex].LetterInc(ctx); }
    }

    public void OnDown(InputAction.CallbackContext ctx)
    {
        Debug.Log("SelectHandler OnDown");
        Debug.Log("selector: " + selectors[selectorIndex]);
        if (ctx.started) { selectors[selectorIndex].LetterDec(ctx); }
    }


    public void OnConfirm(InputAction.CallbackContext ctx)
    {
        Debug.Log("SelectHandler OnConfirm");
        if (ctx.started && selectorIndex < (selectors.Length - 1)) { selectorIndex++; }
    }

    public void OnBack(InputAction.CallbackContext ctx)
    {
        Debug.Log("SelectHandler OnBack");
        if (ctx.started && selectorIndex > 0) { selectorIndex--; }
    }
}