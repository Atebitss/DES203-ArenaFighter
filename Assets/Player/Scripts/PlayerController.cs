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
    //~~~ ICE ~~~\\
    [Header("Ice Movement")]
    [SerializeField] private float IceDecceleration = 0.95f; //must be between 1 and 0
    private bool onIce;

    //~~~ WALL SLIDE & JUMP ~~~\\
    [Header("Wall sliding and Jumping")]
    private float wallJumpCooldown;
    [SerializeField] private float wallSlideSpeed = -2f;
     private bool onStickyWall;
     private bool isWallJumping;
    [SerializeField] private float postWallJumpMoveForce = 1f;
    [SerializeField] private float wallJumpingDirection;
    [SerializeField] private float wallJumpingDuration = 0.4f;

    //~~~ BOUNCE ~~~\\
    [Header("Bouncing")]
    [SerializeField] private float bounceMultiplier = 1.5f;
    [SerializeField] private float bounceRebound = 1.25f;
    private bool onBouncy;

    //~~~ TELEPORT ~~~\\
    [Header("Teleport")]
    [SerializeField] private GameObject currentTeleporter;
    private bool isTeleporting;
    [SerializeField] private float teleportingDuration = 0.2f;

    //~~~ FLIP ~~~\\
    private bool left, right;

    //~~~ COMBAT ~~~\\
    [Header("Combat")]
    private bool deflecting;
    [SerializeField] private float deflectDuration = 0.5f;
    [Header("Misc")]
    [SerializeField] private float fallGravityMult;
    private float startingGravity;





    void Awake()
    {
        //set level script, update in-game object with name, give level script new player object
        ls = GameObject.Find("LevelController").GetComponent<LevelScript>();
        gameObject.name = "Player" + ls.CurrentPlayer();
        Debug.Log("New player awake, " + gameObject.name);
        ls.NewPlayer(gameObject);
        startingGravity = playerRigid.gravityScale;
    }


    void FixedUpdate()
    {
        //Debug.Log("playerVelocity.x: " + playerVelocity.x);
        PlayerMovement();


        Flip();
        IceMovement();
        WallSlide();
        BounceMovement();

        if (devMode) { HighlightHitboxes(); }

        //GRAVITY SHENANIGANS
        if (playerRigid.velocity.y < 0 && !OnStickyWall())
        {
            playerRigid.gravityScale = 5 * fallGravityMult;
        }
        else
        {
            playerRigid.gravityScale = 5;
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
        if (move.x != 0 && onIce || !onIce && !isWallJumping && !deflecting)
        {
            //Debug.Log("move.x/y: " + move.x + "/" + move.y + "   onGround: " + IsGrounded() + "   onIce: " + IsOnIce() + "   onBouncy: " + IsOnBouncy() + "   onStickyWall: " + OnStickyWall() + "   isDeflecting: " + IsDeflecting());
            //Player's rigid component's velocity set to 1/-1 * 15, 0/15
            playerRigid.velocity = new Vector2(move.x * moveForce, playerVelocity.y);  
        }
        else if (move.x != 0 && onIce || !onIce  && !deflecting) //for post-wall jump velocity
        {

            //playerRigid.velocity = new Vector2(move.x * postWallJumpMoveForce, playerVelocity.y);
        }
    }


    //~~~ JUMP ~~~\\ 
    public void OnJump(InputAction.CallbackContext ctx)
    {
        //A/space 0/+1
        //if +1, set vertical velocity to levels provided jump force

        if (IsGrounded())
        {
            if (IsOnBouncy())
            {
                print("BIG JUMP");
                playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceMultiplier); //Bouncy Jump
            }
            else
            {
                playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce); // Normal Jump
            }

        }
        else if (OnStickyWall() & !IsGrounded()) // Wall Jumping
        {
            isWallJumping = true;
            playerRigid.velocity = new Vector2(-transform.localScale.x * 12, 20);
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
            transform.localScale = Vector3.one;
        else if (move.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }
    private void IceMovement()
    {
        if (IsOnIce())
        {
            if (move.x != 0) //when we have input, move normally (will refine this later)
            {
                playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0);
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
            playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0);

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
        if (IsOnBouncy() )//&& playerRigid.velocity.y >= 0f)
        {

            // float previousYMovement = playerRigid.velocity.y;
            // playerRigid.velocity = new Vector2(playerVelocity.x, -previousYMovement * bounceRebound);
            playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceRebound);
            print("Yipeeee");
        }

    }


    //~~~ TELEPORT ~~~\\ 
    private void Teleport()
    {
        isTeleporting = true;
        Vector2 previousVelocity = playerVelocity;
        transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;

    

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
        //Debug.Log("deflecting");
        deflecting = true;

        //Debug.Log("velocity before: " + playerRigid.velocity);
        //playerRigid.velocity = new Vector2(playerVelocity.x * -10, playerVelocity.y * -10);
        //Debug.Log("velocity after: " + playerRigid.velocity);
        Invoke(nameof(StopDeflect), deflectDuration);
    }

    private void StopDeflect()
    {
        //Debug.Log("deflect stop");
        deflecting = false;
    }


    //~~~ COMBAT ~~~\\
    private void Combat(GameObject target)
    {
        //ADD ANIMATIONS AND SOUND

        //Debug.Log(this.gameObject.name + " is attacking " + target.name);
        if (playerVelocity.x > 10f || playerVelocity.x < -10f || playerVelocity.y > 10f || playerVelocity.y < -10f )
        {
            //Debug.Log(this.gameObject.name + " is going fast enough");
            Debug.Log(this.gameObject.name + " stabs " + target.name + " in the back.");
            ls.Kill(target, this.gameObject);
        }
        //else { Debug.Log(this.gameObject.name + " is not going fast enough"); }
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
        string colTag = collision.gameObject.tag;

        switch (colTag)
        {
            case "PlayerFront":
                //deflect player
                if (!deflecting)
                {
                    Deflect(collision.gameObject.transform.parent.gameObject); 
                }
                break;
            case "PlayerBack":
                //if going fast enough, kill other player
                Combat(collision.gameObject.transform.parent.gameObject);
                break;
            default:
                break;
        }
    }


    //~~~ BACK TRIGGER ~~~\\
    public void BackTrigger(Collider2D collision)
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
        return deflecting;
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





    public void SetDevMode(bool dev)
    {
        devMode = dev;
    }

    private void HighlightHitboxes()
    {
        BoxCollider2D frontCol, backCol, playerCol;
        Vector3 fCenter, bCenter, pCenter;
        Vector3 fSize, fMin, fMax, bSize, bMin, bMax, pSize, pMin, pMax;

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
        Color.red);

        Debug.DrawLine //front bottom line
        (new Vector3(fMin.x, fMin.y, 0), //start
        new Vector3(fMax.x, fMin.y, 0), //end
        Color.red);

        Debug.DrawLine //front left line
        (new Vector3(fMin.x, fMin.y, 0), //start
        new Vector3(fMin.x, fMax.y, 0), //end
        Color.red);

        Debug.DrawLine //front right line
        (new Vector3(fMax.x, fMin.y, 0), //start
        new Vector3(fMax.x, fMax.y, 0), //end
        Color.red);



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



        playerCol = this.GetComponent<BoxCollider2D>();
        pCenter = playerCol.bounds.center;
        pSize = playerCol.bounds.size;
        pMin = playerCol.bounds.min;
        pMax = playerCol.bounds.max;

        /*Debug.Log("Collider Center : " + pCenter);
        Debug.Log("Collider Size : " + pSize);
        Debug.Log("Collider bound Minimum : " + pMin);
        Debug.Log("Collider bound Maximum : " + pMax);*/

        Debug.DrawLine //back top line
        (new Vector3(pMin.x, pMax.y, 0), //start
        new Vector3(pMax.x, pMax.y, 0), //end
        Color.green);

        Debug.DrawLine //back bottom line
        (new Vector3(pMin.x, pMin.y, 0), //start
        new Vector3(pMax.x, pMin.y, 0), //end
        Color.green);

        Debug.DrawLine //back left line
        (new Vector3(pMin.x, pMin.y, 0), //start
        new Vector3(pMin.x, pMax.y, 0), //end
        Color.green);

        Debug.DrawLine //back right line
        (new Vector3(pMax.x, pMin.y, 0), //start
        new Vector3(pMax.x, pMax.y, 0), //end
        Color.green);
    }
}