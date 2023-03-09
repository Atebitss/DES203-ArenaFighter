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
    private float jumpForce = 15f, moveForce = 5f, gravity = -9.81f, previousXMovement;

    //~~~~~~~ GAMEPLAY ~~~~~~~\\
    //~~~ ICE ~~~\\
    private float IceDecceleration = 0.95f; //must be between 1 and 0
    private bool onIce;

    //~~~ WALL SLIDE & JUMP ~~~\\
    private float wallJumpCooldown;
    private float wallSlideSpeed = -2f;
    private bool onStickyWall;
    private bool isWallJumping;
    private bool isJumping;
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





    void FixedUpdate()
    {
        IsOnGround();
        IsOnBouncy();
        IsOnIce();

        Flip();
        IceMovement();
        WallSlide();
        BounceMovement();

        playerVelocity = playerRigid.velocity;   //update current velocity Vector2 to players current velocity
        //playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0);   //Player's rigid component's velocity set to 1/-1 * 15, 0/15
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

    //~~~ JUMP ~~~\\ 
    public void OnJump(InputAction.CallbackContext ctx)
    {
        //A/space 0/+1
        //if +1, set vertical velocity to levels provided jump force

        //if player is on the ground, then jump with relative force
        //unless they are on a sticky wall and not on the ground, then wall jump
        //if the player is not on the ground, not on a sticky wall, and not wall jumping, then apply gravity 

        Debug.Log("jump");
        if (IsOnGround())
        {
            Debug.Log("grounded");
            if (IsOnBouncy())
            {
                Debug.Log("on bouncy");
                playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceMultiplier); //Bouncy Jump
            }
            else { Debug.Log("normal"); playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce); } // Normal Jump
        }
        else if (IsOnStickyWall() & !IsOnGround()) // Wall Jumping
        {
            isWallJumping = true;
            playerRigid.velocity = new Vector2(-transform.localScale.x * 12, 20);
            wallJumpCooldown = 0;
            Debug.Log("wall jumping");

            if (transform.localScale.x != wallJumpingDirection) //flips player so they are facing the direction that they are jumping towards
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
                Debug.Log("flipping player");
            }

            //while we are wall jumping, the player cannot change thier velocity, so after a duration, let the players control the PC again
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
            Debug.Log("starting wall jump cooldown");
        }

        //apply gravity if not grounded, on a stick wall, or wall jumping
        if(!IsOnGround() && !IsOnStickyWall() && !isWallJumping && !isJumping)
        {
            Debug.Log("apply gravity");
            playerRigid.velocity = new Vector2(playerVelocity.x, gravity);
        }
    }





    //~~~~~~~ MOVEMENT LOGIC ~~~~~~~\\ 
    //called by Fixed Update, line 42

    //~~~ FLIP ~~~\\ 
    private void Flip()
    {
        //flips the player around when moving left or right | Allows us to determine player direction for animating and combat
        if (move.x > 0.01f)
        {
            Debug.Log("flip right");
            transform.localScale = Vector3.one;
            right = true;
            left = false;
        }
        else if (move.x < -0.01f)
        {
            Debug.Log("flip left");
            transform.localScale = new Vector3(-1, 1, 1);
            right = false;
            left = true;
        }
    }


    //~~~ ICE ~~~\\ 
    private void IceMovement()
    {
        //if player is on ice and there's no input, slow the players velocity until 0

        if (IsOnIce() && move.x == 0) //if there's no player movement
        {
            //x velocity decreases by the rate of IceDecceleration
            playerRigid.velocity = new Vector3(previousXMovement * IceDecceleration, playerVelocity.y, 0);
            previousXMovement = playerRigid.velocity.x;

            Debug.Log("slowing player on ice");
        }
    }


    //~~~ WALL SLIDE & JUMP ~~~\\ 
    private void WallSlide()
    {
        //wall jumping code adapted  from https://www.youtube.com/watch?v=_UBpkdKlJzE and https://www.youtube.com/watch?v=O6VX6Ro7EtA

        //if wall jump cooldown is greater than 0.2 and the player is:
        //1.not teleporting     2.not wall jumping     3.not on ice     4.not on the ground     5.on a sticky wall
        //then set wall jumping false and update player y velocity with sliding down speed
        //otherwise, increase wall jump cooldown

        if (wallJumpCooldown > 0.2f && (!IsOnIce()) && !isWallJumping && !isTeleporting && !IsOnGround() && IsOnStickyWall())
        {
            Debug.Log("sliding player on sticky wall");
            isWallJumping = false;
            CancelInvoke(nameof(StopWallJumping));

            //could this be done with SetPlayerYVelocity(wallSlideSpeed)
            playerRigid.velocity = new Vector2(playerRigid.velocity.x, Mathf.Clamp(playerRigid.velocity.y, wallSlideSpeed, float.MaxValue));
        }
        else { wallJumpCooldown += Time.deltaTime; }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }


    //~~~ BOUNCE ~~~\\ 
    private void BounceMovement()
    {
        //if the player is on a bouncy surface and the players y velocity is less than 0
        //throw player vertically

        if (IsOnBouncy() && playerRigid.velocity.y < 0f)
        {
            Debug.Log("bouncing player");
            playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceRebound);
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
            Debug.Log("rotation is different, adding force");
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





    //~~~~~~~ TRIGGERS ~~~~~~~\\
    //~~~ ENTER ~~~\\ 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            if (!isTeleporting)
            {
                currentTeleporter = collision.gameObject;
                Teleport();
                Debug.Log("Teleporting");
            }

        }
    }

    //~~~ EXIT ~~~\\ 
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter") && currentTeleporter == collision.gameObject){ currentTeleporter = null; }
    }





    //~~~~~~~ GROUND & WALL CHECKS ~~~~~~~\\
    //IsGrounded, IsOnWall Method orginally from https://www.youtube.com/watch?v=_UBpkdKlJzE
    //~~~ GROUNDED ~~~\\ 
    private bool IsOnGround()
    {
        //casts an invisble box from the players center down to see if the player is touching the ground
        RaycastHit2D groundRaycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);

        if (groundRaycastHit.collider != null) { return onGround = true; }
        else { return onGround = false; }
    }

    public bool GetOnGround()
    {
        return onGround;
    }


    //~~~ ICE ~~~\\ 
    private bool IsOnIce()
    {
        RaycastHit2D iceRaycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);

        //if we hit something, and that something has the Ice tag, return true, else return false
        if (iceRaycastHit.collider != null && iceRaycastHit.collider.tag == "Ice") { return onIce = true; }
        else { return onIce = false; }
    }

    public bool GetOnIce()
    {
        return onIce;
    }


    //~~~ BOUNCE ~~~\\ 
    private bool IsOnBouncy()
    {
        RaycastHit2D bouncyRaycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);

        //if we hit something and that something has the Bouncy tag, return true, else return false
        if (bouncyRaycastHit.collider != null && bouncyRaycastHit.collider.tag == "Bouncy") { return onBouncy = true; }
        else { return onBouncy = false; }
    }

    public bool GetOnBouncy()
    {
        return onBouncy;
    }


    //~~~ STICKY WALL ~~~\\ 
    private bool IsOnStickyWall()
    {
        //casts an invisble box from the players center to whichever way the player is facing to see if we are touching a sticky wall
        RaycastHit2D stickyRaycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.2f, wallLayer);
        if(stickyRaycastHit.collider != null){ return onStickyWall = true; }
        else { return onStickyWall = false; }
    }

    public bool GetOnStickyWall()
    {
        return onStickyWall;
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
        Vector3 newVel = new Vector3(newXVel, playerRigid.velocity.y);
        playerRigid.velocity = newVel;
    }

    //~~~ Y VELOCITY ~~~\\ 
    public float GetPlayerYVelocity()
    {
        return playerRigid.velocity.y;
    }
    public void SetPlayerYVelocity(float newYVel)
    {
        Vector3 newVel = new Vector3(playerRigid.velocity.x, newYVel);
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
