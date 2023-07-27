using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    TurnBasedPlayerMovement player;
    [SerializeField]
    GameObject freeLookCamera;

    // Update is called once per frame
    void Update()
    {
        if (player.arrivedAtKing) 
        {
            print("You reached the king!");
        }
        if (Input.GetMouseButtonDown(0)) // Mouse 1 is pressed
        {
            freeLookCamera.SetActive(true);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            freeLookCamera.SetActive(false);
        }
    }
}
