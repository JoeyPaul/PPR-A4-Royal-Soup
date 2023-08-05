using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZoomerEnemy : MonoBehaviour
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

    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        if (targetPoints.Length != 0)
        {
            transform.position = targetPoints[0].position;
            agent.destination = targetPoints[1].position;
        }

    }

    void Update()
    {
        zoomCountdown += Time.deltaTime;

        if (!currentlyMoving && zoomCountdown >= zoomInterval) 
        {   // Every zoomInterval seconds
            
                currentlyMoving = true;
                //print(zoomCountdown);
                zoomCountdown = 0f;
            
        }
        if (currentlyMoving)
        {
            StartZoomerMovement();
        }

        if (zoomCountdown >= zoomInterval - 2f)
        {
            openTheDoors = true;
        }
        if (openTheDoors) 
        {
            OpenDoors(leftDoors, -90f, 0.03f);
            OpenDoors(rightDoors, 90f, -0.03f);
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
                        {
                            point = 0;
                        }
                        //print("Reached Destination " + point);
                        currentlyMoving = false;
                        agent.destination = targetPoints[point].position;
                    }
                }
            }
        }
    }

    private void OpenDoors(Transform[] doors, float targetRotationAngle, float targetZMoveAmount)
    {
        float rotationSpeed = 1f;
        foreach (var door in doors)
        {
            float currentRotationAngle = door.rotation.eulerAngles.y;
            if (Quaternion.Angle(door.rotation, Quaternion.Euler(0f, targetRotationAngle, 0f)) <= 0.1f)
            {
                doorsOpen = true;
                openTheDoors = false;
            }
            else
            {
                Quaternion targetRotation = Quaternion.Euler(0f, targetRotationAngle, 0f);
                Vector3 targetPosition = door.position + new Vector3(0f, 0f, targetZMoveAmount);
                door.rotation = Quaternion.Lerp(door.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                door.position = Vector3.Lerp(door.position, targetPosition, (rotationSpeed * 2) * Time.deltaTime);
            }
        }
    }
}
