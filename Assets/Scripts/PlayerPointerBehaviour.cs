using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPointerBehaviour : MonoBehaviour
{
    TurnBasedPlayerMovement player;
    [SerializeField] Vector3 direction;
    private void Start()
    {
        player = FindObjectOfType<TurnBasedPlayerMovement>();
    }
    private void OnMouseDown()
    {
        StartCoroutine(player.MovePlayer(direction, true));
    }
}
