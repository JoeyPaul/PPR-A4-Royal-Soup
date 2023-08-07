using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningDangerMarker : MonoBehaviour
{
    public Renderer renderer; // Assign the Renderer component with the material to lerp
    public float lerpDuration = 2f; // Time taken to complete one full lerp (in seconds)
    public float minAlpha = 0f; // Minimum alpha value (fully transparent)
    public float maxAlpha = 1f; // Maximum alpha value (fully opaque)
    public float pauseDuration = 1f;

    private bool currentlyRunningCoroutine = false;
    public bool dangerImminent = false;

    private void Update()
    {
        if (!currentlyRunningCoroutine && dangerImminent)
        {
            renderer.enabled = true;
            StartCoroutine(LerpAlpha());
        }
        else if (!dangerImminent)
        {
            renderer.enabled = false;
        }
    }



    private IEnumerator LerpAlpha()
    {
        currentlyRunningCoroutine = true;
        float startTime;
        float alphaValue;

        while (true)
        {
            if (dangerImminent == false) break; 
            startTime = Time.time;
            // Lerp from minAlpha to maxAlpha
            while (Time.time < startTime + lerpDuration)
            {
                alphaValue = Mathf.Lerp(minAlpha, maxAlpha, (Time.time - startTime) / lerpDuration);
                SetAlpha(alphaValue);
                yield return null;
            }
            yield return new WaitForSeconds(pauseDuration);

            // Lerp back from maxAlpha to minAlpha
            startTime = Time.time;
            while (Time.time < startTime + lerpDuration)
            {
                alphaValue = Mathf.Lerp(maxAlpha, minAlpha, (Time.time - startTime) / lerpDuration);
                SetAlpha(alphaValue);
                yield return null;
            }
            yield return new WaitForSeconds(pauseDuration);
            currentlyRunningCoroutine = false;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = renderer.material.color;
        color.a = alpha;
        renderer.material.color = color;
    }
}
