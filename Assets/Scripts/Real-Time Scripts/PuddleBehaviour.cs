using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleBehaviour : MonoBehaviour
{
    bool isSliding = false;
    TurnBasedPlayerMovement player;
    private void OnTriggerEnter(Collider other)
    {
        print("Hit");
        if (other.CompareTag("Player") && !isSliding)
        {
            player = other.GetComponent<TurnBasedPlayerMovement>();
            StartCoroutine(SlidePlayer());
        }
    }
    private IEnumerator SlidePlayer()
    {
        isSliding = true;
        yield return new WaitUntil(() => player.canMove == true);
        StartCoroutine(player.MovePlayer(player.prevDir, false));
        isSliding = false;
    }
}
