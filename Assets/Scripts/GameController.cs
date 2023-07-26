using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    TurnBasedPlayerMovement player;

    // Update is called once per frame
    void Update()
    {
        if (player.arrivedAtKing) 
        {
            print("You reached the king!");
        }
    }
}
