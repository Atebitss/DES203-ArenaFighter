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
    private bool onGround;
    private float jumpForce = 15f, moveForce = 5f, previousXMovement;
    private LevelScript ls;

    //~~~~~~~ GAMEPLAY ~~~~~~~\\
    //~~~ ICE ~~~\\
    private float IceDecceleration = 0.95f; //must be between 1 and 0
    private bool onIce;

    //~~~ WALL SLIDE & JUMP ~~~\\
    private float wallJumpCooldown;
    private float wallSlideSpeed = -2f;
    private bool onStickyWall;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingDuration = 0.4f;

    //~~~ BOUNCE ~~~\\
    private float bounceMultiplier = 1.5f;
    private float bounceRebound = 1.25f;
    private bool onBouncy;

    //~~~ TELEPORT ~~~\\
    private GameObject currentTeleporter;
    private bool isTeleporting;
    private float teleportingDuration = 0.2f;

    //~~~ FLIP ~~~\\
    private bool left, right;

    //~~~ COMBAT ~~~\\
    private bool deflecting;
    private float deflectDuration = 0.5f;





    void Awake()
    {
        ls = GameObject.Find("LevelController").GetComponent<LevelScript>();
        gameObject.name = "Player" + ls.CurrentPlayers();
        ls.NewPlayer(gameObject);
    }

    void FixedUpdate()
    {
        //Debug.Log("playerVelocity.x: " + playerVelocity.x);
        PlayerMovement();


        Flip();
        IceMovement();
        WallSlide();
        BounceMovement();
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
            playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0);  
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
        if (IsOnBouncy() && playerRigid.velocity.y <= 0f)
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
    private void Deflect()
    {
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
        //ADD ART & SPECIFICS

        //Debug.Log(this.gameObject.name + " is attacking " + target.name);
        if(playerVelocity.x > 10f || playerVelocity.x < -10f || playerVelocity.y > 10f || playerVelocity.y < -10f )
        {
            //Debug.Log(this.gameObject.name + " is going fast enough");
            Destroy(target);
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
                    Deflect(); 
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
}