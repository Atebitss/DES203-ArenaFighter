using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private PlayerController playerController;
    private Vector3 startPos = new Vector3(0, 0, 0);
    private LevelScript ls;

    private void Awake()
    {
        //sets spawn location to ordered location
        //flips the player depending on the position of the spawn location
        //sets the input to the players location

        if (playerPrefab != null)
        {
            Debug.Log("New PlayerInputHandler");
            ls = GameObject.Find("LevelController").GetComponent<LevelScript>();
            Vector2 SpawnPointLocation = ls.GetNextSpawnPoint();

            playerController = GameObject.Instantiate(playerPrefab, SpawnPointLocation, transform.rotation).GetComponent<PlayerController>();

            if (SpawnPointLocation.x < 0) { playerController.transform.localScale = new Vector2(1, 1); }
            else if (SpawnPointLocation.x > 0) { playerController.transform.localScale = new Vector2(-1, 1); }

            transform.parent = playerController.transform;
            transform.position = playerController.transform.position;
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        playerController.OnMove(ctx);
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            playerController.OnJump(ctx);
        }
    }
}
