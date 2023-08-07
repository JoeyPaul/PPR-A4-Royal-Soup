using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayWind : MonoBehaviour
{
    public float swaySpeed = 1f; 
    public float swayAmount = 0.1f;
    public float rotationSpeed = 1f; 
    public float rotationAmount = 15f; 

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float delay;
    private float timeOffset;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        delay = Random.Range(0.5f, 2.0f);
        timeOffset = Random.Range(0f, 10f);
        StartCoroutine(SwayObject(delay));
    }

    private IEnumerator SwayObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        while (true)
        {
            // Calculate the sway movement using Sin
            float swayOffset = Mathf.Sin(Time.time + timeOffset) * swaySpeed * swayAmount;
            float rotationOffset = Mathf.Sin((Time.time + timeOffset) * rotationSpeed) * rotationAmount;

            // Apply the sway to the object
            Vector3 swayPosition = initialPosition + Vector3.right * swayOffset;
            Quaternion swayRotation = initialRotation * Quaternion.Euler(0f, 0f, rotationOffset);
            transform.localPosition = swayPosition;
            transform.localRotation = initialRotation * swayRotation;

            yield return null;
        }
    }
}
