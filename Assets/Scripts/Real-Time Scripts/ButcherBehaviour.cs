using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    TurnBasedPlayerMovement player;

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

            if (Physics.Raycast(leftRayOrigin, -transform.up, out leftHit, 1f))
            {
                if (leftHit.collider.CompareTag("Player"))
                {
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

            if (Physics.Raycast(rightRayOrigin, -transform.up, out rightHit, 1f))
            {
                if (rightHit.collider.CompareTag("Player"))
                {
                    player.soupAmount -= 1;
                }
            }

            rightDangerWarning.dangerImminent = false;
        }

        attackInProgress = false;
        elapsedTime = 0f;
    }

    int PickRandomDirection()
    {
        int randomLeftRight = Random.Range(0, 1 + 1);
        return randomLeftRight;
    }
}
