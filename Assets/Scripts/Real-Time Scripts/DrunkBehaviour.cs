using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DrunkBehaviour : MonoBehaviour
{
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] WarningDangerMarker dangerMarker;

    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 10f;
    [SerializeField] private float moveCooldown = 1f;
    private float elapsedTime = -1f;
    private float timeSinceMoved = 0f;
    private float waitTime;
    public float wanderTime = 10f;
    private float timeWhenDestinationSet = 0.0f;
    [SerializeField] private float xLimit;
    [SerializeField] private float zLimit;

    Vector3 originalPosition;

    bool canMove = true;
    bool destinationSet = false;
    bool destinationSetCooldownComplete = false;
    bool returnToOriginalPos = false;

    [SerializeField] int animationFrames;
    [SerializeField] int animationLength;

    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position;
        agent.destination = transform.position;

        waitTime = Random.Range(minWaitTime, maxWaitTime);
        SetRandomDestinationOrReturnToOriginalPosition();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
       
        if (canMove)
        {
            // Set destination
            if (!destinationSet)
            {
                SetRandomDestinationOrReturnToOriginalPosition();
                agent.isStopped = true;
                destinationSet = true;
                timeWhenDestinationSet = Time.time;
            }
            // Warning Finished
            if (Time.time > timeWhenDestinationSet + 2f)
            {
                dangerMarker.dangerImminent = false;
                // Move to random pos
                if (timeSinceMoved > moveCooldown && !returnToOriginalPos)
                {
                    StartCoroutine(MoveEnemy(ConvertToClosestDirection(agent.destination)));
                }
                else
                {
                    timeSinceMoved += Time.deltaTime;
                }
                // Return to original pos
                if (returnToOriginalPos == true && Vector3.Distance(transform.position, originalPosition) < 0.1f)
                {
                    elapsedTime = 0.0f;
                    returnToOriginalPos = false;
                    destinationSet = false;
                    canMove = true;
                }
                else if (timeSinceMoved > moveCooldown && returnToOriginalPos)
                {
                    Vector3 directionToOriginal = (originalPosition - transform.position).normalized;
                    Vector3 roundedDirection = ConvertToClosestDirection(directionToOriginal);
                    agent.destination = transform.position + roundedDirection;
                    //print("return");
                    StartCoroutine(MoveEnemy(roundedDirection));
                }
                else if (timeSinceMoved < moveCooldown)
                {
                    timeSinceMoved += Time.deltaTime;
                }
            }
            else // Show warning
            {
                dangerMarker.transform.position = transform.position + ConvertToClosestDirection(agent.destination);
                dangerMarker.dangerImminent = true;
            }
        }
    }

    Vector3 ClosestDirection(Vector3 direction) 
    {
        Vector3 closestDirection = direction + transform.position;
        closestDirection = closestDirection.normalized;

        return closestDirection;
    }

    void SetRandomDestinationOrReturnToOriginalPosition()
    {
        if (elapsedTime > waitTime && elapsedTime < waitTime + wanderTime)
        {
            //print("set random destination vector");
            agent.destination = new Vector3(Random.Range(-xLimit, xLimit), 0, Random.Range(-zLimit, zLimit));
        }
        else if (elapsedTime > waitTime + wanderTime) // reset condition
        {
            returnToOriginalPos = true;
        }

    }

    public IEnumerator MoveEnemy(Vector3 moveDirection)
    {
        if (!Physics.Raycast(transform.position, moveDirection, 1, obstacleLayer) && canMove)
        {
            canMove = false;

            Vector3 destinationPos = transform.position + moveDirection;

            while (Vector3.Distance(transform.position, destinationPos) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, destinationPos, agent.speed * Time.deltaTime);
                yield return null;
            }

            transform.position = destinationPos;
            canMove = true;
            destinationSet = false;
            timeSinceMoved = 0.0f;
        }
        else if (Physics.Raycast(transform.position, moveDirection, 1, obstacleLayer))
        {
            SetRandomDestinationOrReturnToOriginalPosition();
        }
    }


    public Vector3 ConvertToClosestDirection(Vector3 inputVector)
    {
        Vector3 normalizedVector = inputVector.normalized;

        Vector3 closestDirection = Vector3.zero;
        float maxComponent = Mathf.Max(Mathf.Abs(normalizedVector.x), Mathf.Abs(normalizedVector.y), Mathf.Abs(normalizedVector.z));

        if (Mathf.Abs(normalizedVector.x) == maxComponent)
        {
            closestDirection = new Vector3(Mathf.Sign(normalizedVector.x), 0, 0);
        }
        else if (Mathf.Abs(normalizedVector.y) == maxComponent)
        {
            closestDirection = new Vector3(0, Mathf.Sign(normalizedVector.y), 0);
        }
        else if (Mathf.Abs(normalizedVector.z) == maxComponent)
        {
            closestDirection = new Vector3(0, 0, Mathf.Sign(normalizedVector.z));
        }

        return closestDirection;
    }
}
