using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class LevelScript : MonoBehaviour
{
    //variable player stats
    [SerializeField] [Range(1, 10)] private float playerGravity = 2.5f;
    [SerializeField] [Range(1, 100)] private float playerMoveForce = 25f;
    [SerializeField] [Range(10, 50)] private float playerJumpForce = 25f;

    //debug
    [Header("Debug")]
    [SerializeField] private bool devMode = false;
    private DebugUIManager DUIM;

    //spawn points
    private GameObject[] spawnPoints;
    private int[] spawnOrder = new int[4];

    //PlayerData.playerInputs
    private PlayerJoinHandler pjh;
    private PlayerController[] playerScripts = new PlayerController[4];
    public GameObject[] players = new GameObject[4];
    private int curPlayerPos = 0;
    private int prevPlayerPos = 0;

    //important level & multiplayer stuff
    [Header("Level and Multiplayer")]
    [SerializeField] private InputAction joinAction;
    [SerializeField] private InputAction leaveAction;
    [SerializeField] private GameObject playerPrefab;
    public static LevelScript instance = null;
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;
    [SerializeField] [Range(1, 5)] private float roundMins = 3;

    //collectable stuff
    [Header("Collectables")]
    [SerializeField] private GameObject[] collectableType;
     private GameObject[] collectableSpawnPoints;
    [SerializeField] private float collectableSpawnInterval;
    [SerializeField] private float initialCollectableSpawnDelay;
    private bool collectableCanSpawn;
    private float intervalTime;



    //player sprites 
    // [SerializeField] private Sprite player1sprite;
    // [SerializeField] private Sprite player2sprite;
    // [SerializeField] private Sprite player3sprite;
    // [SerializeField] private Sprite player4sprite; 



    //~~~~~~~ LEVEL BASICS ~~~~~~~\\
    private void Awake()
    {
        //ensure there is only 1 level script
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        //PlayerData.GetPlayers();
        if (!PlayerData.gameRun) { PlayerData.gameRun = true; }

        //start level music
        FindObjectOfType<AudioManager>().Play("MusicFight");


        //set spawn point order
        SetSpawnPoints();


        //update debug
        if (devMode)
        {
           DUIM = GameObject.Find("DebugUI").GetComponent<DebugUIManager>();
        }
    }


    private void Start()
    {
        for (int player = 0; player < PlayerData.numOfPlayers; player++)
        {
            string playerRef = "PlayerJoin" + player;
            //Debug.Log(playerRef);
            Destroy(GameObject.Find(playerRef));
            //Debug.Log("curplayerpos currently " + curPlayerPos);
            curPlayerPos = player;
            //Debug.Log("setting curplayerpos to " + player);

            //saviour code from Rene-Damm in the Unity Forum - https://forum.unity.com/threads/local-multiplayer-lobby-scene-gameplay-scene.845044/
            PlayerInput.Instantiate(playerPrefab, controlScheme: PlayerData.playerControlScheme[curPlayerPos], playerIndex: curPlayerPos, pairWithDevices: PlayerData.playerDevices[curPlayerPos]);
            NewPlayer();
            StartCoroutine(InitialCollectableSpawnDelay());
        }

    }
    private IEnumerator InitialCollectableSpawnDelay()
    {
        collectableCanSpawn = false;
        yield return new WaitForSeconds(initialCollectableSpawnDelay);
        collectableCanSpawn = true;
    }


    //~~~~~~~ REFERENCE PLAYER VIA NUMBER ~~~~~~~\\
    void FixedUpdate()
    {
        //update when a player leaves/joins 
        if (prevPlayerPos != curPlayerPos)
        {
            //Debug.Log(curPlayerPos);
            prevPlayerPos = curPlayerPos;
        }

        if (intervalTime >= collectableSpawnInterval) //Collectable spawn timer
        {
            Collectable();
            intervalTime = 0;
        }
        else
        {
            intervalTime += Time.deltaTime;
        }
       
    }



    //~~~~~~~ SPAWN POINTS & ORDER ~~~~~~~\\
    private void SetSpawnPoints()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint"); //fill spawn point array

        //set spawn point order
        //for each spawn point, assign a random number
        //check each previous spawn point and ensure the given number is not the same
        //if it is, reroll the number and restart the duplicate check
        for (int spawnIndex = 0; spawnIndex < spawnOrder.Length; spawnIndex++)
        {
            //Debug.Log("");
            spawnOrder[spawnIndex] = Random.Range(1, spawnPoints.Length);
            //Debug.Log("SpawnOrder " + spawnIndex + " given SpawnPoint " + spawnOrder[spawnIndex]);

            for (int checkIndex = 0; checkIndex < spawnIndex; checkIndex++)
            {
                if (spawnOrder[checkIndex] == spawnOrder[spawnIndex] && checkIndex != spawnIndex)
                {
                    //Debug.Log("SpawnPoint " + spawnOrder[spawnIndex] + " is being used by SpawnOrder " + checkIndex);
                    spawnOrder[spawnIndex] = Random.Range(1, 5);
                    checkIndex = -1;
                    //Debug.Log("SpawnOrder " + spawnIndex + " given SpawnPoint " + spawnOrder[spawnIndex]);
                }
            }
        }

        /*for (int spawnIndex = 0; spawnIndex < spawnOrder.Length; spawnIndex++)
        {
            Debug.Log("order " + spawnIndex + ": " + spawnPoints[spawnOrder[spawnIndex] - 1] + "\tspawn location: " + spawnPoints[spawnOrder[spawnIndex]-1].transform.position);
        }*/
    }



    //~~~~~~~ ADD NEW PLAYER ~~~~~~~\\
    public void NewPlayer()
    {
        //fills the arrays with the applicable player & script
        GameObject newPlayer = GameObject.Find("Player" + curPlayerPos);
        players[curPlayerPos] = newPlayer;
        playerScripts[curPlayerPos] = newPlayer.GetComponent<PlayerController>();
        //Debug.Log("New Player: " + newPlayer.name);

        //apply stats
        ApplyColour();
        ApplyLevelStats();

        if (devMode)
        {
            //Debug.Log("Dev mode for " + newPlayer.name);
            DUIM.EnablePlayer(curPlayerPos, players[curPlayerPos]);
        }
    }


    //~~~~~~~ NEW PLAYER SPECIFICS ~~~~~~~\\
    private void ApplyLevelStats()
    {
        //applies levels stats to new player
        //Debug.Log("applying stats to " + players[PlayerData.numOfPlayers] + ", " + playerScripts[PlayerData.numOfPlayers]);
        playerScripts[curPlayerPos].SetMoveForce(playerMoveForce);
        playerScripts[curPlayerPos].SetJumpForce(playerJumpForce);
        playerScripts[curPlayerPos].SetPlayerGravity(playerGravity);

        playerScripts[curPlayerPos].SetDevMode(devMode);
    }

    private void ApplyColour()
    {
        //Debug.Log("ApplyColour to " + players[curPlayerPos]);
        //old sprite colouring
        //SpriteRenderer playerRend = newPlayer.GetComponent<SpriteRenderer>();
        //Color newColor = new Color(0.5f, 0.5f, 1f, 1f);
        //playerRend.color = Color.red;

        //gives the appropriate colour based on player number
        Light2D playerAuraLight = players[curPlayerPos].GetComponent<Light2D>();
        switch (curPlayerPos)
        {
            case 0:
                playerAuraLight.color = new Color(1f, 0f, 0f, 1f); //red
                break;
            case 1:
                playerAuraLight.color = new Color(0f, 0f, 1f, 1f); //blue
                break;
            case 2:
                playerAuraLight.color = new Color(0f, 1f, 0f, 1f); //green
                break;
            case 3:
                playerAuraLight.color = new Color(0f, 1f, 1f, 1f); //cyan
                break;
            default:
                playerAuraLight.color = new Color(1f, 1f, 1f, 1f); //white, should never appear
                break;
        }
    }
    public void Collectable()
    {
        if (collectableCanSpawn)
        {
            Debug.Log("Spawn Collectable");
            Transform chosenSpawn = ChooseCollectableSpawnPoint().transform; //uses ChooseCollectableSpawnPoint() to choose one collectable spawn in the level
            Vector2 chosenSpawnPos = chosenSpawn.position;
            Quaternion chosenSpawnRot = chosenSpawn.rotation;

            int randomNo = Random.Range(0, collectableType.Length); //rnadomly chooses a number to randomize what collectable we get

            Instantiate(collectableType[randomNo], chosenSpawnPos, chosenSpawnRot);
        }
        
    }

    public GameObject ChooseCollectableSpawnPoint()
    {
        collectableSpawnPoints = GameObject.FindGameObjectsWithTag("CollectableSpawn"); //fill spawn point array

        if (collectableSpawnPoints != null) //chooses one spawn point at random
        {
            int chosenSpawn = Random.Range(0, collectableSpawnPoints.Length);

            return collectableSpawnPoints[chosenSpawn];
        }
        else
        {
            return null ;
        }
    }

    public void InvertControls(GameObject exemptPlayer)
    {
        float invertDuration = 5f;

        Debug.Log("Level script has been called");
        foreach (GameObject player in players)
        {
            if( player != exemptPlayer)
            {
                player.GetComponent<PlayerController>().hasInvertedControls = true;
                InvertDuration(invertDuration);

            }
           
        }
    }
    private IEnumerator InvertDuration(float invertDuration) // for dashDuration we move to IgnoreCollisions layer to dash through players
    {

        yield return new WaitForSeconds(invertDuration);
        UnInvertControls();
    }
    public void UnInvertControls()
    {
        foreach (GameObject player in players)
        {
 
            player.GetComponent<PlayerController>().hasInvertedControls = false;
               
        }

        
    }



    //~~~~~~~ KILL PLAYER ~~~~~~~\\
    public void Kill(GameObject target, GameObject killer)
    {
        //update scores
        int killerPlayerNum = (int)char.GetNumericValue(killer.name[6]);
        PlayerData.playerScores[killerPlayerNum]++;
        Debug.Log(killer.name + " has " + PlayerData.playerScores[killerPlayerNum] + " kills");
        target.GetComponent<PlayerController>().Death();
    }

    public void Respawn(int playerNum, GameObject player, Animator anim)
    {
        //Debug.Log("respawning " + player.name);
        player.transform.position = spawnPoints[spawnOrder[playerNum] - 1].transform.position;
        if (player.transform.position.x < 0) { player.transform.localScale = new Vector2(1, 1); }
        else if (player.transform.position.x > 0) { player.transform.localScale = new Vector2(-1, 1); }
        player.GetComponent<PlayerController>().Respawn();
        anim.ResetTrigger("Dying");
        anim.SetTrigger("Respawning");
    }



    //~~~~~~~ GET CURRENT PLAYER NUMBER ~~~~~~~\\
    public int CurrentPlayer()
    {
        return curPlayerPos;
    }

    //~~~~~~~ GET CURRENT PLAYER SPAWN POINT ~~~~~~~\\
    public Vector2 GetNextSpawnPoint()
    {
        //Debug.Log("player " + curPlayerPos + " spawn point is " + spawnOrder[curPlayerPos] + " at " + spawnPoints[spawnOrder[curPlayerPos]-1].transform.position);
        return spawnPoints[spawnOrder[curPlayerPos]-1].transform.position;
    }

    //~~~~~~~ TIME UP ~~~~~~~\\
    public void TimeUp()
    {
        for(int p = 0; p < PlayerData.numOfPlayers; p++){ PlayerData.timeSinceLastKill[p] = playerScripts[p].GetTimeSinceLastKill(); }
        SceneManager.LoadScene(5);
    }

    public float GetRoundLength()
    {
        return roundMins;
    }
}