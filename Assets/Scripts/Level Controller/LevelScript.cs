using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;

public class LevelScript : MonoBehaviour
{
    //variable player stats
    [SerializeField] [Range(1, 5)] private float playerGravity = 2.5f;
    [SerializeField] [Range(1, 100)] private float playerMoveForce = 25f;
    [SerializeField] [Range(10, 50)] private float playerJumpForce = 25f;

    //debug
    [SerializeField] private bool devMode = false;
    private DebugUIManager DUIM;

    //spawn points
    private GameObject[] spawnPoints;
    private int[] spawnOrder = new int[4];

    //playersInput
    private PlayerController[] playerScripts = new PlayerController[4];
    private PlayerInput[] playersInput = new PlayerInput[4];
    private GameObject[] players = new GameObject[4];
    private int numOfPlayers = 0;
    private int curPlayerPos = 0;
    private int prevPlayerPos = 0;
    private int leavingPlayerNum = 0;

    //important level & multiplayer stuff
    public static LevelScript instance = null;
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;
    [SerializeField] private InputAction joinAction;
    [SerializeField] private InputAction leaveAction;

    //player sprites 
    [SerializeField] private Sprite player1sprite;
    [SerializeField] private Sprite player2sprite;
    [SerializeField] private Sprite player3sprite;
    [SerializeField] private Sprite player4sprite;



    //~~~~~~~ LEVEL BASICS ~~~~~~~\\
    private void Awake()
    {
        //start level music
        FindObjectOfType<AudioManager>().Play("MusicFight");

        //ensure there is only 1 level script
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        //set join/leave actions
        joinAction.Enable();
        joinAction.performed += ctx => JoinAction(ctx);

        leaveAction.Enable();
        leaveAction.performed += ctx => LeaveAction(ctx);

        //set spawn point order
        SetSpawnPoints();


        //update debug
        if (devMode)
        {
            DUIM = GameObject.Find("DebugUI").GetComponent<DebugUIManager>();
        }
    }



    //~~~~~~~ REFERENCE PLAYER VIA NUMBER ~~~~~~~\\
    void Update()
    {
        //update when a player leaves/joins 
        if (prevPlayerPos != curPlayerPos)
        {
            //Debug.Log(curPlayerPos);
            prevPlayerPos = curPlayerPos;
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
    public void NewPlayer(GameObject newPlayer)
    {
        //fills the arrays with the applicable player & script
        //Debug.Log("New Player: " + newPlayer.name);
        players[curPlayerPos] = newPlayer;
        playerScripts[curPlayerPos] = newPlayer.GetComponent<PlayerController>();

        //apply stats
        ApplyColour(newPlayer);
        ApplyLevelStats();

        if (devMode)
        {
            //Debug.Log("Dev mode for " + newPlayer.name);
            DUIM.EnablePlayer(curPlayerPos, newPlayer);
        }
    }



    //~~~~~~~ NEW PLAYER SPECIFICS ~~~~~~~\\
    private void ApplyLevelStats()
    {
        //applies levels stats to new player
        //Debug.Log("applying stats to " + players[numOfPlayers] + ", " + playerScripts[numOfPlayers]);
        playerScripts[curPlayerPos].SetMoveForce(playerMoveForce);
        playerScripts[curPlayerPos].SetJumpForce(playerJumpForce);
        playerScripts[curPlayerPos].SetPlayerGravity(playerGravity);

        playerScripts[curPlayerPos].SetDevMode(devMode);
    }

    private void ApplyColour(GameObject newPlayer)
    {
        //old sprite colouring
        //SpriteRenderer playerRend = newPlayer.GetComponent<SpriteRenderer>();
        //Color newColor = new Color(0.5f, 0.5f, 1f, 1f);
        //playerRend.color = Color.red;

        //gives the appropriate colour based on player number
        Light2D playerAuraLight = newPlayer.GetComponent<Light2D>();
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
                playerAuraLight.color = new Color(1f, 1f, 0f, 1f); //yellow
                break;
            default:
                playerAuraLight.color = new Color(1f, 1f, 1f, 1f); //white, should never appear
                break;
        }
    }





    //~~~~~~~ PLAYER JOINED ~~~~~~~\\
    void JoinAction(InputAction.CallbackContext ctx)
    {
        //joins player as long as there are less than 4 players
        //Debug.Log("JoinAction()");
        if (numOfPlayers < 4)
        {
            PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(ctx);
        }
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        //runs when a player joins
        //Debug.Log("Player joined..");
        if (PlayerJoinedGame != null)
        {
            PlayerJoinedGame(playerInput);
        }

        //finds the lowest empty element in the players array & updates the current player int
        for (int playerCheck = 0; playerCheck < 4; playerCheck++)
        {
            //Debug.Log(playerCheck);
            if (playersInput[playerCheck] == null)
            {
                //Debug.Log("player input " + playerCheck + ": " + playersInput[playerCheck]);
                playersInput[playerCheck] = playerInput;
                curPlayerPos = playerCheck;
                playerCheck = 4;
                //Debug.Log("playersInput: " + playersInput[numOfPlayers]);
            }
        }

        numOfPlayers++; //increase total number of players
    }



    //~~~~~~~ PLAYER LEFT ~~~~~~~\\
    void LeaveAction(InputAction.CallbackContext ctx)
    {
        //leaves player
        //Debug.Log("LeaveAction()");
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        //player leaving code
        //Debug.Log("OnPlayerLeft()");
        //Debug.Log("Player left...");

        //remove targeted player from of array
        playerScripts[leavingPlayerNum] = null;
        playersInput[leavingPlayerNum] = null;
        players[leavingPlayerNum] = null;

        if (devMode)
        {
            DUIM.DisablePlayer(leavingPlayerNum);
        }

        //decrease total number of players
        numOfPlayers--;


        if (PlayerLeftGame != null)
        {
            PlayerLeftGame(playerInput);
        }
    }



    //~~~~~~~ KILL PLAYER ~~~~~~~\\
    public void Kill(GameObject target, GameObject killer)
    {
        //identify the targeted player
        leavingPlayerNum = (int)char.GetNumericValue(target.name[6]);
        target.GetComponent<PlayerController>().Death();
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
}