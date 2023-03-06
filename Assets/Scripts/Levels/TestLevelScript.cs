using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestLevelScript : MonoBehaviour
{
    //variable player stats for different levels
    [SerializeField] [Range(1, 5)] private float playerGravity = 2.5f;
    [SerializeField] [Range(1, 25)] private float playerMoveForce = 25f;
    [SerializeField] [Range(10, 50)] private float playerJumpForce = 25f;
    private int[] spawnOrder = new int[3];
    private PlayerController[] playerScripts;
    private GameObject activePlayer;
    private GameObject[] spawnPoints;

    public List<PlayerInput> players = new List<PlayerInput>();

    public static TestLevelScript instance = null;

    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

    [SerializeField] private InputAction joinAction;
    [SerializeField] private InputAction leaveAction;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != null)
        {
            Destroy(gameObject);
        }

        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        joinAction.Enable();
        joinAction.performed += ctx => JoinAction(ctx);

        leaveAction.Enable();
        leaveAction.performed += ctx => LeaveAction(ctx);


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
    }

    private void Start()
    {
        //PlayerInputManager.instance.JoinPlayer(0, -1, null);
    }

    void Update()
    {
        activePlayer = GameObject.FindWithTag("Player");

        /*if (activePlayer != null)
        {
            playerScripts[] = activePlayer.GetComponent<PlayerController>();
        }

        if (playerScripts[] != null)
        {
            ApplyLevelStats();
        }*/
    }

    private void ApplyLevelStats(PlayerController playerController)
    {
        playerController.SetMoveForce(playerMoveForce);
        playerController.SetJumpForce(playerJumpForce);
        playerController.SetPlayerGravity(playerGravity);
    }


    void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player joined..");
        players.Add(playerInput);
        
        //only apply to newest joined player
        ApplyLevelStats(playerInput.transform.parent.gameObject.GetComponent<PlayerController>());

        if (PlayerJoinedGame != null)
        {
            PlayerJoinedGame(playerInput);
        }
    }

    void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("Player left...");
    }


    public GameObject[] GetSpawnPoints()
    {
        return spawnPoints;
    }

    public GameObject GetSpawnPoint(int i)
    {
        return spawnPoints[i];
    }


    void JoinAction(InputAction.CallbackContext ctx)
    {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(ctx);
    }

    void LeaveAction(InputAction.CallbackContext ctx)
    {

    }
}