using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZoomerEnemy : MonoBehaviour
{
    [SerializeField] private Transform[] targetPoints;
    private int point = 2;

    private NavMeshAgent agent;

    private bool currentlyMoving = false;
    private float levelStartTime;

    void Start()
    {
        levelStartTime = Time.time;

        agent = GetComponent<NavMeshAgent>();
        if (targetPoints.Length != 0)
        {
            transform.position = targetPoints[0].position;
            agent.destination = targetPoints[1].position;
        }

    }

    void Update()
    {
        float timeSinceLevelStart = Time.time - levelStartTime;

        if (!currentlyMoving) 
        {   // Every 5 seconds since level start
            if (Mathf.Approximately(timeSinceLevelStart % 5f, 0f))
            {
                currentlyMoving = true;
                print(timeSinceLevelStart);
            }
        }
        else
        {
            StartZoomerMovement();
        }
    }

    void StartZoomerMovement()
    {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        if (targetPoints.Length != 0)
                        {
                            point++;
                            if (point >= targetPoints.Length)
                                point = 0;
                            print("Reached Destination " + point);
                            agent.destination = targetPoints[point].position;
                            currentlyMoving = false;
                        }
                    }
                }
            }
    }
}
