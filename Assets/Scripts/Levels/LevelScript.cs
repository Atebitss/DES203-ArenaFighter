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
    private int[] spawnOrder = new int[3];

    //playersInput
    /*public List<PlayerController> playerScripts = new List<PlayerController>();
    public List<PlayerInput> playersInput = new List<PlayerInput>();
    public List<GameObject> players = new List<GameObject>();*/

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
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(gameObject);
        }

        //set join/leave actions
        joinAction.Enable();
        joinAction.performed += ctx => JoinAction(ctx);

        leaveAction.Enable();
        leaveAction.performed += ctx => LeaveAction(ctx);


        //fill spawn point array
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //set spawn point order
        /*for (int spawnIndex = 0; spawnIndex < spawnOrder.Length; spawnIndex++)
        {
            Debug.Log("Index: " + spawnIndex);
            spawnOrder[spawnIndex] = Random.Range(0, 3);
            Debug.Log("Order Index " + spawnIndex + ": " + spawnOrder[spawnIndex]);

            for (int checkIndex = 0; checkIndex < spawnOrder.Length; checkIndex++)
            {
                if(spawnOrder[checkIndex] == spawnOrder[spawnIndex] && checkIndex != spawnIndex)
                {
                    Debug.Log("Order Index " + spawnIndex + " is the same as Order Index " + spawnOrder[checkIndex]);
                    spawnOrder[spawnIndex] = Random.Range(0, 3);
                    Debug.Log("New Order Index " + spawnIndex + ": " + spawnOrder[spawnIndex]);
                }
            }
        }*/


        //update debug
        if(devMode)
        {
            DUIM = GameObject.Find("DebugUI").GetComponent<DebugUIManager>();
        }
    }

    void Update()
    {
    }

    private void ApplyLevelStats()
    {
        //applies levels stats to current joining player
        //Debug.Log("applying stats to " + players[numOfPlayers] + ", " + playerScripts[numOfPlayers]);
        playerScripts[numOfPlayers].SetMoveForce(playerMoveForce);
        playerScripts[numOfPlayers].SetJumpForce(playerJumpForce);
        playerScripts[numOfPlayers].SetPlayerGravity(playerGravity);
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

        playersInput[numOfPlayers] = playerInput;
        //Debug.Log("playersInput: " + playersInput[numOfPlayers]);

        numOfPlayers++;
    }



    void LeaveAction(InputAction.CallbackContext ctx)
    {
        //leaves player
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        //player leaving code
        //Debug.Log("Player left...");
        numOfPlayers--;
    }



    public int CurrentPlayers()
    {
        return numOfPlayers;
    }

    public void NewPlayer(GameObject newPlayer)
    {
        //Debug.Log("New Player()");
        players[numOfPlayers] = newPlayer;
        //Debug.Log("players: " + numOfPlayers + " - " + players[numOfPlayers]);

        playerScripts[numOfPlayers] = newPlayer.GetComponent<PlayerController>();
        //Debug.Log("playerScripts: " + playerScripts[numOfPlayers]);

        ApplyLevelStats();

        if (devMode)
        {
            DUIM.EnablePlayer(numOfPlayers, players[numOfPlayers]);
        }
    }



    public GameObject[] GetSpawnPoints()
    {
        return spawnPoints;
    }

    public GameObject GetSpawnPoint(int i)
    {
        return spawnPoints[i];
    }
}