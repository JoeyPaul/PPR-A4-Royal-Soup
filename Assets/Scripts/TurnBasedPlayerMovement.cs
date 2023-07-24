using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedPlayerMovement : MonoBehaviour
{
    public float moveDistance;
    private int xLean;
    private int zLean;
    public Transform potTrans;
    public float leanMultiplier;
    public float maxLean;
    public GameObject spill;
    public int soupAmount;
    public Slider soupSlider;
    public LayerMask obstacleLayer;

    private void Start()
    {
        //soupSlider.maxValue = soupAmount;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!Physics.Raycast(transform.position, transform.right, 1, obstacleLayer))
            {
                xLean = MoveTowardsZero(xLean);
                CheckForSpill();
                transform.position += new Vector3(moveDistance, 0, 0);
                zLean = IncreaseLean(zLean, 1);
                EnemyMovement.MoveEnemies();
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!Physics.Raycast(transform.position, -transform.right, 1, obstacleLayer))
            {
                xLean = MoveTowardsZero(xLean);
                CheckForSpill();
                transform.position += new Vector3(-moveDistance, 0, 0);
                zLean = IncreaseLean(zLean, -1);
                EnemyMovement.MoveEnemies();
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!Physics.Raycast(transform.position, -transform.forward, 1, obstacleLayer))
            {
                zLean = MoveTowardsZero(zLean);
                CheckForSpill();
                transform.position += new Vector3(0, 0, -moveDistance);
                xLean = IncreaseLean(xLean, 1);
                EnemyMovement.MoveEnemies();
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!Physics.Raycast(transform.position, transform.forward, 1, obstacleLayer))
            {
                zLean = MoveTowardsZero(zLean);
                CheckForSpill();
                transform.position += new Vector3(0, 0, moveDistance);
                xLean = IncreaseLean(xLean, -1);
                EnemyMovement.MoveEnemies();
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            xLean = MoveTowardsZero(xLean);
            zLean = MoveTowardsZero(zLean);
            EnemyMovement.MoveEnemies();
        }

        //soupSlider.value = soupAmount;
        potTrans.rotation = Quaternion.Euler(xLean * leanMultiplier, 0, zLean * leanMultiplier);
    }
    private int MoveTowardsZero(int leanToChange)
    {
        if (leanToChange < 0) leanToChange++;
        if (leanToChange > 0) leanToChange--;
        return leanToChange;
    }
    private void CheckForSpill()
    {
        if (xLean == 3 || zLean == 3 || xLean == -3 || zLean == -3)
        {
            Instantiate(spill, transform.position, Quaternion.identity);
            soupAmount--;
        }
    }
    private int IncreaseLean(int currentLean, int difference)
    {
        if(difference > 0 && currentLean < maxLean || difference < 0 && currentLean > -maxLean)
            currentLean += difference;

        return currentLean;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.right * 2);
    }
}
