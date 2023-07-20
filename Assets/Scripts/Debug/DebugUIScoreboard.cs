using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DebugUIScoreboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;

    private void Update()
    {
        debugText.text =
              "Position 1: Player " + (PlayerData.playerPositions[0]) + " with score " + PlayerData.playerScores[0] +
            "\nPosition 2: Player " + (PlayerData.playerPositions[1]) + " with score " + PlayerData.playerScores[1] +
            "\nPosition 3: Player " + (PlayerData.playerPositions[2]) + " with score " + PlayerData.playerScores[2] +
            "\nPosition 4: Player " + (PlayerData.playerPositions[3]) + " with score " + PlayerData.playerScores[3];
    }
}
