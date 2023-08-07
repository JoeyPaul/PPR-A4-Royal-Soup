using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Trap : MonoBehaviour
{
    GameController game;
    TurnBasedPlayerMovement player;
    MeshRenderer renderer;

    [SerializeField] int interval;
    [SerializeField] int startingTurn;
    private int myCount;

    private int previousTurn;

    //public enum ActiveEvery
    //{
    //    Second_Turn,
    //    Third_Turn,
    //    Fourth_Turn
    //}

    //public ActiveEvery trapActive;
    public int trapDamageAmount = 33;
    int turnWhenHit;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        game = FindObjectOfType<GameController>();
        player = FindObjectOfType<TurnBasedPlayerMovement>();
    }

    // Cast a ray from above, going down towards the player, to detect if the "Player" tagged object is currently on the same tile.
    // Then deducts the soup.
    void CheckForPlayer()
    {
        if (turnWhenHit != game.currentTurn)
        {
            Vector3 offset = new Vector3(0, 2, 0);
            Vector3 raycastStart = transform.position + offset;
            Vector3 raycastDirection = -Vector3.up;
            float maxDistance = 2f;
            RaycastHit hitInfo;
            Debug.DrawRay(raycastStart, raycastDirection, Color.red);
            if (Physics.Raycast(raycastStart, raycastDirection, out hitInfo, maxDistance))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                    // "Player" was detected.
                    //print(hitInfo);
                    player.SpillSoup(); // Deduct Soup
                    // Store the turn when the player was hit
                    turnWhenHit = game.currentTurn;
                }
            }
        }
    }
    void UpdateCounter()
    {
        myCount++;
        if(myCount == interval)
        {
            myCount = 0;
            renderer.enabled = true;
            CheckForPlayer();
        }
        else
        {
            renderer.enabled = false;
        }
    }
    
    void Update()
    {
        if (game.currentTurn <= startingTurn)
            return;

        if(previousTurn != game.currentTurn)
        {
            previousTurn = game.currentTurn;
            UpdateCounter();
        }
    }
}
