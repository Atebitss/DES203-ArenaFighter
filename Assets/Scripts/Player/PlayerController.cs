using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D playerRigid;
    private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private bool controller = true;
    private InputPlayerControls controls;

    private Vector2 move;
    private Vector2 playerVelocity;

    private float jumpForce = 15f, moveForce = 5f;
    private bool inputA, inputD;

    void Awake()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

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
        playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0);
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
       // Debug.Log("A/space");
        
        playerRigid.velocity = new Vector3(playerVelocity.x, jumpForce, 0);
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
        //only jump when on the ground
        if (IsGrounded())
        {
            Jump();
        }
        
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







    private bool IsGrounded()
    {
        //casts an invisble ray from the players center down to see if the player is touching the ground
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.4f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool IsOnIce()
    {
        //casts an invisble ray to see if the player is touching the ground
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
        //casts an invisble ray to see if the player is touching the ground
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

}
