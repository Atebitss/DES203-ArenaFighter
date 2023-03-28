using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        //set join/leave actions
        joinAction.Enable();
        joinAction.performed += ctx => JoinAction(ctx);

        leaveAction.Enable();
        leaveAction.performed += ctx => LeaveAction(ctx);

        SetSpawnPoints();


        //update debug
        if (devMode)
        {
            DUIM = GameObject.Find("DebugUI").GetComponent<DebugUIManager>();
        }
    }

    void Update()
    {
        if (prevPlayerPos != curPlayerPos)
        {
            Debug.Log(curPlayerPos);
            prevPlayerPos = curPlayerPos;
        }
    }

    private void SetSpawnPoints()
    {
        //fill spawn point array
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //set spawn point order
        //for each spawn point, assign a random number
        //check each previous spawn point and ensure the given number is not the same
        //if it is, reroll the number and restart the duplicate check
        for (int spawnIndex = 0; spawnIndex < spawnOrder.Length; spawnIndex++)
        {
            //Debug.Log("");
            spawnOrder[spawnIndex] = Random.Range(0, 3);

            for (int checkIndex = 0; checkIndex < spawnIndex; checkIndex++)
            {
                //Debug.Log("Checking Location " + checkIndex);
                if (spawnOrder[checkIndex] == spawnOrder[spawnIndex] && checkIndex != spawnIndex)
                {
                    //Debug.Log("Spawn Point " + spawnOrder[spawnIndex] + " is being used by Spawn Location " + checkIndex);
                    spawnOrder[spawnIndex] = Random.Range(0, 4);
                    checkIndex = -1;
                    //Debug.Log("New Spawn Point: " + spawnOrder[spawnIndex]);
                }
            }

            Debug.Log("spawn " + spawnIndex + ", spawn point " + spawnOrder[spawnIndex]);
        }
    }

    private void ApplyLevelStats()
    {
        //applies levels stats to current joining player
        //Debug.Log("applying stats to " + players[numOfPlayers] + ", " + playerScripts[numOfPlayers]);
        playerScripts[curPlayerPos].SetMoveForce(playerMoveForce);
        playerScripts[curPlayerPos].SetJumpForce(playerJumpForce);
        playerScripts[curPlayerPos].SetPlayerGravity(playerGravity);

        playerScripts[curPlayerPos].SetDevMode(devMode);
    }

    private void ApplyColour(GameObject newPlayer)
    {
        SpriteRenderer playerRend = newPlayer.GetComponent<SpriteRenderer>();
        //Color newColor = new Color(0.5f, 0.5f, 1f, 1f);
        //playerRend.color = Color.red;
        switch(curPlayerPos)
        {
            case 0:
                playerRend.sprite = player1sprite;
                break;
            case 1:
                playerRend.sprite = player2sprite;
                break;
            case 2:
                playerRend.sprite = player3sprite;
                break;
            case 3:
                playerRend.sprite = player4sprite;
                break;
            default:
                playerRend.color = Color.magenta;
                break;
        }
    }



    void JoinAction(InputAction.CallbackContext ctx)
    {
        //joins player
        //Debug.Log("JoinAction()");
        if (numOfPlayers < 4)
        {
            PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(ctx);
        }
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        //runs when a player joins
        //Debug.Log("OnPlayerJoined()");
        //Debug.Log("Player joined..");

        if (PlayerJoinedGame != null)
        {
            PlayerJoinedGame(playerInput);
        }

        Debug.Log("New player joining");
        for (int playerCheck = 0; playerCheck < 4; playerCheck++)
        {
            Debug.Log(playerCheck);
            if (playersInput[playerCheck] == null)
            {
                Debug.Log("player input " + playerCheck + ": " + playersInput[playerCheck]);
                playersInput[playerCheck] = playerInput;
                curPlayerPos = playerCheck;
                playerCheck = 4;
                //Debug.Log("playersInput: " + playersInput[numOfPlayers]);
            }
        }

        numOfPlayers++;
    }



    void LeaveAction(InputAction.CallbackContext ctx)
    {
        //leaves player
        Debug.Log("LeaveAction()");
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        //player leaving code
        Debug.Log("OnPlayerLeft()");
        //Debug.Log("Player left...");

        if (PlayerLeftGame != null)
        {
            PlayerLeftGame(playerInput);
        }

        numOfPlayers--;
    }

    public void Kill(GameObject target, GameObject killer)
    {
        string targetName = target.name;
        int playerNum = (int)char.GetNumericValue(targetName[6]);

        //player num out of array
        playerScripts[playerNum] = null;
        playersInput[playerNum] = null;
        players[playerNum] = null;

        if (devMode)
        {
            DUIM.DisablePlayer(playerNum);
        }

        Destroy(target);
    }



    public int CurrentPlayer()
    {
        return curPlayerPos;
    }

    public void NewPlayer(GameObject newPlayer)
    {
        //Debug.Log("New Player()");
        //curPlayerPos = 0;

        /*for (int playerCheck = 0; playerCheck < 4; playerCheck++)
        {
            //Debug.Log(playerCheck);
            if (players[playerCheck] == null)
            {
                Debug.Log("New Player: " + newPlayer.name);
                players[playerCheck] = newPlayer;
                playerScripts[playerCheck] = newPlayer.GetComponent<PlayerController>();
                playerCheck = 4;
                //Debug.Log("playersInput: " + playersInput[numOfPlayers]);
            }
        }*/

        Debug.Log("New Player: " + newPlayer.name);
        players[curPlayerPos] = newPlayer;
        playerScripts[curPlayerPos] = newPlayer.GetComponent<PlayerController>();

        //apply stats
        ApplyColour(newPlayer);
        ApplyLevelStats();

        if (devMode)
        {
            Debug.Log("Dev mode for " + newPlayer.name);
            DUIM.EnablePlayer(curPlayerPos, newPlayer);
        }
    }



    public Vector2 GetNextSpawnPoint()
    {
        return spawnPoints[curPlayerPos].transform.position;
    }
}