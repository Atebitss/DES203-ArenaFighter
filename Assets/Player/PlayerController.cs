using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigid;
    private Vector2 move;
    private Vector2 playerVelocity;
    private float jumpForce = 15f, moveForce = 5f;

    void Awake()
    {
    }


    void FixedUpdate()
    {
        playerVelocity = playerRigid.velocity;   //update current velocity Vector2
        playerRigid.velocity = new Vector3(move.x * moveForce, playerVelocity.y, 0);   //Player's rigid component's velocity set to 1/-1 * 15, 0/15
    }



    //~~~~~~~PLAYER CONTROL~~~~~~~\\
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 movement = ctx.ReadValue<Vector2>();
        //Debug.Log(movement);
        //Debug.Log(ctx.control.displayName);
        /*if (ctx.control.displayName == "A" || ctx.control.displayName == "D")
        {
            movement.x/=2;   //keyboard control -1/1 is twice as fast controller -1 to 1
        }*/

        move = new Vector3(movement.x, 0, playerVelocity.y);
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        //do jump
        //Debug.Log("A/space");
        playerRigid.velocity = new Vector3(playerVelocity.x, jumpForce, 0);
    }



    //~~~~~~~PLAYER POSITION & VELOCITY~~~~~~~\\
    public float GetPlayerX()
    {
        return this.transform.position.x;
    }
    public void SetPlayerX(float newX)
    {
        Vector2 newPos = new Vector2(newX, playerRigid.position.y);
        playerRigid.position = newPos;
    }

    public float GetPlayerY()
    {
        return this.transform.position.y;
    }
    public void SetPlayerY(float newY)
    {
        Vector2 newPos = new Vector2(playerRigid.position.x, newY);
        playerRigid.position = newPos;
    }


    public void GetPlayerPos(Vector2 newPos)
    {
        playerRigid.position = newPos;
    }
    public void SetPlayerPos(Vector2 newPos)
    {
        playerRigid.position = newPos;
    }


    public float GetPlayerXVelocity()
    {
        return playerRigid.velocity.x;
    }
    public void SetPlayerXVelocity(float newXVel)
    {
        Vector3 newVel = new Vector3(newXVel, playerRigid.velocity.y);
        playerRigid.velocity = newVel;
    }

    public float GetPlayerYVelocity()
    {
        return playerRigid.velocity.y;
    }
    public void SetPlayerYVelocity(float newYVel)
    {
        Vector3 newVel = new Vector3(playerRigid.velocity.x, newYVel);
        playerRigid.velocity = newVel;
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

    public void SetPlayerGravity(float newPG)
    {
        playerRigid.gravityScale = newPG;
    }
}
