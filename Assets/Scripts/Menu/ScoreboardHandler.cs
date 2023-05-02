using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardHandler : MonoBehaviour
{
    [SerializeField] private Sprite[] playerSprites = new Sprite[4];

    void Awake()
    {
        //Debug.Log("ScoreboardHandler Awake()");
        int[] scores = new int[3];                //hold all player scores {0: 2,   1: 1,   2: 3,   3: 0}
        int[] scoresPlayer = new int[3];
        int[] scoreboardPlayerPos = new int[3];   //holds the players position in the scoreboard {0: 2nd,   1: 3rd,   2: 1st,   3: 4th}
        int pos = 2;

        for(int player = 0; player < scores.Length; player++)
        {
            //for each player
            //Debug.Log("player: " + player + "   score: " + PlayerData.playerScores[player]);
            for (int compare = 0; compare < scores.Length; compare++)
            {
                //compare their score to other players
                //if (player != compare) { Debug.Log("compared player: " + compare + "   score: " + PlayerData.playerScores[compare]); }
                if (PlayerData.playerScores[player] > PlayerData.playerScores[compare] && player != compare)
                {
                    //their score was higher than the others, so their position is increased;
                    //Debug.Log("player score(" + PlayerData.playerScores[player] + ") > compared player score(" + PlayerData.playerScores[compare] + ")");
                    pos--;
                }
            }

            //update scoreboard pos then reset pos for the next player
            //Debug.Log("---player " + player + " pos: " + pos + " with " + PlayerData.playerScores[player] + " kill---");
            scoreboardPlayerPos[player] = pos;
            scoresPlayer[pos] = player;
            scores[pos] = PlayerData.playerScores[player];
            pos = 2;
        }


        //Debug.Log("___________________________");
        //for(int i = 0; i < scores.Length; i++) { Debug.Log("pos " + (i+1) + ": player " + scoresPlayer[i] + " with " + scores[i] + " kills"); }



        for (int i = 0; i < scores.Length; i++)
        {
            string imgRef = "Image" + (i + 1);
            GameObject.Find(imgRef).GetComponent<ChangeImage>().ImageChange(playerSprites[scoresPlayer[i]]);

            string textRef = "Text" + (i + 1);
            switch(i)
            {
                case 0:
                    GameObject.Find(textRef).GetComponent<TMPro.TextMeshProUGUI>().text = "1ST\nKILLS: " + scores[i];
                    break;
                case 1:
                    GameObject.Find(textRef).GetComponent<TMPro.TextMeshProUGUI>().text = "2ND\nKILLS: " + scores[i];
                    break;
                case 2:
                    GameObject.Find(textRef).GetComponent<TMPro.TextMeshProUGUI>().text = "3RD\nKILLS: " + scores[i];
                    break;
            }
        }


        PlayerData.ResetScores();
    }
}
