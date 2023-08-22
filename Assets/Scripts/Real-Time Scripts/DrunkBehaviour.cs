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

    bool canMove = true;
    bool destinationSet = false;

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
        if (canMove && !destinationSet)
        {
            SetRandomDestinationOrReturnToOriginalPosition();
            agent.isStopped = true;
            destinationSet = true;
        }
        if (canMove && timeSinceMoved > moveCooldown)
        {
            print("start move coroutine" + ConvertToClosestDirection(agent.destination));
            StartCoroutine(MoveEnemy(ConvertToClosestDirection(agent.destination)));
        }
        else
        {
            timeSinceMoved += Time.deltaTime;
        }
    }

    void SetRandomDestinationOrReturnToOriginalPosition()
    {
        if (elapsedTime > waitTime && elapsedTime < waitTime + wanderTime)
        {
            print("set random destination vector");
            agent.destination = transform.position + new Vector3(Random.Range(-xLimit, xLimit), 0, Random.Range(-zLimit, zLimit));
        }
        else if (elapsedTime > waitTime + wanderTime) // reset condition
        {
            print("set original position destination vector");
            agent.destination = originalPosition;
            elapsedTime = 0.0f;
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
