using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    bool doorOpen = false;
    public float doorOpeningDuration = 2.0f;

    public GameObject door;

    // Cast a ray from above, going down towards the player, to detect if the "Player" tagged object is currently on the same tile.
    // Then starts door opening coroutine
    void CheckForPlayer()
    {
        if (!doorOpen)
        {
            Vector3 offset = new Vector3(0, 2, 0);
            Vector3 raycastStart = transform.position + offset;
            Vector3 raycastDirection = -Vector3.up;
            float maxDistance = 2f;
            RaycastHit hitInfo;
            Debug.DrawRay(raycastStart, raycastDirection, Color.red);
            if (Physics.Raycast(raycastStart, raycastDirection, out hitInfo, maxDistance))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                    // "Player" was detected.
                    //print(hitInfo);
                    transform.position += new Vector3(0,-0.04f,0);
                    StartCoroutine(OpenDoor());
                    doorOpen = true;

                }
            }
        }
    }

    private void Update()
    {
        CheckForPlayer();
    }

    IEnumerator OpenDoor()
    {
        Vector3 startPosition = door.transform.position;
        Vector3 targetPosition = startPosition - Vector3.up * 2.0f; // Move downwards by 2 units

        float elapsedTime = 0f;

        while (elapsedTime < doorOpeningDuration)
        {
            // Calculate the lerp percentage
            float t = elapsedTime / doorOpeningDuration;

            // Smoothly lerp the position downwards
            door.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Increment the elapsed time with the delta time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the object reaches the target position exactly
        door.transform.position = targetPosition;

        // Any additional actions after the lerping is completed can be placed here

        Debug.Log("Lerping completed!");
    }
}
