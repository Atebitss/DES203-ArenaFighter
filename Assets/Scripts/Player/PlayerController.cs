using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //~~~~~~~REFRENCES ~~~~~~~\\
    [Header("REFRENCES")]
    [Header("Player Components")]
    [SerializeField] private Rigidbody2D playerRigid;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    
    [Header("Prefab Components")]
    [SerializeField] private GameObject playerTop;
    [SerializeField] private PlayerLight playerLight;
    [SerializeField] private ButtonPress buttonPress;
    [SerializeField] private GameObject crown;
    [SerializeField] private Transform deflectRef;
    [SerializeField] private PlayerArrows playerArrow;
    [SerializeField] private IceBlock iceBlock;

    private Gamepad controller;
    [HideInInspector] public int playerNum;
    private LevelScript ls;
    private GameObject vfxController;


    //~~~~~~~ GAMEPLAY ~~~~~~~\\
    //~~~ MOVEMENT ~~~\\

    private Vector2 move;
    private Vector2 playerVelocity;
    private bool onGround, devMode;
    private float jumpForce = 15f, moveForce = 5f, previousXMovement;
    private bool isRunning;
    private bool wasInAir;
    private bool isHoldingDown;



    [Header("MOVEMENT AND GAMEPLAY")]
    //~~~ JUMPING ~~~\\
    [Header("Jumping")]
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteCounter;

    private float jumpBufferCounter;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [SerializeField] private float playerGravity = 5.5f;
    [SerializeField] private float fallGravityMult = 1.4f;
    [SerializeField] private float midAirMoveMultiplier = 0.5f;
    [SerializeField] private float maxFallSpeed;
    private bool isFalling;
    private bool isLanding;

    private bool isJumping;
    private bool topTrigger;


    //~~~ DASHING ~~~\\
    [Header("Dashing")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0f;
    [SerializeField] private float dashCooldownTime = 0.8f;
    [SerializeField] private bool dashEnabled;
    private float dashCooldown;
    private bool isDashing;


    //~~~ ICE ~~~\\
    [Header("Ice Movement")]
    [SerializeField] private float IceDecceleration = 0.95f; //must be between 1 and 0
    [SerializeField] private float iceSpeed = 9f;
    private bool onIce;


    //~~~ WALL SLIDE & JUMP ~~~\\
    [Header("Wall Jumps, Climbs and Slides")]
    [SerializeField] private float wallSlideSpeed = -2f;
    [SerializeField] private float wallClimbX = 4f;
    [SerializeField] private float wallClimbY = 16f;
    [SerializeField] private float wallKickX = 4f;
    [SerializeField] private float wallKickY = 16f;
    [SerializeField] private float wallCoyoteTime;
    private float wallCoyoteCounter;
    private float wallJumpCooldown;
    private bool onStickyWall;
    private bool isWallJumping;
    private float wallJumpingDirection;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private float wallClimbingDuration = 0.1f;

    //~~~ BOUNCE ~~~\\
    [Header("Bouncing")]
    [SerializeField] private float bounceMultiplier = 1.5f;
    [SerializeField] private float bounceRebound = 1.25f;
    private bool onBouncy;

    //~~~ TELEPORT ~~~\\
    [Header("Teleport")]
    [SerializeField] private float teleportingDuration = 0.2f;
    private GameObject currentTeleporter;
    private bool isTeleporting;

    //~~~ POWERUPS ~~~~//
    [Header("PowerUps")]
    public Sprite[] floatingPrompts;

    private bool hasIcePower = false;
    private bool frozen = false;
    private int breakAmount;
    private int breakCounter;
    [SerializeField] private float stunTime = 1f;


    private bool hasInvertPower = false;
    [HideInInspector]public bool hasInvertedControls = false;

    private bool hasDashPower = false;

   

    //~~~ FLIP ~~~\\
    private bool facingRight;
    private Vector3 startingScale = new Vector3(4, 4, 1);


    //~~~ COMBAT ~~~\\
    [Header("Combat")]
    [SerializeField] private float deflectDuration = 0.5f;
    [SerializeField] private float deflectForce = 60f;
    [SerializeField] private float attackBuildUp = 0.0f;
    [SerializeField][Range(0.1f, 0.5f)] private float attackTimer = 0.2f;

    private GameObject attackObject;
    private bool isDeflecting, isAttacking, isDying, crowned;
    private int score = 0;
    private float timeSinceLastKill = 0;

    //~~~ MISC ~~~\\
    [Header("Invincibility")]
    [SerializeField] private Material originMat;
    [SerializeField] private Material invincibilityMat;
    [SerializeField] private float invincibilityTime = 1f;
    private bool invincible;

    private AudioManager AM;



    void Awake()
    {
        //reference control scripts
        ls = GameObject.Find("LevelController").GetComponent<LevelScript>();
        vfxController = GameObject.Find("VFXController");
        AM = FindObjectOfType<AudioManager>();

        //set player name
        //Debug.Log("PC.A, playerNum: " + ls.CurrentPlayer());
        playerNum = ls.CurrentPlayer();
        gameObject.name = "Player" + playerNum;

        onStickyWall = false;

         //player animator
        animator = GetComponent<Animator>();
        //animator.SetInteger("PlayerNum", playerNum);

        //reference for haptics
        string inputDevice = PlayerData.playerDevices[playerNum].name;
        //Debug.Log(this.gameObject.name + ", " + inputDevice);
        if (!inputDevice.Equals("Keyboard")) { controller = (Gamepad)PlayerData.playerDevices[playerNum]; }
        else { controller = null; }

     

        //hide crown
        DisableCrown();
        playerArrow.UpdateArrow(playerNum);
    }
    private void Start()
    {
        FaceTowardCenter();
    }  

    void FaceTowardCenter()
    {
        if (transform.position.x > 0)
        {
            transform.localScale = new Vector3(-startingScale.x, startingScale.y, startingScale.z);
        }
        else
        {
            transform.localScale = startingScale;
        }
    }
      

    void FixedUpdate()
    {
        MiscAdjustments();
        PlayerMovement();
        Flip();
        IceMovement();
        WallSlide();
        BounceMovement();
  
        if (devMode) { HighlightHitboxes(); }

    }
    public void MiscAdjustments()
    {
       
        if (!ls.introIsOver || !ls.outroIsOver || isDying || invincible) //freeze player movement while level intro/outro is playing OR WHEN PLAYER IS DYING 
        {
            playerRigid.constraints = RigidbodyConstraints2D.FreezeAll;

            playerRigid.isKinematic = true;
        }
        else if (!frozen)
        {
            playerRigid.constraints = ~RigidbodyConstraints2D.FreezePosition;
            playerRigid.isKinematic = false;
        }

        //Gravity tweaking, we fall faster when we start falling in our jump
        if (playerRigid.velocity.y < 0 && !OnStickyWall())
        {
            playerRigid.gravityScale = playerGravity * fallGravityMult;
        }
        else
        {
            playerRigid.gravityScale = playerGravity;
        }

        if (isDashing) //Disables gravity when dashing, dashes in straight line in air
        {
            playerRigid.gravityScale = 0;

        }
        else
        {
            playerRigid.gravityScale = playerGravity;
        }

        //Adding coyote time (can jump a bit after not being on a platform)
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        //Adding wall jump coyote time (can wall jump a bit after not being on a wall)
        if (OnStickyWall())
        {
            wallCoyoteCounter = wallCoyoteTime;
        }
        else
        {
            wallCoyoteCounter -= Time.deltaTime;
        }

        //Adding jump buffer (can jump a bit before touching the ground)
        if (isJumping)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        //~~~ UI AND VFX ~~~\\ 

        if (frozen) //Activate the Button press UI above the player
        {
            animator.SetBool("isFrozen", true);
            DeleteVFXOfTag("CollectableVFX");
        }
        else
        {
            animator.SetBool("isFrozen", false);
        }

        if (frozen && hasIcePower) //Disable powerup when frozen
        {
            hasIcePower = false;

        }

        if (!hasIcePower) //delete vfx if we dont have the power
        {
            DeleteVFXOfTag("CollectableVFX");
        }
        
        //~~~ STATES ~~~\\ 

        if (move.x != 0 && IsGrounded())
        {
            animator.SetBool("isRunning", true);
            StartRunTrigger();
           
        
        }
        else
        {
            animator.SetBool("isRunning", false);
            isRunning = false;
            //AM.StopPlaying("Steps");
        
        }

        if (!IsGrounded() && playerVelocity.y > 0 && !OnStickyWall())
        {
            animator.SetBool("IsJumping", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }

        if (!IsGrounded() && playerVelocity.y < 0 && !OnStickyWall())
        {
            animator.SetBool("isFalling", true);
            isFalling = true;
        }
        else
        {
            animator.SetBool("isFalling", false);
            isFalling = false;
        }

        //Landing Logic

        if (!IsGrounded())
        {
            wasInAir = true;
        }
        if (wasInAir && IsGrounded()) 
        {
            LandingTrigger();
            wasInAir = false;
        }
        else
        {
            isLanding = false;
        }


        animator.SetBool("isWallSliding", OnStickyWall() && !IsGrounded() && !frozen);


        timeSinceLastKill += Time.deltaTime;
        dashCooldown += Time.deltaTime;
    }
    public void StartRunTrigger() //to trigger sprint effect only once
    {
        if (!isRunning)
        {
            isRunning = true;
            PlayTriggeredEffect("Sprint");
        }
    }
    public void LandingTrigger() //to trigger landing effect only once
    {
        if (!isLanding)
        {
            isLanding = true;
            animator.SetTrigger("Landing");
            PlayTriggeredEffect("Land");
            AM.Play("Landing");
        }
    }
    public void HideArrow()
    {
        playerArrow.HideArrow();
    }



    //~~~~~~~ PLAYER CONTROL ~~~~~~~\\
    //~~~ MOVE ~~~\\ 
    //Function(Action Input file's current input)
    public void OnMove(InputAction.CallbackContext ctx)
    {
        //A/D or Thumbstick -1/+1
        Vector2 movement = ctx.ReadValue<Vector2>();
        //Debug.Log(movement);
        
        //update move vector with -1/+1, players current y velocity
        move = new Vector3(movement.x, movement.y, playerVelocity.y);
       
        
    }
 
    //called by FixedUpdate
    private void PlayerMovement()
    {

        
        playerVelocity = playerRigid.velocity;   //update current velocity Vector2 to players current velocity

        if (move.x != 0) //gets previous Xmovement for use in Ice Stuff
        {
            previousXMovement = playerRigid.velocity.x;
        }


         if (move.x != 0 && IsGrounded())
        {
                        //PlaySteps();
        }




        if (hasInvertedControls) //Basic horizontal movment, and inverted horizontal movement, and slowed horizontal movement when in air
        {
            playerRigid.velocity = new Vector2(-move.x * moveForce, playerVelocity.y);
        }
        else if (!onIce && !isWallJumping && !isDeflecting && !isDashing && !isDying && !frozen)
        {
            playerRigid.velocity = new Vector2(move.x * moveForce, playerVelocity.y);
        }
        else if (!IsGrounded() && !isWallJumping && !isDeflecting && !isDashing && !frozen)
        {
            playerRigid.velocity = new Vector2(move.x * moveForce * midAirMoveMultiplier, playerVelocity.y);
        }
    }

   

    //~~~ JUMP ~~~\\ 
    public void OnJump(InputAction.CallbackContext ctx) //when A is pressed
    {
        isJumping = true;

       

        if (frozen) //code for when frozen, have to jump [breakAmount] of times to escape
        {

            breakCounter++;
            iceBlock.BreakIce();
            if (breakCounter == breakAmount)
            {
                playerRigid.constraints = ~RigidbodyConstraints2D.FreezePosition;
                playerRigid.isKinematic = false;

                breakCounter = 0;
                frozen = false;
                iceBlock.HideIce();
                buttonPress.HideButtonPress();
                iceBlock.ResetIce();
                Debug.Log("Broke free from Ice!!!");
                vfxController.GetComponent<VFXController>().PlayVFX(transform, "Shatter");
                AM.Play("BreakFree");
            }

        }
        else if ((coyoteCounter > 0 && jumpBufferCounter > 0 || coyoteCounter > 0 && jumpBufferCounter < 0) && !isDashing) //checking for coyote time and jump buffer
        {
            if (IsOnBouncy())
            {
                //print("BIG JUMP");
                playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceMultiplier); //Bouncy Jump
                PlayJumpAudio();

                coyoteCounter = 0f;
                jumpBufferCounter = 0f;
                isJumping = true;
            }
            else
            {
                PlayJumpAudio();

                //Debug.Log("jump");
                playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce); // Normal Jump
                coyoteCounter = 0f;
                jumpBufferCounter = 0f;
                isJumping = true;
            }
        }

        //~~~ WALL JUMP ~~~\\ 
        else if (wallCoyoteCounter > 0 & !IsGrounded() & facingRight && move.x > 0f) // Wall Climbing when facing right
        {
            isWallJumping = true;
            playerRigid.velocity = new Vector2(-transform.localScale.x * wallClimbX, wallClimbY);
            PlayJumpAudio();
            wallJumpCooldown = 0;


            Invoke(nameof(StopWallJumping), wallClimbingDuration); //while we are wall climbing, the player cannot change thier velocity, so after a duration, let the players control the PC again
        }
        else if (wallCoyoteCounter > 0 & !IsGrounded() & !facingRight && move.x < 0f) // Wall Climbing when facing left
        {
            isWallJumping = true;
            playerRigid.velocity = new Vector2(-transform.localScale.x * wallClimbX, wallClimbY);
            PlayJumpAudio();
            wallJumpCooldown = 0;


            Invoke(nameof(StopWallJumping), wallClimbingDuration);
        }
        else if (wallCoyoteCounter > 0 & !IsGrounded()) // Wall Jumping / Kicking
        {
            isWallJumping = true;
            playerRigid.velocity = new Vector2(-transform.localScale.x * wallKickX, wallKickY);
            PlayJumpAudio();
            wallJumpCooldown = 0;

            if (transform.localScale.x != wallJumpingDirection) //flips player so they are facing the direction that they are jumping towards
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration); //while we are wall jumping, the player cannot change thier velocity, so after a duration, let the players control the PC again

        }
    }

    private void StopWallJumping()
    {
        //Debug.Log("wall jump stop");
        isWallJumping = false;
    }


    public void TopTrigger()
    {
        playerRigid.velocity = new Vector2(playerVelocity.x, 0.5f * jumpForce);
        // AM.Play("Bounce");
    }





    //~~~~~~~ MOVEMENT LOGIC ~~~~~~~\\ 
    //called by Fixed Update, line 42
    private void Flip()  //flips the player around when moving left or right | Allows us to determine player direction for animating and other stuffs
    {
        if (!frozen) //cannot flip while frozen
        {
            if (move.x > 0f)
            {
                transform.localScale = startingScale;
                facingRight = true;
            }

            else if (move.x < 0f)
            {
                transform.localScale = new Vector3(-startingScale.x, startingScale.y, 1);
                facingRight = false;
            }
        }



    }

    //~~~ ICE ~~~\\
    private void IceMovement()
    {
        if (IsOnIce())
        {
            if (move.x != 0) //when we have input, move normally (will refine this later)
            {
                playerRigid.velocity = new Vector3(move.x * iceSpeed, playerVelocity.y, 0);
            }
            else
            {

                playerRigid.velocity = new Vector2(previousXMovement * IceDecceleration, playerVelocity.y); //when no input, slide across|Speed decreases by the rate of IceDecceleration
                previousXMovement = playerRigid.velocity.x;
            }

        }

    }

    //~~~ WALL SLIDE ~~~\\
    private void WallSlide()
    {
        //wall jumping code adapted  from https://www.youtube.com/watch?v=_UBpkdKlJzE and https://www.youtube.com/watch?v=O6VX6Ro7EtA | 
        if (wallJumpCooldown > 0.2f && (!IsOnIce()) && !isWallJumping && !isTeleporting)
        {
            //playerRigid.velocity = new Vector2(move.x * moveForce, playerVelocity.y);

            if (OnStickyWall() && !IsGrounded() )
            {
                isWallJumping = false;
                CancelInvoke(nameof(StopWallJumping));


                //PlayWallSlide();
                


                //flip player light around when sliding down a wall
                playerLight.FlipLight(true);
                if (move.y >= -0.8f)
                {
                    playerRigid.velocity = new Vector2(playerRigid.velocity.x, Mathf.Clamp(playerRigid.velocity.y, wallSlideSpeed, float.MaxValue));
                }


            }
            else
            {
                playerLight.FlipLight(false);


                //AM.StopPlaying("WallSlide");
            }

        }
        else
            wallJumpCooldown += Time.deltaTime;
    }
  

    //~~~ BOUNCE ~~~\\
    private void BounceMovement()
    {
        if (IsOnBouncy())
        {

            // float previousYMovement = playerRigid.velocity.y;
            // playerRigid.velocity = new Vector2(playerVelocity.x, -previousYMovement * bounceRebound);
            playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceRebound);
            AM.Play("Bouncy");
            print("Yipeeee");

            //animates the mushroom
            RaycastHit2D mushroom = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 50f, groundLayer);
            if (mushroom.collider != null)
            {
                if (mushroom.collider.CompareTag("Bouncy"))
                {
                    mushroom.collider.gameObject.GetComponent<Mushroom>().Bounce();
                    //print("shrrom innit");
                }

            }
            else
            {
                //print ("whoops");
            }

        }

    }

    //~~~ DASH ~~~\\
    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (dashCooldown > dashCooldownTime && ls.introIsOver && ls.outroIsOver && !frozen)
        {
            isDashing = true;
            dashCooldown = 0;
            playerRigid.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
            StartCoroutine(IgnorePlayerCollisions(dashDuration));

            AM.Play("Dash");
            vfxController.GetComponent<VFXController>().PlayPlayerVFX(playerNum, "Dash");
            //change direction of effect whether facing right or left
            PlayTriggeredEffect("DashEffect");


            animator.SetTrigger("Dashing");
        }
         Invoke(nameof(StopDashing), dashDuration);
    }

    //~~~IGNORE COLLISIONS ~~~\\
    private IEnumerator IgnorePlayerCollisions(float duration) // for duration we move to IgnoreCollisions layer 
    {
        gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
        playerTop.gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
        yield return new WaitForSeconds(duration);
        gameObject.layer = LayerMask.NameToLayer("Player");
        playerTop.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void StopDashing()
    {
        isDashing = false;
    }


    //~~~ TELEPORT ~~~\\ 
    private void Teleport()
    {
        isTeleporting = true;
        Vector2 previousVelocity = playerVelocity;
        transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;
        AM.Play("Teleport");


        Quaternion destinationRotation = currentTeleporter.GetComponent<Teleporter>().GetDestination().rotation;
        Quaternion currentRotation = currentTeleporter.GetComponent<Transform>().rotation;

        if (destinationRotation != currentRotation)
        {
            //Debug.Log("rotation is different, adding force");
            //will shoot out portal with the 80% of the velocity it fell into the portal with
            playerRigid.velocity = new Vector2((-previousVelocity.y) * 0.8f, previousVelocity.x);
            //will not work when teleporting to teleports facing left due to velocity direction, need to find a fix
        }

        Invoke(nameof(StopTeleporting), teleportingDuration);
    }

    private void StopTeleporting()
    {
        isTeleporting = false;
    }



    //~~~ COMBAT ~~~\\
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        //ADD ANIMATIONS AND SOUND
        //build up timer, enable attack collider, attack timer, disable attack collider
        if (!isAttacking)
        {
            //Debug.Log("attack begin");
            isAttacking = true;   //begin attack
            Invoke(nameof(Attack), attackBuildUp);   //timer for animation - .1 second
            animator.SetTrigger("Attacking");
            PlaySwordAudio();
        }
    }

    private void Attack()
    {
        //create new array & fill with collider array from attack script
        //for each collider in the array,
        //if the position isnt null,
        //get the colliders tag & run the appropriate function
        //Debug.Log("attacking");
        BoxCollider2D[] collisions = this.gameObject.transform.Find("Attack").GetComponent<PlayerAttackTrigger>().GetColliders();
        
          for (int colIndex = 0; colIndex < collisions.Length; colIndex++)
        {
            if (collisions[colIndex] != null && !frozen) //cannot attack while frozen
            {
                /*if (this.gameObject.name == "Player0")
                {
                    //Debug.Log("index " + colIndex + ": " + collisions[colIndex]);
                }*/

                string colTag = collisions[colIndex].gameObject.tag;
                PlayerController otherPlayer = null;
                if (colTag == "PlayerFront" || colTag == "PlayerBack")
                {
                    otherPlayer = collisions[colIndex].gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
                }

                switch (colTag)
                {
                    case "PlayerFront":
                        //deflect player if not hitting with Ice attack
                        if (hasIcePower == true && this.gameObject.transform.localScale.x == -collisions[colIndex].gameObject.transform.parent.localScale.x && !otherPlayer.invincible )
                        {
                            //if player attacking has ice power, freeze target
                            IceAttack(otherPlayer);

                        }
                        else if (!isDeflecting && this.gameObject.transform.localScale.x == -collisions[colIndex].gameObject.transform.parent.localScale.x )
                        {
                            //deflect if this player isnt being deflected and both players are facing opposite directions  ,0>  <0,
                            Deflect();
                            vfxController.GetComponent<VFXController>().PlayVFX(deflectRef, "Deflect");

                            otherPlayer.Deflect();
                        }
                        break;
                    case "PlayerBack":
                        //kill other player if both facing the same direction ,0> ,0>
                        if (this.gameObject.transform.localScale.x == collisions[colIndex].gameObject.transform.parent.localScale.x && !otherPlayer.invincible)
                        {
                            ls.Kill(collisions[colIndex].gameObject.transform.parent.gameObject, this.gameObject);
                            if (controller != null) { vfxController.GetComponent<HapticController>().PlayHaptics("Kill", controller); }
                            if (otherPlayer.OnStickyWall())
                            {
                                Deflect();
                            }
                            else
                            {
                                ls.Kill(collisions[colIndex].gameObject.transform.parent.gameObject, this.gameObject);
                                if (controller != null)
                                {
                                    vfxController.GetComponent<HapticController>().PlayHaptics("Kill", controller);
                                }
                              
                            }

                        }
                        break;
                    default:
                        break;
                }
            }
        }

        Invoke(nameof(AttackFinish), attackTimer); 
    }

    private void AttackFinish()
    {
        //Debug.Log("attack over");
        isAttacking = false;   //end attack

    }


    //~~~ SCORE ~~~\\
    public void SetScore(int newScore)
    {
        score = newScore;
        Debug.Log(this.gameObject.name + "'s score set to " + score);
    }

    public void IncScore()
    {
        score++;
        Debug.Log(this.gameObject.name + "'s score increased by 1: " + score);
    }

    public void RedScore()
    {
        score--;
        Debug.Log(this.gameObject.name + "'s score decreased by 1: " + score);
    }

    public int GetScore()
    {
        return score;
    }



    //~~ ICE ATTACK ~~\\
    public void IceAttack(PlayerController otherPlayer) //when player attacks otherPlayer with an ice attack
    {
        if (hasIcePower == true)
        {
            Debug.Log("Player Has Attacked with Ice");
            hasIcePower = false;
            DeleteVFXOfTag("CollectableVFX");
            otherPlayer.Freeze();
            AM.Play("Freeze");
            
        }
    }
    public void IceAttackTest() //when player attacks otherPlayer with an ice attack
    {
        if (hasIcePower == true)
        {
            Debug.Log("Player Has Attacked with Ice");
            hasIcePower = false;
            DeleteVFXOfTag("CollectableVFX");

            AM.Play("Freeze");

        }
    }
    public void Freeze()
    {
       // breakAmount = Random.Range(5, 10); //sets the amount of times we need to press jump to escape to a ranodm number between these numbers
        breakAmount = 6;
        iceBlock.ShowIce();
        buttonPress.ShowButtonPress();
        frozen = true;
        hasIcePower = false;
        DeleteVFXOfTag("CollectableVFX");
        //RigidbodyConstraints2D.FreezeRotationZ; to freeze flip?
        playerRigid.constraints = RigidbodyConstraints2D.FreezeAll;

    }

    //~~~ STUN ~~~\\
    public void Stun()      //CURRENTLY USES FROZEN ANIM
    {
        Debug.Log("Stun started on " + this.gameObject.transform.name);
        //start stun animation & freeze player for 1 second
        frozen = true;
        Invoke(nameof(StunEnd), stunTime);
    }

    public void StunEnd()
    {
        Debug.Log("Stun ended on " + this.gameObject.transform.name);
        playerRigid.constraints = ~RigidbodyConstraints2D.FreezePosition;
        playerRigid.isKinematic = false;
        frozen = false;
    }




    //~~~ DEFLECT ~~~\\
    private void Deflect()
    {
        isDeflecting = true;
        PlayRebound();
        playerRigid.velocity = new Vector2(-transform.localScale.x * deflectForce, 10);



        Invoke(nameof(StopDeflect), deflectDuration);
    }

    private void StopDeflect()
    {
        //Debug.Log("deflect stop");
        isDeflecting = false;
    }



    //~~~ DEATH ~~~\\
    public void Death() //RIP
    {
        isDying = true;

        animator.SetTrigger("Dying");
        playerLight.HideLight();
        iceBlock.HideIce();
        buttonPress.HideButtonPress();
        spriteRenderer.sortingOrder = 4;
        PlayDeathAudio();

        if (frozen)
        {
            spriteRenderer.enabled = false;
            
        }
        

        Invoke(nameof(KillDelay), 0.6f); //set to time of deathAnimation

        StartCoroutine(IgnorePlayerCollisions(0.4f)); //stops players colliding with eachother after one has died for a duration

       // if (controller != null) RENABLE TO CAUSE HAPTICS ON PLAYER DEATH
       // { vfxController.GetComponent<HapticController>().PlayHaptics("Death", controller); }
        if (frozen)
        { vfxController.GetComponent<VFXController>().PlayVFX(transform, "Ice Death"); }
        else
        { vfxController.GetComponent<VFXController>().PlayVFX(this.gameObject.transform, "Death"); }

        DeleteVFXOfTag("CollectableVFX");
    }

    //delays destroying target to allow the death anim to play
    public void KillDelay()
    {
        ls.Respawn((int)char.GetNumericValue(this.gameObject.name[6]), this.gameObject, animator);
    }

    public void Respawn() //deals with changing values once player has already respawned, actual respawning is done in LevelScript
    {
        invincible = true;
        frozen = false;
        hasIcePower = false;
        spriteRenderer.enabled = true;
        playerArrow.ShowArrow();
        FaceTowardCenter();

        DeleteVFXOfTag("CollectableVFX");
        vfxController.GetComponent<VFXController>().PlayVFX(transform, "Respawn");
        
        spriteRenderer.sortingOrder = 3;
        StartCoroutine(InvincibilityFlash());
        invincible = true;
        //spriteRenderer.material = whiteMaterial;
        Invoke(nameof(InvincibilityTimer), invincibilityTime);
    }
    public void InvincibilityTimer()
    {
        invincible = false;
        frozen = false;
        hasIcePower = false;
        //spriteRenderer.material = defaultMaterial;
        playerArrow.HideArrow();
        playerLight.ShowLight();
    }
    public bool GetIsInvincible() { return invincible; }

    private IEnumerator InvincibilityFlash()
    {
        //Debug.Log("InvincibilityFlash");
        //flashing duration and interval
        float flashDuration = invincibilityTime;
        float flashInterval = invincibilityTime / 5;
        //Debug.Log("Dur: " + flashDuration + "/Int: " + flashInterval);

        while (flashDuration > 0)
        {
            //set renderer material to flash, wait, set back

            //Debug.Log("on");
            spriteRenderer.material = invincibilityMat;
            yield return new WaitForSeconds(flashInterval);
            flashDuration -= flashInterval;

            //Debug.Log("off");
            spriteRenderer.material = originMat;
            yield return new WaitForSeconds(flashInterval);
            flashDuration -= flashInterval;
        }

        //set renderer material to origin on flash end
        spriteRenderer.material = originMat;
        //Debug.Log("finished");
    }

    public bool GetIsDying() { return isDying; }
    public void SetIsDying(bool dying) { isDying = dying; }

    private void DeleteVFXOfTag(string tag) //deletes all children with VFX tag, messy fix but it should work
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject == GameObject.FindWithTag(tag))
            {
                Destroy(child.gameObject);
            }
            
        }
    }
    public void PlayTriggeredEffect(string effectName) //effects that are dependent on player Direction
    {
        if (facingRight) { vfxController.GetComponent<VFXController>().PlayVFXwithDirection(transform, effectName, 1); }
        else { vfxController.GetComponent<VFXController>().PlayVFXwithDirection(transform, effectName, -1); }
    }



    //~~~~~~~ TRIGGERS ~~~~~~~\\
    //~~~ ENTER ~~~\\ 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string colTag = collision.gameObject.tag;
        //Debug.Log(this.name);
        //Debug.Log(collision.gameObject.name);

        //ADD COLLECTABLES
        switch (colTag)
        {
            case "Teleporter":
                if (!isTeleporting)
                {
                    //Debug.Log("Teleporting");
                    currentTeleporter = collision.gameObject;
                    Teleport();
                }
                break;
            case "DashCollectable":
                //Debug.Log("Collected Dash");
                hasDashPower = true;
                Destroy(collision.gameObject);
                break;
            case "IceCollectable":
                //Debug.Log("Collected Ice");
                if (hasIcePower) //remove collectable if we arady have that one equipped 
                {
                    //Destroy(collision.gameObject);
                    //Debug.Log("Already have collectable, ignored");
                }
                else
                {
                    hasIcePower = true;
                    //Debug.Log("Collected Ice");
                    collision.gameObject.GetComponent<Collectable>().PickUp();
                    AM.Play("Collect");
                    vfxController.GetComponent<VFXController>().PlayPlayerVFX(playerNum, "Snow");
                }
                break;
            case "InverseCollectable":
                //Debug.Log("Collected Invert");
                hasInvertPower = true;
                Destroy(collision.gameObject);
                AM.Play("Collect");
                InvertCollected();
                break;
            default:
                break;
        }
    }

    public void InvertCollected()
    {

        //print("Invert Collected");
        GameObject[] players = PlayerData.GetPlayers(); //store player list from player data

        foreach (GameObject thisPlayer in players)  //search player list for this player
        {
            if (thisPlayer.gameObject == gameObject)
            {
                ls.InvertControls(thisPlayer); //call Invert Controls passing in this player sp they are exempt
            }
            else
            {
                //print("not this p;layer");
            }

        }
    }
    public void InvertControls()
    {
        hasInvertedControls = true;
    }
    public void UnInvertControls()
    {
        hasInvertedControls = false;
    }


    //~~~ EXIT ~~~\\ 
    private void OnTriggerExit2D(Collider2D collision)
    {
        string colTag = collision.gameObject.tag;

        switch (colTag)
        {
            case "Teleporter":
                if (currentTeleporter == collision.gameObject)
                {
                    currentTeleporter = null;
                }
                break;
            default:
                break;
        }
    }


    //~~~ FRONT TRIGGER ~~~\\
    public void FrontTrigger(Collider2D collision)
    {
    }


    //~~~ BACK TRIGGER ~~~\\
    public void BackTrigger(Collider2D collision)
    {
    }


    //~~~ ATTACK TRIGGER ~~~\\
    public void AttackTrigger(Collider2D collision)
    {
    }
    


    //~~~~~~~ CROWN ~~~~~~~\\
    public void EnableCrown() { crown.SetActive(true); crowned = true; }

    public void DisableCrown() { crown.SetActive(false); crowned = false; }

    public bool GetCrowned() { return crowned; }

   



    //~~~~~~~ GROUND & WALL CHECKS ~~~~~~~\\
    //IsGrounded, IsOnWall Method orginally from https://www.youtube.com/watch?v=_UBpkdKlJzE
    public bool IsGrounded()
    {
        //casts an invisble box from the players center down to see if the player is touching the ground
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.2f, groundLayer);
        return raycastHit.collider != null;
    }
    public bool IsOnIce()
    {

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);

        //if we hit something, and that something has the Ice tag, return true, else, return false
        if (raycastHit.collider != null)
        {

            if (raycastHit.collider.CompareTag("Ice"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }


    }
    public bool IsOnBouncy()
    {

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);

        //if we hit something, and that something has the Bouncy tag, return true, else, return false
        if (raycastHit.collider != null)
        {

            if (raycastHit.collider.CompareTag("Bouncy"))
            {
                return true;

            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }


    }
    public bool OnStickyWall()
    {
        //casts an invisble box from the players center to whichever way the player is facing to see if we are touching a sticky wall
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.2f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool IsDeflecting()
    {
        return isDeflecting;
    }





    //~~~~~~~ PLAYER POSITION & VELOCITY ~~~~~~~\\
    //~~~ X ~~~\\ 
    public float GetPlayerX()
    {
        return this.transform.position.x;
    }
    public void SetPlayerX(float newX)
    {
        Vector2 newPos = new Vector2(newX, playerRigid.position.y);
        playerRigid.position = newPos;
    }

    //~~~ Y ~~~\\ 
    public float GetPlayerY()
    {
        return this.transform.position.y;
    }
    public void SetPlayerY(float newY)
    {
        Vector2 newPos = new Vector2(playerRigid.position.x, newY);
        playerRigid.position = newPos;
    }


    //~~~ POSITION ~~~\\ 
    public void GetPlayerPos(Vector2 newPos)
    {
        playerRigid.position = newPos;
    }
    public void SetPlayerPos(Vector2 newPos)
    {
        playerRigid.position = newPos;
    }


    //~~~ X VELOCITY ~~~\\ 
    public float GetPlayerXVelocity()
    {
        return playerRigid.velocity.x;
    }
    public void SetPlayerXVelocity(float newXVel)
    {
        Vector2 newVel = new Vector2(newXVel, playerRigid.velocity.y);
        playerRigid.velocity = newVel;
    }

    public Vector3 GetPlayerLocalScale() { return playerRigid.transform.localScale; }


    //~~~ Y VELOCITY ~~~\\ 
    public float GetPlayerYVelocity()
    {
        return playerRigid.velocity.y;
    }
    public void SetPlayerYVelocity(float newYVel)
    {
        Vector2 newVel = new Vector2(playerRigid.velocity.x, newYVel);
        playerRigid.velocity = newVel;
    }





    //~~~~~~~ PLAYER FORCES & SPEED ~~~~~~~\\
    //~~~ JUMP FORCE ~~~\\ 
    public void SetJumpForce(float newJF)
    {
        jumpForce = newJF;
    }

    //~~~ MOVE FORCE ~~~\\ 
    public void SetMoveForce(float newMF)
    {
        moveForce = newMF;
    }

    //~~~ GRAVITY ~~~\\ 
    public void SetPlayerGravity(float newPG)
    {
        playerRigid.gravityScale = newPG;
    }





    //~~~~~~~ AUDIO ~~~~~~~\\
    //~~~ JUMP ~~~\\ 
    public void PlayJumpAudio()
    {

        AM.Play("JumpWhoosh");


    }


    /*public void PlayWallSlide()
    {

        bool isWallSlidePlaying = false;

        foreach (var sound in AM.sounds)
        {
            if (sound.name == "WallSlide" && sound.isPlaying)
            {
                isWallSlidePlaying = true;
                break; // Exit the loop if the sound is found to be playing
            }
        }

        if (!isWallSlidePlaying)
        {
            AM.Play("WallSlide");
        }
    }




     public void PlaySteps()
    {

    bool isStepsPlaying = false;

    foreach (var sound in AM.sounds)
    {
    if (sound.name == "Steps" && sound.isPlaying)
    {
        isStepsPlaying = true;
        break; // Exit the loop if the sound is found to be playing
    }
    }

    if (!isStepsPlaying)
    {
    AM.Play("Steps");
    }
    }*/


    //~~~ DEATH ~~~\\ 
    public void PlayDeathAudio()
    {
        // TODO 1.Change audio dpeending on what player has died
        //      2.random change of pitch

        int SoundNo = PlayerData.GetSpriteID(playerNum);

        AM.Play("DeathSound");

        if (SoundNo == 1)
        {
            AM.Play("HornDie");
        }
        if (SoundNo == 2)
        {
            AM.Play("TreeDie");
        }
        if (SoundNo == 3)
        {
            AM.Play("CopperDie");
        }
        if (SoundNo == 0)
        {
            AM.Play("LavaDie");
        }
         if (SoundNo == 4)
        {
            AM.Play("DeathCry3");
        }

    }
    //~~~ SWORD SWING ~~~\\ 
    public void PlaySwordAudio()
    {
        int SoundNo = Random.Range(1, 4);


        if (SoundNo == 1)
        {
            AM.Play("SwordSwing1");
        }
        if (SoundNo == 2)
        {
            AM.Play("SwordSwing2");
        }
        if (SoundNo == 3)
        {
            AM.Play("SwordSwing3");
        }

    }


    //~~~ REBOUND ~~~\\ 
    public void PlayRebound()
    {
        int SoundNo = Random.Range(1, 4);


        if (SoundNo == 1)
        {
            AM.Play("SwordClang");
        }
        if (SoundNo == 2)
        {
            AM.Play("SwordClang2");
        }
        if (SoundNo == 3)
        {
            AM.Play("SwordClang3");
        }

    }


    //~~~~~~~ DEV ~~~~~~~\\
    //~~~ SET MODE ~~~\\ 
    public void SetDevMode(bool dev)
    {
        devMode = dev;
    }

    //~~~ HITBOXES ~~~\\
    private void HighlightHitboxes()
    {
        //declare variables
        BoxCollider2D frontCol, backCol, playerCol, attackCol, topCol;
        Vector3 fCenter, bCenter, pCenter, aCenter, tCenter;
        Vector3 fSize, fMin, fMax, bSize, bMin, bMax, pSize, pMin, pMax, aSize, aMin, aMax, tSize, tMin, tMax;


        //~~~ FRONT ~~~\\
        //Debug.Log(this.transform.Find("Front"));
        frontCol = this.transform.Find("Front").GetComponent<BoxCollider2D>();
        fCenter = frontCol.bounds.center;
        fSize = frontCol.bounds.size;
        fMin = frontCol.bounds.min;
        fMax = frontCol.bounds.max;

        /*//Debug.Log("Collider Center : " + fCenter);
        //Debug.Log("Collider Size : " + fSize);
        //Debug.Log("Collider bound Minimum : " + fMin);
        //Debug.Log("Collider bound Maximum : " + fMax);*/

        Debug.DrawLine //front top line
        (new Vector3(fMin.x, fMax.y, 0), //start
        new Vector3(fMax.x, fMax.y, 0), //end
        Color.blue);

        Debug.DrawLine //front bottom line
        (new Vector3(fMin.x, fMin.y, 0), //start
        new Vector3(fMax.x, fMin.y, 0), //end
        Color.blue);

        Debug.DrawLine //front left line
        (new Vector3(fMin.x, fMin.y, 0), //start
        new Vector3(fMin.x, fMax.y, 0), //end
        Color.blue);

        Debug.DrawLine //front right line
        (new Vector3(fMax.x, fMin.y, 0), //start
        new Vector3(fMax.x, fMax.y, 0), //end
        Color.blue);



        //~~~ BACK ~~~\\
        backCol = this.transform.Find("Back").GetComponent<BoxCollider2D>();
        bCenter = backCol.bounds.center;
        bSize = backCol.bounds.size;
        bMin = backCol.bounds.min;
        bMax = backCol.bounds.max;

        /*//Debug.Log("Collider Center : " + bCenter);
        //Debug.Log("Collider Size : " + bSize);
        //Debug.Log("Collider bound Minimum : " + bMin);
        //Debug.Log("Collider bound Maximum : " + bMax);*/

        Debug.DrawLine //back top line
        (new Vector3(bMin.x, bMax.y, 0), //start
        new Vector3(bMax.x, bMax.y, 0), //end
        Color.magenta);

        Debug.DrawLine //back bottom line
        (new Vector3(bMin.x, bMin.y, 0), //start
        new Vector3(bMax.x, bMin.y, 0), //end
        Color.magenta);

        Debug.DrawLine //back left line
        (new Vector3(bMin.x, bMin.y, 0), //start
        new Vector3(bMin.x, bMax.y, 0), //end
        Color.magenta);

        Debug.DrawLine //back right line
        (new Vector3(bMax.x, bMin.y, 0), //start
        new Vector3(bMax.x, bMax.y, 0), //end
        Color.magenta);



        //~~~ PLAYER ~~~\\
        playerCol = this.GetComponent<BoxCollider2D>();
        pCenter = playerCol.bounds.center;
        pSize = playerCol.bounds.size;
        pMin = playerCol.bounds.min;
        pMax = playerCol.bounds.max;

        /*//Debug.Log("Collider Center : " + pCenter);
        //Debug.Log("Collider Size : " + pSize);
        //Debug.Log("Collider bound Minimum : " + pMin);
        //Debug.Log("Collider bound Maximum : " + pMax);*/

        Debug.DrawLine //player top line
        (new Vector3(pMin.x, pMax.y, 0), //start
        new Vector3(pMax.x, pMax.y, 0), //end
        Color.green);

        Debug.DrawLine //player bottom line
        (new Vector3(pMin.x, pMin.y, 0), //start
        new Vector3(pMax.x, pMin.y, 0), //end
        Color.green);

        Debug.DrawLine //player left line
        (new Vector3(pMin.x, pMin.y, 0), //start
        new Vector3(pMin.x, pMax.y, 0), //end
        Color.green);

        Debug.DrawLine //player right line
        (new Vector3(pMax.x, pMin.y, 0), //start
        new Vector3(pMax.x, pMax.y, 0), //end
        Color.green);



        //~~~ ATTACK ~~~\\
        attackCol = this.transform.Find("Attack").GetComponent<BoxCollider2D>();
        aCenter = attackCol.bounds.center;
        aSize = attackCol.bounds.size;
        aMin = attackCol.bounds.min;
        aMax = attackCol.bounds.max;

        /*//Debug.Log("Collider Center : " + aCenter);
        //Debug.Log("Collider Size : " + aSize);
        //Debug.Log("Collider bound Minimum : " + aMin);
        //Debug.Log("Collider bound Maximum : " + aMax);*/

        Debug.DrawLine //attack top line
        (new Vector3(aMin.x, aMax.y, 0), //start
        new Vector3(aMax.x, aMax.y, 0), //end
        Color.red);

        Debug.DrawLine //attack bottom line
        (new Vector3(aMin.x, aMin.y, 0), //start
        new Vector3(aMax.x, aMin.y, 0), //end
        Color.red);

        Debug.DrawLine //attack left line
        (new Vector3(aMin.x, aMin.y, 0), //start
        new Vector3(aMin.x, aMax.y, 0), //end
        Color.red);

        Debug.DrawLine //attack right line
        (new Vector3(aMax.x, aMin.y, 0), //start
        new Vector3(aMax.x, aMax.y, 0), //end
        Color.red);



        //~~~ TOP ~~~\\
        topCol = this.transform.Find("Top").GetComponent<BoxCollider2D>();
        tCenter = topCol.bounds.center;
        tSize = topCol.bounds.size;
        tMin = topCol.bounds.min;
        tMax = topCol.bounds.max;

        /*//Debug.Log("Collider Center : " + tCenter);
        //Debug.Log("Collider Size : " + tSize);
        //Debug.Log("Collider bound Minimum : " + tMin);
        //Debug.Log("Collider bound Maximum : " + tMax);*/

        Debug.DrawLine //top top line
        (new Vector3(tMin.x, tMax.y, 0), //start
        new Vector3(tMax.x, tMax.y, 0), //end
        Color.green);

        Debug.DrawLine //top bottom line
        (new Vector3(tMin.x, tMin.y, 0), //start
        new Vector3(tMax.x, tMin.y, 0), //end
        Color.green);

        Debug.DrawLine //top left line
        (new Vector3(tMin.x, tMin.y, 0), //start
        new Vector3(tMin.x, tMax.y, 0), //end
        Color.green);

        Debug.DrawLine //top right line
        (new Vector3(tMax.x, tMin.y, 0), //start
        new Vector3(tMax.x, tMax.y, 0), //end
        Color.green);
    }


    public float GetTimeSinceLastKill()
    {
        return timeSinceLastKill;
    }

    public void ResetTimeSinceLastKill()
    {
        timeSinceLastKill = 0;
    }
}