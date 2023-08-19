using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DrunkBehaviour : MonoBehaviour
{
    [SerializeField] LayerMask obstacleLayer;

    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 10f;
    [SerializeField] private float moveCooldown = 1f;
    private float elapsedTime = -1f;
    private float timeSinceMoved = 0f;
    private float waitTime;
    public float wanderTime = 10f;
    [SerializeField] private float xLimit;
    [SerializeField] private float zLimit;

    Vector3 originalPosition;

    bool canMove = false;

    [SerializeField] int animationFrames;
    [SerializeField] int animationLength;

    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalPosition = transform.position;
        agent.destination = transform.position;

        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    SetRandomDestinationOrReturnToOriginalPosition();
                    agent.isStopped = true;
                    Vector3 destination = new Vector3(agent.destination.x, agent.destination.y, agent.destination.z);
                    if (canMove && timeSinceMoved > moveCooldown)
                    {
                        StartCoroutine(MovePlayer(ConvertToClosestDirection(destination),false));
                    }
                    else
                    {
                        timeSinceMoved += Time.deltaTime;
                    }
                }
            }
        }
    }

    void SetRandomDestinationOrReturnToOriginalPosition()
    {
        if (elapsedTime > waitTime && elapsedTime < waitTime + wanderTime)
        {
            agent.destination = transform.position + new Vector3(Random.Range(-xLimit, xLimit), 0, Random.Range(-zLimit, zLimit));
        }
        else if (elapsedTime > waitTime + wanderTime) // reset condition
        {
            agent.destination = originalPosition;
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    elapsedTime = 0.0f;
                }
            }
        }
    }

    public IEnumerator MovePlayer(Vector3 moveDirection, bool completeTurn)
    {
        if (!Physics.Raycast(transform.position, moveDirection, 1, obstacleLayer) && canMove)
        {
            canMove = false;
            Vector3 startPos = transform.position;
            Vector3 nextPos = transform.position + moveDirection;
          
                print("should mve");
                transform.position = Vector3.Lerp(startPos, nextPos,(float)animationFrames);
                timeSinceMoved = 0.0f;
            transform.position = nextPos;
            canMove = true;
        }
        yield return new WaitForSeconds(0.1f);
    }

    public Vector3 ConvertToClosestDirection(Vector3 inputVector)
    {
        Vector3 normalizedVector = inputVector.normalized;

        if (Mathf.Abs(normalizedVector.x) >= Mathf.Abs(normalizedVector.y) && Mathf.Abs(normalizedVector.x) >= Mathf.Abs(normalizedVector.z))
        {
            return new Vector3(Mathf.Sign(normalizedVector.x), 0, 0);
        }
        else if (Mathf.Abs(normalizedVector.y) >= Mathf.Abs(normalizedVector.x) && Mathf.Abs(normalizedVector.y) >= Mathf.Abs(normalizedVector.z))
        {
            return new Vector3(0, Mathf.Sign(normalizedVector.y), 0);
        }
        else
        {
            return new Vector3(0, 0, Mathf.Sign(normalizedVector.z));
        }
    }
}
