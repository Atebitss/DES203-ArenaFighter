using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;

public class LevelScript : MonoBehaviour
{
    //variable player stats
    public CameraShake CameraShake;
    [SerializeField] [Range(1, 100)] private float playerMoveForce = 25f;
    [SerializeField] [Range(10, 50)] private float playerJumpForce = 25f;
    
    //debug
    [Header("Debug")]
    private bool devMode = false;
    private DebugUIManager DUIM;

    //spawn points
    private GameObject[] spawnPoints;
    private int[] spawnOrder = new int[4];
    private int lastUsedSpawn = 0;

    //PlayerData.playerInputs
    private PlayerJoinHandler pjh;
    private PlayerController[] playerScripts = new PlayerController[4];
    private GameObject[] players = new GameObject[4];
    private int curPlayerPos = 0;
    private int prevPlayerPos = 0;

    //intro stuff
    [Header("Intro and Outro")]
    [HideInInspector] public bool introIsOver;
    [HideInInspector] public bool outroIsOver = true;
    [SerializeField] private float introTime = 4;
    [SerializeField] private float outroTime = 4;

    //important level & multiplayer stuff
    [Header("Level and Multiplayer")]
    [SerializeField] private GameObject playerPrefab;
    public static LevelScript instance = null;
    [SerializeField] [Range(1, 5)] private float roundMins = 3;
    private AudioManager AM;
    private bool[] playerWallSliding = new bool[] { false, false, false, false };
    

    //collectable stuff
    [Header("Collectables")]
    [SerializeField] private GameObject[] collectableType;
    private GameObject[] collectableSpawnPoints;
    [SerializeField] private float collectableSpawnInterval;
    [SerializeField] private float initialCollectableSpawnDelay;
    private bool collectableCanSpawn;
    private GameObject lastSpawnedCollectable;
    private float intervalTime;



    //player sprites
    private int[] playerSpriteIDs;
    [SerializeField] private SpriteLibraryAsset player1Sprites;
     [SerializeField] private SpriteLibraryAsset player2Sprites;
     [SerializeField] private SpriteLibraryAsset player3Sprites;
     [SerializeField] private SpriteLibraryAsset player4Sprites;
     [SerializeField] private SpriteLibraryAsset player5Sprites;

    private bool hasBeenShook = false;


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

        if (!PlayerData.GetGameRun()) { PlayerData.SetGameRun(true); }
        devMode = PlayerData.GetDevMode();

        //find audio manager and start level music
        AM = FindObjectOfType<AudioManager>();
        AM.StopPlaying("SpookyNoise");

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

        AM.Play("MusicFight");
        AM.Play("SoundTrees");

        playerSpriteIDs = PlayerData.GetSpriteIDs();    //update sprite ids with previously selected sprites

