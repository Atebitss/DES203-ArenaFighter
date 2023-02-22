using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevelScript : MonoBehaviour
{
    [SerializeField] private PlayerController playerScript;
    [SerializeField] [Range(1, 25)] private float playerMoveForce = 25f;
    [SerializeField] [Range(10, 50)] private float playerJumpForce = 25f;

    void Start()
    {
        playerScript.SetMoveForce(playerMoveForce);
        playerScript.SetJumpForce(playerJumpForce);
    }
}