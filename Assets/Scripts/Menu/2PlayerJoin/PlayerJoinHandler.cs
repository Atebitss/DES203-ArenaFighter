using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerJoinHandler : MonoBehaviour
{
    //actions
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;
    [SerializeField] private GameObject playerJoinMenuController;
    [SerializeField] private InputAction joinAction;
    [SerializeField] private InputAction leaveAction;
    [SerializeField] private InputAction startAction;

    //header animation
    private Animator[] headerAnimators = new Animator[4];
    private bool[] headersPlaying = new bool[4];

    //player 
    private int curPlayerPos;
    private InputDevice curPlayerDevice;
    private GameObject[] selectorPrefabs = new GameObject[4];

    //refs
    public static PlayerJoinHandler instance = null;
    private CharacterSelectHandler csh;
    private GameObject[] pressA = new GameObject[4];


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

        //Debug.Log("PlayerJoinHandler Awake()");

        //set join/leave actions
        joinAction.performed += ctx => JoinAction(ctx);
        leaveAction.performed += ctx => LeaveAction(ctx);
        startAction.performed += ctx => StartAction(ctx);

        joinAction.Enable();
        leaveAction.Enable();
        startAction.Enable();


        for (int i = 0; i < headerAnimators.Length; i++)
        {
            headerAnimators[i] = GameObject.Find("Header" + (i + 1)).GetComponent<Animator>();
            pressA[i] = GameObject.Find("A" + (i+1));
        }

        csh = GameObject.Find("CharacterSelectManager").GetComponent<CharacterSelectHandler>();
    }

    void FixedUpdate() { Debug.Log("cPP: " + curPlayerPos); }


    //~~~~~~~ PLAYER JOINED ~~~~~~~\\
    void JoinAction(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Join action attempt");
        //joins player as long as there are less than 4 players
        if (SceneManager.GetActiveScene().name == "2PlayerJoin" && PlayerData.GetNumOfPlayers() < 4)
        {
            Debug.Log("JoinAction()");
            Debug.Log("device attempting join: " + ctx.control.device);
            for (int playerCheck = 0; playerCheck < 4; playerCheck++)
            {
                if (PlayerData.playerInputs[playerCheck] != null && PlayerData.playerDevices[playerCheck].Equals(ctx.control.device)) { Debug.Log("device already found: " + ctx.control.device); break; }
                if (PlayerData.playerInputs[playerCheck] == null)
                {
                    Debug.Log("player input " + playerCheck + " empty");

                    curPlayerPos = playerCheck;
                    PlayerData.playerDevices[curPlayerPos] = ctx.control.device;
                    //if (!PlayerData.gameRun) { curPlayerPos = playerCheck; }
                    //else if (PlayerData.gameRun) { curPlayerPos = playerCheck-1; }

                    playerCheck = 4;
                    break;
                }
            }

            PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(ctx);
        }
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        if (SceneManager.GetActiveScene().name == "2PlayerJoin")
        {
            curPlayerPos = playerInput.playerIndex;
            //runs when a player joins
            Debug.Log("Player " + curPlayerPos + " joined..");
            //Debug.Log("Input: " + playerInput);

            if (PlayerJoinedGame != null)
            {
                PlayerJoinedGame(playerInput);
            }

            selectorPrefabs[curPlayerPos] = playerInput.gameObject;

            pressA[curPlayerPos].SetActive(false);

            //finds the lowest empty element in the players array & updates the current player int
            PlayerData.playerInputs[curPlayerPos] = playerInput;
            PlayerData.playerControlScheme[curPlayerPos] = playerInput.currentControlScheme;

            //Debug.Log(playerInput + ": " + PlayerData.playerInputs[curPlayerPos]);
            //Debug.Log(playerInput.currentControlScheme + ": " + PlayerData.playerControlScheme[curPlayerPos]);

            //Debug.Log("increasing player count");
            PlayerData.SetNumOfPlayers((PlayerData.GetNumOfPlayers()+1)); //increase total number of players

            headersPlaying[curPlayerPos] = true;
            headerAnimators[curPlayerPos].SetBool("playing", true);

            //Debug.Log("playerjoinhandler onplayerjoin curpos: " + curPlayerPos);
            //Debug.Log(PlayerData.playerDevices[curPlayerPos].name);
            //Debug.Log(this.gameObject.GetComponent<HapticController>());
            if (!PlayerData.playerDevices[curPlayerPos].name.Equals("Keyboard")) { this.gameObject.GetComponent<HapticController>().PlayHaptics("Rumble", (Gamepad)PlayerData.playerDevices[curPlayerPos]); }


            //update name and position in heirarchy
            if (GameObject.Find("CharacterSelector" + curPlayerPos)) { Debug.Log("duplicate selectors"); }
            selectorPrefabs[curPlayerPos].transform.name = "CharacterSelector" + curPlayerPos;
            selectorPrefabs[curPlayerPos].transform.SetParent(GameObject.Find("Box" + (curPlayerPos + 1)).transform, false);
            Debug.Log(selectorPrefabs[curPlayerPos]);
            selectorPrefabs[curPlayerPos].GetComponent<CharacterSelectorScript>().Wake(curPlayerPos);


            StartCoroutine(AnimDelay(curPlayerPos));
        }
    }

    private IEnumerator AnimDelay(int curPlayerPos)
    {
        yield return new WaitForSeconds(0.2f);
        headersPlaying[curPlayerPos] = false;
        headerAnimators[curPlayerPos].SetBool("playing", false);
    }


    //~~~~~~~ PLAYER LEFT ~~~~~~~\\
    void LeaveAction(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "2PlayerJoin")
        {
            //leaves player
            //Debug.Log("LeaveAction()");

            if (PlayerData.GetNumOfPlayers() > 0)
            {
                for (int player = 0; player < PlayerData.playerInputs.Length; player++)
                {
                    foreach (var device in PlayerData.playerInputs[player].devices)
                    {
                        if (device != null && ctx.control.device == device)
                        {
                            //Debug.Log("running unregister");
                            UnregisterPlayer(PlayerData.playerInputs[player]);

                            //decrease total number of players
                            //Debug.Log("lowering player count");
                            PlayerData.SetNumOfPlayers((PlayerData.GetNumOfPlayers()-1));

                            int playerNum = (int)char.GetNumericValue(PlayerData.playerInputs[player].name[10]) + 1;
                            //Debug.Log(playerNum);
                            csh.GetCSS(playerNum).ResetImage();     
                            pressA[playerNum].SetActive(true);

                            //Debug.Log("removing " + PlayerData.playerInputs[player] + " from list");
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
        //Debug.Log("Unregistering " + playerInput.transform.gameObject.name);

        if (PlayerLeftGame != null)
        {
            //Debug.Log("leaving " + playerInput);
            PlayerLeftGame(playerInput);
        }

        //Debug.Log("destroying " + playerInput);
        Destroy(playerInput.transform.gameObject);
    }



    void StartAction(InputAction.CallbackContext ctx)
    {
        if (SceneManager.GetActiveScene().name == "2PlayerJoin" && playerJoinMenuController != null)
        {
            //Debug.Log("Start action");
            if (PlayerData.GetNumOfPlayers() >= 2 || PlayerData.GetDevMode() && PlayerData.GetNumOfPlayers() > 0)
            {
                //Debug.Log("Enough player to start");
                playerJoinMenuController.GetComponent<PlayerJoinMenuController>().StartGame();
            }
            else
            {
                //Debug.Log("Not enough players to start");
            }
        }
    }


    //~~~~~~~ GET CURRENT PLAYER NUMBER ~~~~~~~\\
    public int CurrentPlayer()
    {
        if (PlayerData.GetDevMode()) { Debug.Log("cPP: " + curPlayerPos); }
        return curPlayerPos;
    }
}
