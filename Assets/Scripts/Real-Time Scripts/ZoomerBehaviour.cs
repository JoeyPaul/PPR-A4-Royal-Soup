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
    [SerializeField] WarningDangerMarker warningMarker;
    bool doorsOpen = false;
    bool openTheDoors = false;

    public bool currentlyMoving = false;

    private float elapsedTime = -1f;
    public float zoomCountdownDuration = 5f;
    public float speed = 2f;

    private void Start()
    {
        if (targetPoints.Length != 0)
        {
            transform.position = targetPoints[0].position;
            MoveToNextPoint(); // Move to the first point immediately upon starting
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < zoomCountdownDuration)
        {
            if (elapsedTime >= zoomCountdownDuration - 2f && !openTheDoors) 
            {
                StartCoroutine(OpenDoors(leftDoors, -90f, 0.03f));
                StartCoroutine(OpenDoors(rightDoors, 90f, -0.03f));
                warningMarker.dangerImminent = true;
                //print("Open Doors Called.");
            }
        }
        else
        {
            StartZoomerMovement();
        }
    }

    void StartZoomerMovement()
    {
        if (targetPoints.Length != 0)
        {
            if (HasReachedDestination() && !currentlyMoving)
            {
                MoveToNextPoint();
            }
        }
    }

    bool HasReachedDestination()
    {   
        // Returns true when the transform is within 1 unit cube of the target position.
        if (Vector3.Distance(transform.position, targetPoints[point].transform.position) <= 0.5f)
        {
            print("reached");
            return true;
        }
        return false;
    }

    void MoveToNextPoint()
    {
        if (targetPoints.Length != 0)
        {
            point++;
            if (point >= targetPoints.Length)
            {
                if (doorsOpen)
                {
                    StartCoroutine(delayClose(1f));
                }
                elapsedTime = 0.0f;
                point = 0;
            }
            //print("How many times");
            StartCoroutine(Move());
        }
    }

    IEnumerator delayClose(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(OpenDoors(leftDoors, 0f, -0.03f));
        StartCoroutine(OpenDoors(rightDoors, 0f, 0.03f));
        warningMarker.dangerImminent = false;
    }

    IEnumerator Move()
    {
        currentlyMoving = true;

        Vector3 targetPosition = targetPoints[point].transform.position;
        Vector3 startingPosition = transform.position;
        float duration = 2f;

        float elapsedTimeMove = 0f;
        while (elapsedTimeMove < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTimeMove / duration * speed);
            elapsedTimeMove += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        currentlyMoving = false;
    }

    private IEnumerator OpenDoors(Transform[] doors, float targetRotationAngle, float targetZMoveAmount)
    {
        openTheDoors = true;
        float rotationSpeed = 5f;

        foreach (var door in doors)
        {
            bool doorFullyOpened = false;

            while (!doorFullyOpened)
            {
                if (Quaternion.Angle(door.rotation, Quaternion.Euler(0f, targetRotationAngle, 0f)) <= 0.1f)
                {
                    doorsOpen = true;
                    doorFullyOpened = true;
                    openTheDoors = false;
                }
                else
                {
                    Quaternion targetRotation = Quaternion.Euler(0f, targetRotationAngle, 0f);
                    Vector3 targetPosition = door.position + new Vector3(0f, 0f, targetZMoveAmount);
                    door.rotation = Quaternion.Lerp(door.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    door.position = Vector3.Lerp(door.position, targetPosition, (rotationSpeed * 2) * Time.deltaTime);
                }
                yield return null;
            }
        }
    }

}
