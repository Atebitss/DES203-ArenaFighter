using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRigid;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private bool controller = true;
    private InputPlayerControls controls;

    private Vector2 move;
    private Vector2 playerVelocity;

    private float jumpForce = 15f, moveForce = 5f;
    
    private bool inputA, inputD;

    private float previousXMovement;
    private float IceDecceleration = 0.95f; //must be between 1 and 0
    private float startingGravity;

    private float wallJumpCooldown;
    private float wallSlidingSpeed = 2f;
    private bool  isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingDuration = 0.4f;

    private float bounceMultiplier = 1.5f;
    private float bounceRebound = 1.25f;

    private GameObject currentTeleporter;
    private bool isTeleporting;
    private float teleportingDuration = 0.2f;


    void Awake()
    {
        //basic setup stuffs
        playerRigid = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        startingGravity = playerRigid.gravityScale;


        //Code from Brackeys, CONTROLLER INPUT in Unity! - https://youtu.be/p-3S73MaDP8
        controls = new InputPlayerControls();

        //define interations when controller input registered
        if (controller)
        {
            //CONTROLLER
            controls.PlayerInput.X.performed += ctx => X();
            controls.PlayerInput.Y.performed += ctx => Y();
            controls.PlayerInput.B.performed += ctx => B();
            controls.PlayerInput.Jump.performed += ctx => Jump();

            controls.PlayerInput.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
            controls.PlayerInput.Move.canceled += ctx => move = Vector2.zero;
        }
        else if (!controller)
        {
            //KEYBOARD
            controls.PlayerInput.A.started += ctx => inputA = true;
            controls.PlayerInput.A.canceled += ctx => inputA = false;
            controls.PlayerInput.D.started += ctx => inputD = true;
            controls.PlayerInput.D.canceled += ctx => inputD = false;
            controls.PlayerInput.Space.performed += ctx => Space();
        }
    }


    void FixedUpdate()
    {
        
        if (!controller)
        {
            if (inputA)
            {
                move = new Vector2(-1, 0);
            }
            if (inputD)
            {
                move = new Vector2(1, 0);
            }

            if (!inputA && !inputD)
            {
                move = Vector2.zero;
            }
        }

        playerVelocity = playerRigid.velocity;

        if (move.x != 0)
        {
            previousXMovement = playerRigid.velocity.x;

        }
     

       

        //~~~~~~~ MOVEMENT LOGIC ~~~~~~~\\ (Could pop this in a seperate script tbh)


        Flip();
        IceMovement();
        WallSlide();
        BounceMovement();


    }
    
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
                playerRigid.velocity = new Vector3(previousXMovement * IceDecceleration, playerVelocity.y, 0); //when no input, slide across|Speed decreases by the rate of IceDecceleration
                previousXMovement = playerRigid.velocity.x;
            }

        }
        else if (!OnStickyWall() && !isWallJumping && !isTeleporting)
        {

            playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0); //if not on ice and not on a wall , use normal movement logic
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

                playerRigid.velocity = new Vector2(playerRigid.velocity.x, Mathf.Clamp(playerRigid.velocity.y, -wallSlidingSpeed, float.MaxValue));
            }
            else
            {

            }


            if (Input.GetButtonDown("Jump"))
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;
    }
    private void BounceMovement()
    {
        if (IsOnBouncy() &&  playerRigid.velocity.y <= 0f) 
        {
            // float previousYMovement = playerRigid.velocity.y;
            // playerRigid.velocity = new Vector2(playerVelocity.x, -previousYMovement * bounceRebound);
            playerRigid.velocity = new Vector2(playerVelocity.x, jumpForce * bounceRebound);
            print("Yipeeee");

        }

       
    }








    //~~~~~~~PLAYER INPUT~~~~~~~\\
    void X()
    {
        Debug.Log("X");
    }

    void Y()
    {
        Debug.Log("Y");
    }

    void B()
    {
        Debug.Log("B");
    }

    void Jump()
    {
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
    //deassigns the current teleporter as the player exits it
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
    //teleports player to the current teleporters destination
    private void Teleport()
    {
        isTeleporting = true;
        Vector2 previousVelocity = playerVelocity;
        transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;

        Quaternion destinationRotation = currentTeleporter.GetComponent<Teleporter>().GetDestination().rotation;
        Quaternion currentRotation = currentTeleporter.GetComponent<Transform>().rotation;
        
        if (destinationRotation != currentRotation)
        {
            print("rotation is different, adding force");
           //will shoot out portal with the 80% of the velocity it fell into the portal with
            playerRigid.velocity = new Vector2((-previousVelocity.y) * 0.8f,previousVelocity.x);
            //will not work when teleporting to teleports facing left due to velocity direction, need to find a fix
        }



        Invoke(nameof(StopTeleporting), teleportingDuration);
    }

 

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    private void StopTeleporting()
    {
        isTeleporting = false;
    }


    //~~~~~~~KEYBOARD~~~~~~~\\
    void D()
    {
        inputD = true;
        Debug.Log("D");
    }

    void A()
    {
        inputA = true;
        Debug.Log("A");
    }

    void Space()
    {
        Jump();
    }



    //~~~~~~~CONTROLLER~~~~~~~\\
    void OnEnable()
    {
        controls.PlayerInput.Enable();
    }

    void OnDisable()
    {
        controls.PlayerInput.Disable();
    }



    //~~~~~~~PLAYER POSITION & VELOCITY~~~~~~~\\
    public float GetPlayerX()
    {
        return this.transform.position.x;
    }

    public float GetPlayerY()
    {
        return this.transform.position.y;
    }

    public void SetPlayerPos(Vector2 newPos)
    {
        playerRigid.position = newPos;
    }

    public float GetPlayerXVelocity()
    {
        return playerRigid.velocity.x;
    }

    public float GetPlayerYVelocity()
    {
        return playerRigid.velocity.y;
    }




    //~~~~~~~PLAYER FORCES & SPEED~~~~~~~\\
    public void SetJumpForce(float newJF)
    {
        jumpForce = newJF;
    }

    public void SetMoveForce(float newMF)
    {
        moveForce = newMF;
    }






    //~~~~~~~ GROUND CHECKS ~~~~~~~\\

    //IsGrounded, IsOnWall Method orginally from https://www.youtube.com/watch?v=_UBpkdKlJzE
    private bool IsGrounded()
    {
        //casts an invisble box from the players center down to see if the player is touching the ground
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.5f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool IsOnIce()
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
    private bool IsOnBouncy()
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
    private bool OnStickyWall()
    {
        //casts an invisble box from the players center to whichever way the player is facing to see if we are touching a sticky wall
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0,new Vector2(transform.localScale.x, 0), 0.2f, wallLayer);
        return raycastHit.collider != null;
    }
 
   
}
