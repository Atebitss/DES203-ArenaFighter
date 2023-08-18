using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectHandler : MonoBehaviour
{
    [SerializeField] private LetterSelect[] selectors = new LetterSelect[3];
    private int selectIndex = 0;

    public void OnUpDown(InputAction.CallbackContext ctx)
    {

    }

    public void OnConfirm(InputAction.CallbackContext ctx)
    {

    }

    public void OnBack(InputAction.CallbackContext ctx)
    {

    }
}