using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DrunkBehaviour : MonoBehaviour
{
    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 10f;
    private float elapsedTime = -1f;
    private float waitTime;
    public float wanderTime = 10f;
    [SerializeField] private float xLimit;
    [SerializeField] private float zLimit;

    Vector3 originalPosition;

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
                    if (elapsedTime > waitTime && elapsedTime < waitTime + wanderTime)
                    {
                        agent.destination = new Vector3(Random.Range(-xLimit, xLimit), 0, Random.Range(-zLimit, zLimit));
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
            }
        }
    }
}
