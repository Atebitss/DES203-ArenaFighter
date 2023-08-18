using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class LetterSelect : MonoBehaviour
{
    /*player input
        -up & down on thumbsticks run displayed letter update
        -limited to winner controller (ref PD.devices v cur device)
    */

    [SerializeField] private Text display;  //reference game object text component
    private int charIndex = 0;
    private char[] chars = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    private char[] devChars = new char[]     //▲alphabet array▲ & ▼dev alphabet array▼
    {
              /*letters*/'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                /*maths*/'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '-', '×', '÷', '%', '√', '∞',
               /*shapes*/'☺', '♥', '♦', '♣', '♠','•','◘','○','◙','♂','♀','♪','♫','☼', '†', 'Þ', ':', ';', 'ˆ', '⌂', '░', '▒', '▓', '■', '▬',
               /*arrows*/'▲', '►', '▼', '◄','↕',
                /*marks*/'!', '‼', '?', '¿', '©', '®', '™', '.',
             /*squigles*/'§','▬', '≈', '≡', '≤', '≥',
                /*money*/'£', '¢', '¥', '$', '¤',
        /*greek letters*/'Γ', 'Θ', 'Σ', 'Φ', 'Ω', 'α', 'δ', 'ε', 'π', 'σ', 'τ', 'φ'

               /*spares*///'', '', '', '', '', '', '', '', ''
    };



    public void LetterInc(InputAction.CallbackContext ctx)
    {
        Debug.Log("LetterSelect LetterInc");

        if (PlayerData.GetDevMode() && charIndex < (devChars.Length - 1)){ charIndex++; UpdateDisplay(); }
        else if (charIndex < (chars.Length - 1)) { charIndex++; UpdateDisplay(); }
    }

    public void LetterDec(InputAction.CallbackContext ctx)
    {
        Debug.Log("LetterSelect LetterDec");
        if (PlayerData.GetDevMode() && charIndex > 0) { charIndex--; UpdateDisplay(); }
        else if (charIndex > 0) { charIndex--; UpdateDisplay(); }
    }


    public void WakeDisplay(){ UpdateDisplay(); }

    private void UpdateDisplay()
    {
        Debug.Log("LetterSelect UpdateDisplay");
        Debug.Log("char index: " + charIndex);
        if (PlayerData.GetDevMode()) { display.text = "" + devChars[charIndex]; }
        else { display.text = "" + chars[charIndex]; }
    }


    public char GetChar() 
    {
        if (PlayerData.GetDevMode()) { return devChars[charIndex]; }
        else { return chars[charIndex]; }
    }
}