        for (int player = 0; player < PlayerData.GetNumOfPlayers(); player++)
        {
            curPlayerPos = player;                      //current player ref is the players number (0-3)
            string playerRef = "PlayerJoin" + player;   //reference player
            Destroy(GameObject.Find(playerRef));        //destroy any players found in the level

            //Debug.Log(curPlayerPos);
            //PlayerData.GetPlayers();

            //create new player
            //saviour code from Rene-Damm in the Unity Forum - https://forum.unity.com/threads/local-multiplayer-lobby-scene-gameplay-scene.845044/
            PlayerInput.Instantiate(playerPrefab, controlScheme: PlayerData.playerControlScheme[curPlayerPos], playerIndex: curPlayerPos, pairWithDevices: PlayerData.playerDevices[curPlayerPos]);
            NewPlayer();

            //PlayerData.GetPlayers();

            //begin collectable spawning
            StartCoroutine(InitialCollectableSpawnDelay());
            StartCoroutine(IntroDelay());
        }
    }

    private IEnumerator IntroDelay()
    {
        //Debug.Log("intro delay: " + introTime + ", introIsOver: " + introIsOver);
        introIsOver = false;
        yield return new WaitForSeconds(introTime);
        introIsOver = true;

        if (players != null)
        {
            for (int i = 0; i < PlayerData.GetNumOfPlayers(); i++)
            {
                players[i].GetComponent<PlayerController>().HideArrow();
            }
        }
        //TODO : CHANGE THIS BACK TO BEGIN AT START OF LEVEL ONCE COUNTDOWN IS ADDED TO SONG
        //Debug.Log("introIsOver: " + introIsOver);
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

        //play sound if any player's wall sliding
        PlaySounds();
    }

    private void PlaySounds()
    {
        bool wallSlides = false;

        for(int i = 0; i < PlayerData.GetNumOfPlayers(); i++)
        {
            if (playerScripts[i].OnStickyWall()) 
            { 
                //if (PlayerData.GetDevMode()){ Debug.Log("player" + i + " wall sliding"); } 
                wallSlides = true; 
                break; 
            }
            else { wallSlides = false; }
        }

        if (wallSlides) { AM.PlayOnce("WallSlide"); }
        else { AM.StopPlaying("WallSlide"); }



        bool steps = false;

        for (int i = 0; i < PlayerData.GetNumOfPlayers(); i++)
        {
            if (playerScripts[i].GetPlayerXVelocity() != 0)
            {
                if (PlayerData.GetDevMode()) { Debug.Log("player" + i + " walking"); }
                steps = true;
                break;
            }
            else { steps = false; }
        }

        if (steps) { AM.PlayOnce("Steps"); }
        else { AM.StopPlaying("Steps"); }
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
            //Debug.Log("order " + spawnIndex + ": " + spawnPoints[spawnOrder[spawnIndex] - 1] + "\tspawn location: " + spawnPoints[spawnOrder[spawnIndex]-1].transform.position);
        }*/
    }



    //~~~~~~~ ADD NEW PLAYER ~~~~~~~\\
    public void NewPlayer()
    {
        //reference new player object
        //Debug.Log("LS.NP, curPlayerPos: " + curPlayerPos);
        GameObject newPlayer = GameObject.Find("Player" + curPlayerPos);
        //Debug.Log("New Player: " + newPlayer);

        //fills the arrays with the applicable player & script
        players[curPlayerPos] = newPlayer;
        playerScripts[curPlayerPos] = newPlayer.GetComponent<PlayerController>();

        //update player data with new player
        //Debug.Log("LS.NP, players[cPP]: " + players[curPlayerPos]);
        //Debug.Log("LS.NP, playerScripts[cPP]: " + playerScripts[curPlayerPos]);
        PlayerData.SetPlayers(players[curPlayerPos], curPlayerPos, playerScripts[curPlayerPos]);

        //apply stats
        ApplySprites();
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

        playerScripts[curPlayerPos].SetDevMode(devMode);
    }

    private void ApplySprites()
    {
        //Debug.Log("ApplyColour to " + players[curPlayerPos]);
        //old sprite colouring
        //SpriteRenderer playerRend = newPlayer.GetComponent<SpriteRenderer>();
        //Color newColor = new Color(0.5f, 0.5f, 1f, 1f);
        //playerRend.color = Color.red;

        //gives the appropriate colour based on player number
        Light2D playerAuraLight = players[curPlayerPos].transform.Find("Light").GetComponent<Light2D>();
        ParticleSystem.MainModule playerAuraParticles = playerAuraLight.GetComponent <ParticleSystem>().main;
        SpriteLibrary spriteLibary = players[curPlayerPos].GetComponent<SpriteLibrary>();
        switch (playerSpriteIDs[curPlayerPos])
        {
            case 0:
                playerAuraLight.color = new Color(1f, 0f, 0f, 1f); //red
                playerAuraParticles.startColor = new Color(1f, 0f, 0f, 1f); //red
                spriteLibary.spriteLibraryAsset =  player1Sprites;
                break;
            case 1:
                playerAuraLight.color = new Color(0f, 1f, 0f, 1f); //green
                playerAuraParticles.startColor = new Color(0f, 1f, 0f, 1f);
                spriteLibary.spriteLibraryAsset = player2Sprites;
                break;
            case 2:
                playerAuraLight.color = new Color(1f, 0f, 1f, 1f); //pink
                playerAuraParticles.startColor = new Color(1f, 0f, 1f, 1f);
                spriteLibary.spriteLibraryAsset = player3Sprites;
                break;
            case 3:
                playerAuraLight.color = new Color(0.5f, 1f, 0.75f, 1f); //teal
                playerAuraParticles.startColor = new Color(0.5f, 1f, 0.75f, 1f);
                spriteLibary.spriteLibraryAsset = player4Sprites;
                break;
            case 4:
                playerAuraLight.color = new Color(0.75f, 0f, 1f, 1f); //purple
                playerAuraParticles.startColor = new Color(0.75f, 0f, 1f, 1f);
                spriteLibary.spriteLibraryAsset = player5Sprites;
                break;
            default:
                playerAuraLight.color = new Color(1f, 1f, 1f, 1f); //white, should never appear
                spriteLibary.spriteLibraryAsset = player1Sprites;
                break;
        }
    }
    public void Collectable()
    {
        if (collectableCanSpawn && lastSpawnedCollectable == null )
        {
            //add a public bool method to check if any collectables exist in the level already
           Debug.Log("Collectable Spawned");
            Transform chosenSpawn = ChooseCollectableSpawnPoint().transform; //uses ChooseCollectableSpawnPoint() to choose one collectable spawn in the level
            Vector2 chosenSpawnPos = chosenSpawn.position;
            Quaternion chosenSpawnRot = chosenSpawn.rotation;

            int randomNo = Random.Range(0, collectableType.Length); //rnadomly chooses a number to randomize what collectable we get

            lastSpawnedCollectable = Instantiate(collectableType[randomNo], chosenSpawnPos, chosenSpawnRot);

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
            return null;
        }
    }

    public void InvertControls(GameObject exemptPlayer)
    {
        float invertDuration = 5f;

        //Debug.Log("Level script has been called");
        if (players != null)
        {
            foreach (GameObject player in players)
            {
                if (player != exemptPlayer)
                {
                    player.GetComponent<PlayerController>().InvertControls();
                    ShakeCamera(0.4f, 0.16f);
                    Invoke(nameof(UnInvertControls), invertDuration);
                }

            }
        }
      
    }
    private IEnumerator InvertDuration(float invertDuration) 
    {

        yield return new WaitForSeconds(invertDuration);

    }
    public void UnInvertControls()
    {
        if (players != null)
        {
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerController>().UnInvertControls();
            }
        }
    }



    //~~~~~~~ KILL PLAYER ~~~~~~~\\
    public void Kill(GameObject target, GameObject killer)
    {
        //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        //Debug.Log("Target: " + target.name + "   Killer: " + killer.name);
        PlayerController targetPC = target.GetComponent<PlayerController>();
        PlayerController killerPC = killer.GetComponent<PlayerController>();

        if ((targetPC.GetIsInvincible() == false) && !targetPC.GetIsDying())
        {
            //update killer info
            int killerNum = (int)char.GetNumericValue(killer.name[6]);
            float tslk = killerPC.GetTimeSinceLastKill();

            //update killer score
            killerPC.IncScore();
            killerPC.ResetTimeSinceLastKill();
            //Debug.Log(target);
            //kill target
            targetPC.Death();

            //update and then sort player scores
            PlayerData.UpdateScores();
            PlayerData.SortPlayers();
        }

        for (int i = 0; i < PlayerData.GetNumOfPlayers(); i++)
        {
            /*//Debug.Log("~~~~~~~" + 
                "   " + players[i].name +
                "   kills: " + PlayerData.playerScores[i] +
                "   tslk: " + PlayerData.playerTSLK[i] + 
                "   podium: " + PlayerData.playerPositions[i] +
                "   script: " + playerScripts[i]);

            //Debug.Log("Position " + i + ": Player " + (PlayerData.playerPositions[i]-1) + " with score " + PlayerData.playerScores[i]);
            //Debug.Log("player script in ref to podium pos: " + playerScripts[PlayerData.playerPositions[i]-1]);*/

            //Debug.Log("i: " + i);
            //Debug.Log("PD.playerPositions[i]: " + PlayerData.playerPositions[i]);
            //Debug.Log("playerScripts[PD.playerPositions[i]]: " + playerScripts[PlayerData.playerPositions[i]]);
            if (i == 0 && !playerScripts[PlayerData.playerPositions[i]].GetCrowned())
            {
                playerScripts[PlayerData.playerPositions[i]].EnableCrown();
                //Debug.Log("Player " + PlayerData.playerPositions[i] + " has taken the lead!");
            }
            else if (i != 0 && playerScripts[PlayerData.playerPositions[i]].GetCrowned())
            {
                playerScripts[PlayerData.playerPositions[i]].DisableCrown();
                //Debug.Log("Player " + PlayerData.playerPositions[i] + " has lost the lead!");
            }
        }
    }

    public void Respawn(int playerNum, GameObject player, Animator anim)
    {
        PlayerController playerPC = player.GetComponent<PlayerController>();
        //Debug.Log("respawning " + player.name);

        //spawnpoint 1 pos: -9,13
        //spawnpoint 2 pos: 7,13
        //spawnpoint 3 pos: -9,6 but spawns on sp1
        //spawnpoint 4 pos: 7,6 but bugs out
        int curSpawnPoint = Random.Range(0, 4);
        while (curSpawnPoint == lastUsedSpawn) { curSpawnPoint = Random.Range(0, 4); }
        lastUsedSpawn = curSpawnPoint;

        //Debug.Log("respawn num: " + curSpawnPoint);
        //Debug.Log("curspawn: " + spawnPoints[curSpawnPoint]);
        //Debug.Log("curspawn pos: " + spawnPoints[curSpawnPoint].transform.position);

        player.transform.position = spawnPoints[curSpawnPoint].transform.position;

        if (player.transform.position.x < 0) { player.transform.localScale = new Vector2(1, 1); }
        else if (player.transform.position.x > 0) { player.transform.localScale = new Vector2(-1, 1); }

        playerPC.Respawn();
        anim.SetTrigger("Respawning");
        playerPC.SetIsDying(false);
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(CameraShake.Shake(duration, magnitude));
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
        return spawnPoints[spawnOrder[curPlayerPos] - 1].transform.position;
    }

    //~~~~~~~ TIME UP ~~~~~~~\\
    public void TimeUp()
    {
        for (int p = 0; p < PlayerData.GetNumOfPlayers(); p++) { PlayerData.playerTSLKs[p] = playerScripts[PlayerData.playerPositions[p]].GetTimeSinceLastKill(); }
        PlayerData.SortPlayers();
        StartCoroutine(OutroDelay());
    }
    private IEnumerator OutroDelay()
    {
        if (!hasBeenShook)
        {
            ShakeCamera(0.4f, 0.2f);
            hasBeenShook = true;
        }
       
        outroIsOver = false;
        yield return new WaitForSeconds(outroTime);
        outroIsOver = true;
        SceneManager.LoadScene(5);
    }
   

    //public void UpdatePlayerWallSliding(bool sliding, int playerNum) { if (PlayerData.GetDevMode()) { Debug.Log("player" + playerNum + " sliding"); } playerWallSliding[playerNum] = sliding; }

    public float GetRoundLength()
    {
        return roundMins;
    }
}