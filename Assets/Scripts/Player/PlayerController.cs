using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //~~~~~~~ BASE MOVEMENT ~~~~~~~\\
    [SerializeField] private Rigidbody2D playerRigid;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Vector2 move;
    private Vector2 playerVelocity;
    private bool onGround, devMode;
    private float jumpForce = 15f, moveForce = 5f, previousXMovement;
    private LevelScript ls;

    //~~~~~~~ GAMEPLAY ~~~~~~~\\

    //~~~ JUMPING ~~~\\
    [Header("Jumping")]
    [SerializeField] private float coyoteTime;
     private float coyoteCounter;
    [SerializeField] private float jumpBufferTime;
     private float jumpBufferCounter;
    private bool isJumping;
    [SerializeField] private float fallGravityMult;
    [SerializeField] private float playerGravity = 5;

    //~~~ ICE ~~~\\
    [Header("Ice Movement")]
    [SerializeField] private float IceDecceleration = 0.95f; //must be between 1 and 0
    [SerializeField] private float iceSpeed = 9f;
     private bool onIce;

    //~~~ WALL SLIDE & JUMP ~~~\\
    [Header("Wall Jumps, Climbs and Slides")]
    [SerializeField] private float wallSlideSpeed = -2f;
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

    //~~~ FLIP ~~~\\
     private bool facingRight;
    

    //~~~ COMBAT ~~~\\
    [Header("Combat")]
    [SerializeField] private float deflectDuration = 0.5f;
    [SerializeField] private float attackBuildUp = 0.1f;
    [SerializeField] private float attackTimer = 0.4f;
    [SerializeField] private GameObject attackObject;
    private bool isDeflecting, isAttacking;








    void Awake()
    {
        //set level script, update in-game object with name, give level script new player object
        ls = GameObject.Find("LevelController").GetComponent<LevelScript>();
        gameObject.name = "Player" + ls.CurrentPlayer();
        Debug.Log("New player awake, " + gameObject.name);
        ls.NewPlayer(gameObject);
    }


    void FixedUpdate()
    {
        PlayerMovement();
        Flip();
        IceMovement();
        WallSlide();
        BounceMovement();

        if (devMode) { HighlightHitboxes(); }

        //~~~MISC CHECKS AND ADJUSTMENTS ~~~\\ 

        //Gravity tweaking, we fall faster when we start falling in our jump
        if (playerRigid.velocity.y < 0 && !OnStickyWall())
        {
            playerRigid.gravityScale = playerGravity * fallGravityMult;
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
       
    }


    //~~~~~~~ PLAYER CONTROL ~~~~~~~\\
    //called by Player Input component
    //Function(Action Input file's current input)

    //~~~ MOVE ~~~\\ 
    public void OnMove(InputAction.CallbackContext ctx)
    {
        //A/D or Thumbstick -1/+1
        Vector2 movement = ctx.ReadValue<Vector2>();
        //Debug.Log(movement);

        //update move vector with -1/+1, players current y velocity
        move = new Vector3(movement.x, 0, playerVelocity.y);
    }

    private void PlayerMovement()
    {
        playerVelocity = playerRigid.velocity;   //update current velocity Vector2 to players current velocity

        if (move.x != 0) //gets previous Xmovement for use in Ice Stuff
        {
            previousXMovement = playerRigid.velocity.x;
        }
       

        //if player is moving and on ice
        //or not on ice and not wall jumping and on the ground
        //or is not no the ground and and is not wall jumping and is jumping
        if (move.x != 0 && onIce || !onIce && !isWallJumping && !isDeflecting)
        {
            playerRigid.velocity = new Vector2(move.x * moveForce, playerVelocity.y);  
        }
        
     
      
    }


    //~~~ JUMP ~~~\\ 
    public void OnJump(InputAction.CallbackContext ctx)
    {
        isJumping = true;

        if (coyoteCounter > 0 && jumpBufferCounter > 0 ) //checking for coyote time and jump buffer
        {
            if (IsOnBouncy())
            {
                print("BIG JUMP");
                playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceMultiplier); //Bouncy Jump
                PlayJumpAudio();

                coyoteCounter = 0f;
                jumpBufferCounter = 0f;
                isJumping = true;
            }
            else
            {
                playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce); // Normal Jump
                PlayJumpAudio();

                coyoteCounter = 0f;
                jumpBufferCounter = 0f;
                isJumping = true;
            }

        }

        //~~~ WALL JUMP ~~~\\ 

        else if (wallCoyoteCounter > 0 & !IsGrounded() & facingRight && move.x > 0f) // Wall Climbing when facing right
        {
            isWallJumping = true;
            playerRigid.velocity = new Vector2(-transform.localScale.x * 4, 20);
            PlayJumpAudio();
            wallJumpCooldown = 0;

         
            Invoke(nameof(StopWallJumping), wallClimbingDuration); //while we are wall climbing, the player cannot change thier velocity, so after a duration, let the players control the PC again
        }
        else if (wallCoyoteCounter > 0 & !IsGrounded() & !facingRight && move.x < 0f) // Wall Climbing when facing left
        {
            isWallJumping = true;
            playerRigid.velocity = new Vector2(-transform.localScale.x * 4, 20);
            PlayJumpAudio();
            wallJumpCooldown = 0;

           
            Invoke(nameof(StopWallJumping), wallClimbingDuration); 
        }
        else if (wallCoyoteCounter > 0 & !IsGrounded()) // Wall Jumping / Kicking
        {
            isWallJumping = true;
            playerRigid.velocity = new Vector2(-transform.localScale.x * 12, 20);
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


    //~~~~~~~ MOVEMENT LOGIC ~~~~~~~\\ 
    //called by Fixed Update, line 42


    private void Flip()
    {
        //flips the player around when moving left or right | Allows us to determine player direction for animating and other stuffs
        if (move.x > 0.01f)
        {
            transform.localScale = Vector3.one;
            facingRight = true;
        }
           
        else if (move.x < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
         
    }
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
    private void WallSlide()
    {
        //wall jumping code adapted  from https://www.youtube.com/watch?v=_UBpkdKlJzE and https://www.youtube.com/watch?v=O6VX6Ro7EtA | 
        if (wallJumpCooldown > 0.2f && (!IsOnIce()) && !isWallJumping && !isTeleporting)
        {
            playerRigid.velocity = new Vector2(move.x * moveForce, playerVelocity.y);

            if (OnStickyWall() && !IsGrounded())
            {
                isWallJumping = false;
                CancelInvoke(nameof(StopWallJumping));

                playerRigid.velocity = new Vector2(playerRigid.velocity.x, Mathf.Clamp(playerRigid.velocity.y, wallSlideSpeed, float.MaxValue));
            }

        }
        else
            wallJumpCooldown += Time.deltaTime;
    }
    private void BounceMovement()
    {
        if (IsOnBouncy())
        {

            // float previousYMovement = playerRigid.velocity.y;
            // playerRigid.velocity = new Vector2(playerVelocity.x, -previousYMovement * bounceRebound);
            playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceRebound);
            FindObjectOfType<AudioManager>().Play("Bouncy");
            print("Yipeeee");
        }

    }


    //~~~ TELEPORT ~~~\\ 
    private void Teleport()
    {
        isTeleporting = true;
        Vector2 previousVelocity = playerVelocity;
        transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;
        FindObjectOfType<AudioManager>().Play("Teleport");


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


    //~~~ DEFLECT ~~~\\
    private void Deflect(GameObject target)
    {
        Debug.Log(target.name + " deflects " + this.gameObject.name + "'s attack.");
        //Debug.Log("isDeflecting");
        isDeflecting = true;

        //Debug.Log("velocity before: " + playerRigid.velocity);
        //playerRigid.velocity = new Vector2(playerVelocity.x * -10, playerVelocity.y * -10);
        //Debug.Log("velocity after: " + playerRigid.velocity);
        Invoke(nameof(StopDeflect), deflectDuration);
    }

    private void StopDeflect()
    {
        //Debug.Log("deflect stop");
        isDeflecting = false;
    }


    //~~~ COMBAT ~~~\\
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        //ADD ANIMATIONS AND SOUND
        //build up timer, enable attack collider, attack timer, disable attack collider
        //Debug.Log("attack begin");
        isAttacking = true;   //begin attack
        Invoke(nameof(Attack), attackBuildUp);   //timer for animation - .1 second
    }

    private void Attack()
    {
        //create new array & fill with collider array from attack script
        //for each collider in the array,
        //if the position isnt null,
        //get the colliders tag & run the appropriate function
        BoxCollider2D[] collisions = this.gameObject.transform.Find("Attack").GetComponent<PlayerAttackTrigger>().GetColliders();

        for (int colIndex = 0; colIndex < collisions.Length; colIndex++)
        {
            if (collisions[colIndex] != null)
            {
                /*if (this.gameObject.name == "Player0")
                {
                    Debug.Log("index " + colIndex + ": " + collisions[colIndex]);
                }*/

                string colTag = collisions[colIndex].gameObject.tag;

                switch (colTag)
                {
                    case "PlayerFront":
                        //deflect player
                        if (!isDeflecting)
                        {
                            //Debug.Log(this.gameObject.name + " hits a front");
                            Deflect(collisions[colIndex].gameObject.transform.parent.gameObject);
                        }
                        break;
                    case "PlayerBack":
                        //kill other player
                        //Debug.Log(this.gameObject.name + " stabs " + collisions[colIndex].gameObject.transform.parent.gameObject.name + " in the back.");
                        ls.Kill(collisions[colIndex].gameObject.transform.parent.gameObject, this.gameObject);
                        PlayDeathAudio();
                        break;
                    default:
                        break;
                }
            }
        }

        Invoke(nameof(AttackFinish), attackTimer); //end attack after timer - 0.4 seconds
    }

    private void AttackFinish()
    {
        //Debug.Log("hitbox off");
        isAttacking = false;   //end attack
    }





    //~~~~~~~ TRIGGERS ~~~~~~~\\
    //~~~ ENTER ~~~\\ 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string colTag = collision.gameObject.tag;

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
            default:
                break;
        }
    }

    //~~~ EXIT ~~~\\ 
    private void OnTriggerExit2D(Collider2D collision)
    {
        string colTag = collision.gameObject.tag;

        switch(colTag)
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





    //~~~~~~~ GROUND & WALL CHECKS ~~~~~~~\\
    //IsGrounded, IsOnWall Method orginally from https://www.youtube.com/watch?v=_UBpkdKlJzE
    public bool IsGrounded()
    {
        //casts an invisble box from the players center down to see if the player is touching the ground
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);
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
        int SoundNo = Random.Range(1, 5);

        FindObjectOfType<AudioManager>().Play("JumpWhoosh");

        if (SoundNo == 1)
        {
            FindObjectOfType<AudioManager>().Play("JumpGrunt");
        }
        if (SoundNo == 2)
        {
            FindObjectOfType<AudioManager>().Play("JumpGrunt2");
        }

    }

    //~~~ DEATH ~~~\\ 
    public void PlayDeathAudio()
    {
        int SoundNo = Random.Range(1, 4);

        FindObjectOfType<AudioManager>().Play("DeathSound");

        if (SoundNo == 1)
        {
            FindObjectOfType<AudioManager>().Play("DeathCry1");
        }
        if (SoundNo == 2)
        {
            FindObjectOfType<AudioManager>().Play("DeathCry2");
        }
        if (SoundNo == 3)
        {
            FindObjectOfType<AudioManager>().Play("DeathCry3");
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
        BoxCollider2D frontCol, backCol, playerCol, attackCol;
        Vector3 fCenter, bCenter, pCenter, aCenter;
        Vector3 fSize, fMin, fMax, bSize, bMin, bMax, pSize, pMin, pMax, aSize, aMin, aMax;


        //~~~ FRONT ~~~\\
        //Debug.Log(this.transform.Find("Front"));
        frontCol = this.transform.Find("Front").GetComponent<BoxCollider2D>();
        fCenter = frontCol.bounds.center;
        fSize = frontCol.bounds.size;
        fMin = frontCol.bounds.min;
        fMax = frontCol.bounds.max;

        /*Debug.Log("Collider Center : " + fCenter);
        Debug.Log("Collider Size : " + fSize);
        Debug.Log("Collider bound Minimum : " + fMin);
        Debug.Log("Collider bound Maximum : " + fMax);*/

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

        /*Debug.Log("Collider Center : " + bCenter);
        Debug.Log("Collider Size : " + bSize);
        Debug.Log("Collider bound Minimum : " + bMin);
        Debug.Log("Collider bound Maximum : " + bMax);*/

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

        /*Debug.Log("Collider Center : " + pCenter);
        Debug.Log("Collider Size : " + pSize);
        Debug.Log("Collider bound Minimum : " + pMin);
        Debug.Log("Collider bound Maximum : " + pMax);*/

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

        /*Debug.Log("Collider Center : " + aCenter);
        Debug.Log("Collider Size : " + aSize);
        Debug.Log("Collider bound Minimum : " + aMin);
        Debug.Log("Collider bound Maximum : " + aMax);*/

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
    }
}