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
    private float jumpForce = 15f, moveForce = 5f, previousXMovement, startingGravity;

    //~~~~~~~ GAMEPLAY ~~~~~~~\\
    //~~~ ICE ~~~\\
    private float IceDecceleration = 0.95f; //must be between 1 and 0

    //~~~ WALL SLIDE & JUMP ~~~\\
    private float wallJumpCooldown;
    private float wallSlidingSpeed = 2f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingDuration = 0.4f;

    //~~~ BOUNCE ~~~\\
    private float bounceMultiplier = 1.5f;
    private float bounceRebound = 1.25f;

    //~~~ TELEPORT ~~~\\
    private GameObject currentTeleporter;
    private bool isTeleporting;
    private float teleportingDuration = 0.2f;





    void Awake()
    {
        startingGravity = playerRigid.gravityScale;
    }

    void FixedUpdate()
    {
        Flip();
        IceMovement();
        WallSlide();
        BounceMovement();

        playerVelocity = playerRigid.velocity;   //update current velocity Vector2 to players current velocity
        playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0);   //Player's rigid component's velocity set to 1/-1 * 15, 0/15
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

        move = new Vector3(movement.x, 0, playerVelocity.y);
    }

    //~~~ JUMP ~~~\\ 
    public void OnJump(InputAction.CallbackContext ctx)
    {
        //A/space 0/+1
        //if +1, set vertical velocity to levels provided jump force
        //Debug.Log("jump");
        if (IsGrounded())
        {
            if (IsOnBouncy())
            {
                playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceMultiplier); //Bouncy Jump
            }
            else { playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce); } // Normal Jump
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

            //while we are wall jumping, the player cannot change thier velocity, so after a duration, let the players control the PC again
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }





    //~~~~~~~ MOVEMENT LOGIC ~~~~~~~\\ 
    //~~~ FLIP ~~~\\ 
    private void Flip()
    {
        //flips the player around when moving left or right | Allows us to determine player direction for animating and other stuffs
        if (move.x > 0.01f)
            transform.localScale = Vector3.one;
        else if (move.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }


    //~~~ ICE ~~~\\ 
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
                playerRigid.velocity = new Vector3(previousXMovement * IceDecceleration, playerVelocity.y, 0); //when no input, slide across|Speed decreases by the rate of IceDecceleration
                previousXMovement = playerRigid.velocity.x;
            }

        }
        else if (!OnStickyWall() && !isWallJumping && !isTeleporting)
        {

            playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0); //if not on ice and not on a wall , use normal movement logic
        }
    }


    //~~~ WALL SLIDE & JUMP ~~~\\ 
    private void WallSlide()
    {
        //wall jumping code adapted  from https://www.youtube.com/watch?v=_UBpkdKlJzE and https://www.youtube.com/watch?v=O6VX6Ro7EtA
        //if players wall jump cd is more than .2 and they're not on ice, not wall jumping, not teleporting, not on the ground, and on a sticky wall
        //set is wall jumping to false, 
        if (wallJumpCooldown > 0.2f && (!IsOnIce()) && !isWallJumping && !isTeleporting && !IsGrounded() && OnStickyWall())
        {
            isWallJumping = false;
            CancelInvoke(nameof(StopWallJumping));

            playerRigid.velocity = new Vector2(playerRigid.velocity.x, Mathf.Clamp(playerRigid.velocity.y, -wallSlidingSpeed, float.MaxValue));
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
        if (IsOnBouncy() && playerRigid.velocity.y <= 0f)
        {
            //Debug.Log("Bouncing");
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
        if (collision.CompareTag("Teleporter"))
        {
            if (currentTeleporter == collision.gameObject)
            {
                currentTeleporter = null;

            }
        }
    }





    //~~~~~~~ GROUND & WALL CHECKS ~~~~~~~\\
    //IsGrounded, IsOnWall Method orginally from https://www.youtube.com/watch?v=_UBpkdKlJzE
    //~~~ GROUNDED ~~~\\ 
    private bool IsGrounded()
    {
        //casts an invisble box from the players center down to see if the player is touching the ground
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);
        return raycastHit.collider != null;
    }


    //~~~ ICE ~~~\\ 
    private bool IsOnIce()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);

        //if we hit something, and that something has the Ice tag, return true, else, return false
        if (raycastHit.collider != null)
        {
            if (raycastHit.collider.CompareTag("Ice")) { return true; }
            else { return false; }
        }
        else { return false; }
    }


    //~~~ BOUNCE ~~~\\ 
    private bool IsOnBouncy()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);

        //if we hit something and that something has the Bouncy tag, return true, else return false
        if (raycastHit.collider != null)
        {
            if (raycastHit.collider.CompareTag("Bouncy")) { return true; }
            else { return false; }
        }
        else { return false; }
    }


    //~~~ STICKY WALL ~~~\\ 
    private bool OnStickyWall()
    {
        //casts an invisble box from the players center to whichever way the player is facing to see if we are touching a sticky wall
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.2f, wallLayer);
        return raycastHit.collider != null;
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
