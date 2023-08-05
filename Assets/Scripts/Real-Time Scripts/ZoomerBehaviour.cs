using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class ZoomerBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] targetPoints;
    private int point = 2;
    // Doors will open a few seconds before movement
    [SerializeField] Transform[] leftDoors;
    [SerializeField] Transform[] rightDoors;
    bool doorsOpen = false;
    bool openTheDoors = false;

    private NavMeshAgent agent;

    public bool currentlyMoving = false;

    private float zoomCountdown = -1f;
    public float zoomInterval = 3f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (targetPoints.Length != 0)
        {
            transform.position = targetPoints[0].position;
            MoveToNextPoint(); // Move to the first point immediately upon starting
        }
    }

    void Update()
    {
        if (agent.hasPath)
        {
            if (HasReachedDestination())
            {
                MoveToNextPoint();
            }
        }
    }

    bool HasReachedDestination()
    {
        return agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude == 0f;
    }

    void MoveToNextPoint()
    {
        if (targetPoints.Length != 0)
        {
            point++;
            if (point >= targetPoints.Length)
            {
                point = 0;
            }
            agent.SetDestination(targetPoints[point].position);
        }
    }

}
