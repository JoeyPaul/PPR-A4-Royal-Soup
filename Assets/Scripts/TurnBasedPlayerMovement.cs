using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedPlayerMovement : MonoBehaviour
{
    float moveDistance = 1;
    int xLean;
    int zLean;
    [SerializeField] Transform potTrans;
    [SerializeField] float leanMultiplier;
    [SerializeField] float maxLean;
    [SerializeField] GameObject spill;
    [SerializeField] int soupAmount;
    [SerializeField] Slider soupSlider;
    [SerializeField] LayerMask obstacleLayer;

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

    // Stabilises the pot on a single inputted axis by bringing its lean towards zero by 1
    private int MoveTowardsZero(int leanToChange)
    {
        if (leanToChange < 0) leanToChange++;
        if (leanToChange > 0) leanToChange--;
        return leanToChange;
    }

    // Checks if either axis's lean is on maxLean and if it is then spawn a spill and decrease amount of soup
    private void CheckForSpill()
    {
        if (xLean == maxLean || zLean == maxLean || xLean == -maxLean || zLean == -maxLean)
        {
            Instantiate(spill, transform.position, Quaternion.identity);
            soupAmount--;
        }
    }

    // This is what tips the pot when the player moves
    private int IncreaseLean(int currentLean, int difference)
    {
        if(difference > 0 && currentLean < maxLean || difference < 0 && currentLean > -maxLean)
            currentLean += difference;

        return currentLean;
    }
}
