using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectHandler : MonoBehaviour
{
    private CharacterSelectorScript[] css = new CharacterSelectorScript[4];

    public CharacterSelectorScript GetCSS(int scriptIndex) { return css[scriptIndex]; }
    public void AddCSS(int scriptIndex, CharacterSelectorScript script) { css[scriptIndex] = script; }
    public void DelCSS(int scriptIndex) { css[scriptIndex] = null; }
}
