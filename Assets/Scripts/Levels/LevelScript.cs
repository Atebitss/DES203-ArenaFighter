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

    //important level & multiplayer stuff
    public static LevelScript instance = null;
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;
    [SerializeField] private InputAction joinAction;
    [SerializeField] private InputAction leaveAction;


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

    private void ApplyLevelStats(int playerPos)
    {
        //applies levels stats to current joining player
        //Debug.Log("applying stats to " + players[numOfPlayers] + ", " + playerScripts[numOfPlayers]);
        playerScripts[playerPos].SetMoveForce(playerMoveForce);
        playerScripts[playerPos].SetJumpForce(playerJumpForce);
        playerScripts[playerPos].SetPlayerGravity(playerGravity);

        playerScripts[playerPos].SetDevMode(devMode);
    }

    private void ApplyColour(GameObject newPlayer)
    {
        SpriteRenderer playerRend = newPlayer.GetComponent<SpriteRenderer>();
        //Color newColor = new Color(0.5f, 0.5f, 1f, 1f);
        //playerRend.color = Color.red;
        switch(numOfPlayers-1)
        {
            case 0:
                playerRend.color = Color.red;
                break;
            case 1:
                playerRend.color = Color.blue;
                break;
            case 2:
                playerRend.color = Color.green;
                break;
            case 3:
                playerRend.color = Color.black;
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
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(ctx);
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

        for (int playerCheck = 0; playerCheck < 4; playerCheck++)
        {
            Debug.Log(playerCheck);
            if (playersInput[playerCheck] == null)
            {
                Debug.Log(playersInput[playerCheck]);
                playersInput[playerCheck] = playerInput;
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



    public int CurrentPlayers()
    {
        return numOfPlayers;
    }

    public void NewPlayer(GameObject newPlayer)
    {
        //Debug.Log("New Player()");
        int playerPos = 0;

        for (int playerCheck = 0; playerCheck < 4; playerCheck++)
        {
            if (players[playerCheck] == null)
            {
                Debug.Log("New Player" + playerCheck);
                players[playerCheck] = newPlayer;
                playerScripts[playerCheck] = newPlayer.GetComponent<PlayerController>();
                playerPos = playerCheck;
                playerCheck = 4;
                //Debug.Log("playersInput: " + playersInput[numOfPlayers]);
            }
        }

        //apply stats
        ApplyColour(newPlayer);
        ApplyLevelStats(playerPos);

        if (devMode)
        {
            DUIM.EnablePlayer(playerPos, newPlayer);
        }
    }



    public Vector2 GetNextSpawnPoint()
    {
        return spawnPoints[numOfPlayers].transform.position;
    }
}