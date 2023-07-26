using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
    [SerializeField] [Range(0, 20)] int soupSpillAmount;
    [SerializeField] Slider soupSlider;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] int frames;
    [SerializeField] float animLength;
    bool canMove = true;
    private void Start()
    {
        soupSlider.maxValue = soupAmount;
    }
    private void Update()
    {
        if (!canMove)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!Physics.Raycast(transform.position, transform.right, 1, obstacleLayer))
            {
                //xLean = MoveTowardsZero(xLean);
                CheckForSpill();
                Vector3 startPos = transform.position;
                Vector3 nextPos = new Vector3(transform.position.x + moveDistance, transform.position.y, transform.position.z);
                StartCoroutine(LerpAnim(startPos, nextPos));
                zLean = IncreaseLean(zLean, 1);
                EnemyMovement.MoveEnemies();
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!Physics.Raycast(transform.position, -transform.right, 1, obstacleLayer))
            {
                //xLean = MoveTowardsZero(xLean);
                CheckForSpill();
                Vector3 startPos = transform.position;
                Vector3 nextPos = new Vector3(transform.position.x - moveDistance, transform.position.y, transform.position.z);
                StartCoroutine(LerpAnim(startPos, nextPos));
                zLean = IncreaseLean(zLean, -1);
                EnemyMovement.MoveEnemies();
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!Physics.Raycast(transform.position, -transform.forward, 1, obstacleLayer))
            {
                //zLean = MoveTowardsZero(zLean);
                CheckForSpill();
                Vector3 startPos = transform.position;
                Vector3 nextPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - moveDistance);
                StartCoroutine(LerpAnim(startPos, nextPos));
                xLean = IncreaseLean(xLean, 1);
                EnemyMovement.MoveEnemies();
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!Physics.Raycast(transform.position, transform.forward, 1, obstacleLayer))
            {
                //zLean = MoveTowardsZero(zLean);
                CheckForSpill();
                Vector3 startPos = transform.position;
                Vector3 nextPos = new Vector3 (transform.position.x, transform.position.y, transform.position.z + moveDistance);
                StartCoroutine(LerpAnim(startPos, nextPos));
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
        
        soupSlider.value = soupAmount;
        potTrans.rotation = Quaternion.Euler(xLean * leanMultiplier, 0, zLean * leanMultiplier);
    }
    private IEnumerator LerpAnim(Vector3 startPos, Vector3 nextPos)
    {
        StartCoroutine(MoveCooldown());
        for (int i = 0; i < frames; i++)
        {
            //print(Vector3.Lerp(startPos, nextPos, i / frames));
            transform.position = Vector3.Lerp(startPos, nextPos, i / (float)frames);
            yield return new WaitForSeconds(animLength / frames);
        }
        transform.position = nextPos;
    }
    private IEnumerator MoveCooldown()
    {
        canMove = false;
        yield return new WaitForSeconds(animLength);
        canMove = true;
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
            soupAmount -= soupSpillAmount;
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
