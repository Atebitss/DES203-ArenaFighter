using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerUIArrow : MonoBehaviour
{
    [SerializeField] private Image colourI;

    public void StartArrow(int spriteID)
    {
        switch (spriteID)
        {
            case 0:
                colourI.color = new Color(1f, 0f, 0f, 1f); //red
                break;
            case 1:
                colourI.color = new Color(0f, 1f, 0f, 1f); //green
                break;
            case 2:
                colourI.color = new Color(1f, 0f, 1f, 1f); //pink
                break;
            case 3:
                colourI.color = new Color(0.5f, 1f, 0.75f, 1f); //teal
                break;
            case 4:
                colourI.color = new Color(0.75f, 0f, 1f, 1f); //purple
                break;
            default:
                break;
        }
    }
}
