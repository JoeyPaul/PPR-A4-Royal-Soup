using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButcherBehaviour : MonoBehaviour
{
    WarningDangerMarker leftDangerWarning;
    WarningDangerMarker rightDangerWarning;

    private float elapsedTime = -1f;
    public float attackCountdownDuration = 5f;
    public float attackAnimationLength = 1f;

    bool attackInProgress = false;
    bool randomDirectionChosen = false;
    int randomDirection;
    public float swingAttackLerpDuration;

    [SerializeField]
    TurnBasedPlayerMovement player;

    [SerializeField]
    GameObject attackParticle;
    [SerializeField] Transform artPosition;

    private void Start()
    {
        Transform dangerLeft = transform.Find("Danger Left");
        Transform dangerRight = transform.Find("Danger Right");
        leftDangerWarning = dangerLeft.GetComponent<WarningDangerMarker>();
        rightDangerWarning = dangerRight.GetComponent<WarningDangerMarker>();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (!randomDirectionChosen)
            randomDirection = PickRandomDirection();
        
        if (elapsedTime >= attackCountdownDuration - 2f)
        {
            if (randomDirection == 0 && !attackInProgress && !rightDangerWarning.dangerImminent)
                leftDangerWarning.dangerImminent = true;
            else if (randomDirection == 1 && !attackInProgress && !leftDangerWarning.dangerImminent)
                rightDangerWarning.dangerImminent = true;
        }

        if (elapsedTime > attackCountdownDuration)
        {
            // Attack left
            if (randomDirection == 0 && !rightDangerWarning.dangerImminent)
            {
                if (!attackInProgress)
                    StartCoroutine(Attack(0));
            }
            else // Attack Right
            {
                if (!attackInProgress && !leftDangerWarning.dangerImminent)
                StartCoroutine(Attack(1));
            }
        }
    }

    IEnumerator Attack(int direction)
    {
        attackInProgress = true;

        if (direction == 0)
        {
            // Start attack anim
            yield return new WaitForSeconds(attackAnimationLength);

            // Perform raycast in the left direction
            Vector3 leftRayOrigin = leftDangerWarning.transform.position + new Vector3(0, 1, 0);
            RaycastHit leftHit;
            StartCoroutine(SwingAttackLerp(0));
            Instantiate(attackParticle, leftRayOrigin + new Vector3(0,-1.3f,0), Quaternion.identity);
            if (Physics.Raycast(leftRayOrigin, -transform.up, out leftHit, 1f))
            {
                if (leftHit.collider.CompareTag("Player"))
                {
                    player.Hit();
                    player.soupAmount -= 1;
                }
            }

            leftDangerWarning.dangerImminent = false;
        }
        else
        {
            // Start attack anim
            yield return new WaitForSeconds(attackAnimationLength);

            // Perform raycast in the right direction
            Vector3 rightRayOrigin = rightDangerWarning.transform.position + new Vector3(0,1,0);
            RaycastHit rightHit;
            StartCoroutine(SwingAttackLerp(1));
            Instantiate(attackParticle, rightRayOrigin + new Vector3(0, -1.3f, 0), Quaternion.identity);
            if (Physics.Raycast(rightRayOrigin, -transform.up, out rightHit, 1f))
            {
                if (rightHit.collider.CompareTag("Player"))
                {
                    player.Hit();
                    player.soupAmount -= 1;
                }
            }

            rightDangerWarning.dangerImminent = false;
        }

        attackInProgress = false;
        elapsedTime = 0f;
    }

    private IEnumerator SwingAttackLerp(int direction)
    {
        Vector3 originalPosition = artPosition.position;
        Vector3 targetPosition = Vector3.zero;
        // Calculate the target position against the direction input 
        if (direction == 0 )
        {
            targetPosition = artPosition.position + Vector3.left * 0.5f;
        }
        else
        {
            targetPosition = artPosition.position + Vector3.right * 0.5f;
        }
        // Lerp towards the destination then back to the original position
        float startTime = Time.time;
        while (Time.time - startTime < swingAttackLerpDuration)
        {
            float t = (Time.time - startTime) / swingAttackLerpDuration;
            artPosition.position = Vector3.Lerp(originalPosition, targetPosition, t);
            yield return null;
        }
        startTime = Time.time;
        while (Time.time - startTime < swingAttackLerpDuration)
        {
            float t = (Time.time - startTime) / swingAttackLerpDuration;
            artPosition.position = Vector3.Lerp(targetPosition, originalPosition, t);
            yield return null;
        }
        // Ensure the object is back at the original position
        artPosition.position = originalPosition;
    }

    int PickRandomDirection()
    {
        int randomLeftRight = Random.Range(0, 1 + 1);
        return randomLeftRight;
    }
}
