using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            TurnBasedPlayerMovement player = other.GetComponent<TurnBasedPlayerMovement>();
            //player.ismovin
        }
    }
}
