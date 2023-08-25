using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsCamera : MonoBehaviour
{
    [SerializeField]
    Camera camera;

    void Update()
    {
        Vector3 directionToCamera = camera.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

        // Apply the rotation to the object's transform
        transform.rotation = targetRotation;
    }
}
