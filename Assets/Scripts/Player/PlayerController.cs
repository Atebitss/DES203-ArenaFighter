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
    [SerializeField] private float bounceMultiplier = 1.5f;
    private bool inputA, inputD;

    private float previousXMovement;
    private float IceDecceleration = 0.9f; //must be between 1 and 0
    private float startingGravity;
    private float wallJumpCooldown;



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

        //~~~~~~~ MOVEMENT LOGIC ~~~~~~~\\ (Could pop this in a seperate script tbh)

        //flips the player around when moving left or right
        if (move.x > 0.01f)
            transform.localScale = Vector3.one;
        else if (move.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);


        // when we have any x movment input, store the current x velocity for use in ice physics
        if (move.x != 0) 
         {
            previousXMovement = playerRigid.velocity.x;

        }


        //Ice Movement Logic
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
        else if (!OnStickyWall())
        {
           
            playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0); //if not on ice, use normal movement logic
        }





        //wall jumping code orignally from https://www.youtube.com/watch?v=_UBpkdKlJzE || make sure to clean this up/ not outirght copy this later
        if (wallJumpCooldown > 0.2f && (!IsOnIce()))
        {
            playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0);

            if (OnStickyWall() && !IsGrounded())
            {
                playerRigid.gravityScale = 0; //change this if we want to slide down wall!!
                playerRigid.velocity = Vector2.zero;
            }
            else
                playerRigid.gravityScale = startingGravity;

            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
            wallJumpCooldown += Time.deltaTime;


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
       if (IsGrounded()) //jump mechanics when on bouncy terrain
        {
            if (IsOnBouncy())
            {
                playerRigid.velocity = new Vector3(playerVelocity.x, jumpForce * bounceMultiplier, 0);
            }
            else 
            {
                playerRigid.velocity = new Vector3(playerVelocity.x, jumpForce, 0);
            }
            
        }
       else if (OnStickyWall() & !IsGrounded()) //jump mechanics when on sticky walls,(walljumping)
        {
            print("should be jumping");
           if (move.x == 0) //when there is no x input,when we jump just drop down from the wall
            {
                playerRigid.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else //else, jump away from the wall and up a little
            {
                //when facing right transform.localScale.x = 1, and -1 when facing left -> MathF.Sign returns -1 or 1 depending on sign -> function returns 1 or -1 depending on direction | multiplied by -1 at beginnibg to invert
                playerRigid.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 12);
                wallJumpCooldown = 0;
            }
           
        }
       
      

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
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.4f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool IsOnIce()
    {
        
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.4f, groundLayer);

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
        
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.4f, groundLayer);

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
