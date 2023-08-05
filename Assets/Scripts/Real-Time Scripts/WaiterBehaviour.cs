using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaiterBehaviour : MonoBehaviour
{
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;
    [SerializeField] private float xLimit;
    [SerializeField] private float zLimit;

    [SerializeField] private Transform[] targetPoints;
    private int i = 2;

    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (targetPoints.Length != 0)
        {
            transform.position = targetPoints[0].position;
            agent.destination = targetPoints[1].position;
        }
        else
        {
            agent.destination = new Vector3(Random.Range(-xLimit, xLimit), 0, Random.Range(-zLimit, zLimit));
        }
    }

    private void Update()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    if (targetPoints.Length != 0)
                    {
                        i++;
                        if (i >= targetPoints.Length)
                            i = 0;
                        //print("Reached Destination " + i);
                        agent.destination = targetPoints[i].position;
                    }
                    else
                    {
                        agent.destination = new Vector3(Random.Range(-xLimit, xLimit), 0, Random.Range(-zLimit, zLimit));
                    }
                }
            }
        }
    }
}
