using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerJoinHandler : MonoBehaviour
{
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;
    [SerializeField] private InputAction joinAction;
    [SerializeField] private InputAction leaveAction;
    [SerializeField] private InputAction startAction;

    private int curPlayerPos;
    private InputAction.CallbackContext context;
    private InputDevice curPlayerDevice;

    public static PlayerJoinHandler instance = null;


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

        Debug.Log("PlayerJoinHandler Awake()");

        //set join/leave actions
        joinAction.performed += ctx => JoinAction(ctx);
        leaveAction.performed += ctx => LeaveAction(ctx);
        startAction.performed += ctx => StartAction(ctx);

        joinAction.Enable();
        leaveAction.Enable();
        startAction.Enable();
    }


    //~~~~~~~ PLAYER JOINED ~~~~~~~\\
    void JoinAction(InputAction.CallbackContext ctx)
    {
        Debug.Log("JoinAction()");
        if (SceneManager.GetActiveScene().name == "PlayerJoin")
        {
            //joins player as long as there are less than 4 players
            if (PlayerData.numOfPlayers < 4)
            {
                for (int playerCheck = 0; playerCheck < 4; playerCheck++)
                {
                    Debug.Log(PlayerData.playerInputs[playerCheck]);
                    if (PlayerData.playerInputs[playerCheck] == null)
                    {
                        curPlayerPos = playerCheck;
                        context = ctx;
                        PlayerData.playerDevices[curPlayerPos] = ctx.control.device;
                        //if (!PlayerData.gameRun) { curPlayerPos = playerCheck; }
                        //else if (PlayerData.gameRun) { curPlayerPos = playerCheck-1; }

                        Debug.Log("player input " + curPlayerPos + " empty");
                        playerCheck = 4;
                    }
                }

                PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(ctx);
            }
        }
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        //runs when a player joins
        Debug.Log("Player " + curPlayerPos + " joined..");
        Debug.Log("Input: " + playerInput);

        if (PlayerJoinedGame != null)
        {
            PlayerJoinedGame(playerInput);
        }

        //finds the lowest empty element in the players array & updates the current player int
        PlayerData.playerInputs[curPlayerPos] = playerInput;
        PlayerData.playerControlScheme[curPlayerPos] = playerInput.currentControlScheme;

        //Debug.Log("increasing player count");
        PlayerData.numOfPlayers++; //increase total number of players
        PlayerData.GetPlayers();

        string findRef = "Image" + (curPlayerPos+1);
        Debug.Log(findRef);
        GameObject.Find(findRef).GetComponent<ChangeImage>().ImageChange();
    }


    //~~~~~~~ PLAYER LEFT ~~~~~~~\\
    void LeaveAction(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "PlayerJoin")
        {
            //leaves player
            Debug.Log("LeaveAction()");

            if (PlayerData.numOfPlayers > 0)
            {
                for (int player = 0; player < PlayerData.playerInputs.Length; player++)
                {
                    foreach (var device in PlayerData.playerInputs[player].devices)
                    {
                        if (device != null && ctx.control.device == device)
                        {
                            Debug.Log("running unregister");
                            UnregisterPlayer(PlayerData.playerInputs[player]);

                            //decrease total number of players
                            //Debug.Log("lowering player count");
                            PlayerData.numOfPlayers--;

                            int playerNum = (int)char.GetNumericValue(PlayerData.playerInputs[player].name[10]) + 1;
                            string findRef = "Image" + playerNum;
                            //Debug.Log(playerNum);
                            GameObject.Find(findRef).GetComponent<ChangeImage>().ResetImage();

                            Debug.Log("removing " + PlayerData.playerInputs[player] + " from list");
                            PlayerData.playerInputs[player] = null;
                            PlayerData.playerDevices[player] = null;
                            PlayerData.playerControlScheme[player] = null;

                            PlayerData.GetPlayers();
                            return;
                        }
                    }
                }
            }
        }
    }

    private void UnregisterPlayer(PlayerInput playerInput)
    {
        Debug.Log("Unregistering " + playerInput.transform.gameObject.name);

        if (PlayerLeftGame != null)
        {
            //Debug.Log("leaving " + playerInput);
            PlayerLeftGame(playerInput);
        }

        Debug.Log("destroying " + playerInput);
        Destroy(playerInput.transform.gameObject);
    }



    void StartAction(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "PlayerJoin")
        {
            Debug.Log("Start action");
            if (PlayerData.numOfPlayers >= 2)
            {
                Debug.Log("Enough player to start");
                GameObject.Find("PlayerJoinMenu").GetComponent<PlayerJoinMenuController>().StartGame();
            }
            else
            {
                Debug.Log("Not enough players to start");
            }
        }
    }


    //~~~~~~~ GET CURRENT PLAYER NUMBER ~~~~~~~\\
    public int CurrentPlayer()
    {
        return curPlayerPos;
    }
}
