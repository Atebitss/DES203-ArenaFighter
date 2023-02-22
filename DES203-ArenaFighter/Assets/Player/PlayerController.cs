using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigid;
    private InputPlayerControls controls;
    private Vector2 move;
    private Vector2 playerVelocity;
    private float jumpForce = 15f, moveForce = 5f;

    void Awake()
    {
        //Code from Brackeys, CONTROLLER INPUT in Unity! - https://youtu.be/p-3S73MaDP8
        controls = new InputPlayerControls();

        //define interations when controller input registered
        controls.PlayerInput.X.performed += ctx => X();
        controls.PlayerInput.Y.performed += ctx => Y();
        controls.PlayerInput.B.performed += ctx => B();
        controls.PlayerInput.Jump.performed += ctx => Jump();

        controls.PlayerInput.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.PlayerInput.Move.canceled += ctx => move = Vector2.zero;
    }


    void Update()
    {
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
        Debug.Log("A");
        //do jump
        playerRigid.velocity = new Vector3(playerVelocity.x, jumpForce, 0);
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
}