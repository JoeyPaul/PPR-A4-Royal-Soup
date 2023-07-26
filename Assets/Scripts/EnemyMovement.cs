using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] LayerMask obstacleLayer;

    public List<Transform> navigationNodes = new List<Transform>();

    private Transform currentNode;
    private int currentNodeIterator;

    public enum EnemyType
    {
        Waiter,
        Patrol,
        Random,
        None
    }

    public EnemyType type;

    private void Start()
    {
        if (type == EnemyType.Patrol && navigationNodes[0] != null)
        {
            currentNode = navigationNodes[0];
            currentNodeIterator = 0;
        }
    }

    public static void MoveEnemies()
    {
        foreach(EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
        {
            enemy.Move(enemy.type);
        }
    }

    public void Move(EnemyType enemyType)
    {
        switch(type)
        {
            case EnemyType.Waiter:
                break;
            case EnemyType.Patrol:
                PatrolMovementBehaviour();
                break;
            case EnemyType.Random:
                RandomMovementBehaviour();
                break;
            case EnemyType.None:
                break;
        }
    }

    void PatrolMovementBehaviour()
    {
        if (currentNode != null)
        {
            if (transform.position != currentNode.position)
            {
                CalculateDirectionMove();
            }
            else // Have reached the node, check if there are more.
            {
                if (currentNodeIterator >= 0 && currentNodeIterator < navigationNodes.Count)
                {
                    currentNode = navigationNodes[currentNodeIterator++];
                    CalculateDirectionMove();
                }
                else // No more nodes, reset to the beginning of the list
                {
                    currentNode = navigationNodes[0];
                    currentNodeIterator = 0;
                    CalculateDirectionMove();
                }
            }
        }
    }

    // Used in the Patrol Behavior to calculate the direction to move in.
    void CalculateDirectionMove()
    {
        if (transform.position != currentNode.position)
        {
            // Calculate the direction vector from this.transform to the Node
            Vector3 direction = (currentNode.position - transform.position).normalized;
            //print(direction);
            // Compare which tile is closer to the node
            float x = direction.x;
            float z = direction.z;
            if (Mathf.Abs(x) >= Mathf.Abs(z))
            {
                // Move the enemy in the direction that is closer to the node. 
                // using the ternary operator to correct the movement vector to a fixed integer value
                transform.position += new Vector3((Mathf.Abs(x - (-1f)) < Mathf.Abs(x - 1f)) ? -1f : 1f, 0, 0);
            }
            else if (Mathf.Abs(z) >= Mathf.Abs(x))
            {
                transform.position += new Vector3(0, 0, (Mathf.Abs(z - (-1f)) < Mathf.Abs(z - 1f)) ? -1f : 1f);
            }
        }
    }

    // The same random movement, but with added obstacle detection, it will not move if it detects an obstacle
    // in the direction it is moving
    void RandomMovementBehaviour()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0:
                if (!HasDetectedObstacle(Vector3.right))
                    transform.position += Vector3.right;
                break;
            case 1:
                if (!HasDetectedObstacle(-Vector3.right))
                    transform.position += -Vector3.right;
                break;
            case 2:
                if (!HasDetectedObstacle(Vector3.forward))
                    transform.position += Vector3.forward;
                break;
            case 3:
                if (!HasDetectedObstacle(-Vector3.forward))
                    transform.position += -Vector3.forward;
                break;
        }
    }

    bool HasDetectedObstacle(Vector3 moveDir)
    {
        if (moveDir == Vector3.zero) return false;
        else
            return RayCollisionDetected(moveDir);
    }

    // Shoots a ray in the direction checking for collisions with the obstacle layer
    bool RayCollisionDetected(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction, 1, obstacleLayer))
        {
            return true;
        }
        else
            return false;
    }
}
