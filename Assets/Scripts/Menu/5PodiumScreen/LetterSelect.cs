using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterSelect : MonoBehaviour
{
    /*player input
        -up & down on thumbsticks run displayed letter update
        -limited to winner controller (ref PD.devices v cur device)
    */

    [SerializeField] private Text display;  //reference game object text component
    private int charIndex = 0;
    private char[] chars = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    private char[] adChars = new char[]     //▲alphabet array▲ & ▼dev alphabet array▼
    {
                /*maths*/'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '-', '×', '÷', '%', '√', '∞',
               /*shapes*/'☺', '♥', '♦', '♣', '♠','•','◘','○','◙','♂','♀','♪','♫','☼', '†', 'Þ', ':', ';', 'ˆ', '⌂', '░', '▒', '▓', '■', '▬',
               /*arrows*/'▲', '►', '▼', '◄','↕',
                /*marks*/'!', '‼', '?', '¿', '©', '®', '™', '.',
             /*squigles*/'§','▬', '≈', '≡', '≤', '≥',
                /*money*/'£', '¢', '¥', '$', '¤',
        /*greek letters*/'Γ', 'Θ', 'Σ', 'Φ', 'Ω', 'α', 'δ', 'ε', 'π', 'σ', 'τ', 'φ'

               /*spares*///'', '', '', '', '', '', '', '', ''
    };


}
