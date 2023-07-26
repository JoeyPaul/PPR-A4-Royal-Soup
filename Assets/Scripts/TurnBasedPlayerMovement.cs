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

    public bool arrivedAtKing;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            //print("Player collided with the destination collider!");
            arrivedAtKing = true;
        }
    }

    private void Start()
    {
        arrivedAtKing = false;
        soupSlider.maxValue = soupAmount;
    }
    private void Update()
    {
        if (!canMove)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            xLean = MoveTowardsZero(xLean);
            zLean = MoveTowardsZero(zLean);
            EnemyMovement.MoveEnemies();
        }
        
        soupSlider.value = soupAmount;
        potTrans.rotation = Quaternion.Euler(xLean * leanMultiplier, 0, zLean * leanMultiplier);
    }

    // I simplified the WASD movement into one function which is now called by the PlayerPointerBehaviour script whenever the player clicks on a pointer.
    public void MovePlayer(Vector3 moveDirection)
    {
        if (!Physics.Raycast(transform.position, moveDirection, 1, obstacleLayer) && canMove)
        {
            canMove = false;
            CheckForSpill();
            Vector3 startPos = transform.position;
            Vector3 nextPos = transform.position + moveDirection;
            StartCoroutine(LerpAnim(startPos, nextPos));
            xLean = IncreaseLean(xLean, -(int)moveDirection.z);
            zLean = IncreaseLean(zLean, (int)moveDirection.x);
            EnemyMovement.MoveEnemies();
        }
    }

    // Linearly Interpolates our transform between the start pos and the next pos over the course of animLength seconds.
    private IEnumerator LerpAnim(Vector3 startPos, Vector3 nextPos)
    {
        for (int i = 0; i < frames; i++)
        {
            transform.position = Vector3.Lerp(startPos, nextPos, i / (float)frames);
            yield return new WaitForSeconds(animLength / frames);
        }
        transform.position = nextPos;
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
