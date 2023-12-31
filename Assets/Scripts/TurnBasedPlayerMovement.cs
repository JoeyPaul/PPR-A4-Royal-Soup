using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public int soupAmount;
    [SerializeField][Range(0, 20)] int soupSpillAmount;
    [SerializeField] Slider soupSlider;

    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] int frames;
    public float animLength;

    [SerializeField] Transform currentSpriteTrans;
    [SerializeField] Transform cameraTransform;
    [SerializeField] Sprite[] directionalSprites;
    [SerializeField] Transform spriteDirectionTrans;
    [SerializeField] SpriteRenderer spriteRenderer;
    [HideInInspector] public Vector3 prevDir;

    GameController game;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool arrivedAtKing;

    [SerializeField] private GameObject retryScreen;

    [SerializeField] float invincibilityDuration = 1f;
    private bool currentlyInvincible = false;
    private float timeInvincible = 0.0f;

    private bool hasInputted;
    private Vector3 queuedInput;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destination"))
        {
            //print("Player collided with the destination collider!");
            //arrivedAtKing = true;
            SceneManagerScript.ChangeScene("ResultsSceneWin");
        }
        if(other.CompareTag("Enemy"))
        {
            if (!currentlyInvincible)
            {
                Vector3 direction = (transform.position - other.transform.position).normalized;
                if (direction.Abs().x > direction.Abs().z)
                {
                    direction.z = 0;
                }
                else
                {
                    direction.x = 0;
                }
                //print(direction);
                StartCoroutine(MovePlayer(direction.normalized, false));
                soupAmount -= 1;
                Hit();
                currentlyInvincible = true;
            }
        }
    }

    public void Hit()
    {
        Instantiate(spill, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.Euler(-90, 0, 0));
    }

    private void Start()
    {
        //game = FindObjectOfType<GameController>();
        arrivedAtKing = false;
        soupSlider.maxValue = soupAmount;
    }
    private void Update()
    {
        // Make the sprite face towards the camera
        SpriteFaceToCamera();

        if (soupAmount <= 0)
            SceneManagerScript.ChangeScene("ResultsScene");

        //if (!canMove)
        //    return;

        // On trigger (got hit by enemy) invincibility timer and reset condition.
        if (currentlyInvincible)
            timeInvincible += Time.deltaTime;
        if (timeInvincible > invincibilityDuration)
        {
            currentlyInvincible = false;
            timeInvincible = 0.0f;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            xLean = MoveTowardsZero(xLean);
            zLean = MoveTowardsZero(zLean);
            EnemyMovement.MoveEnemies();
            //game.currentTurn += 1;
            //print(game.currentTurn);
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GetComponent<Rigidbody>().AddForce(-1, 0, 0);
        }

        // WASD Input 
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(MovePlayer(transform.forward, true));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(MovePlayer(-transform.forward, true));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(MovePlayer(-transform.right, true));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(MovePlayer(transform.right, true));
        }

        soupSlider.value = soupAmount;
        //potTrans.rotation = Quaternion.Euler(xLean * leanMultiplier, 0, zLean * leanMultiplier);
    }

    private void SpriteFaceToCamera()
    {
        // Make the sprite face towards the camera
        Vector3 directionToCamera = cameraTransform.position - currentSpriteTrans.position;
        directionToCamera.y = 0f;
        if (directionToCamera != Vector3.zero)
        {
            // Face towards the target on the Y-axis
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            currentSpriteTrans.rotation = targetRotation;
        }

        // If the sprite rotation is between the angles 330 and 0 (facing forward);
        if (currentSpriteTrans.localRotation.eulerAngles.y > 315 || currentSpriteTrans.localRotation.eulerAngles.y < 45)
        {
            spriteRenderer.sprite = directionalSprites[0];
        }
        else if (currentSpriteTrans.localRotation.eulerAngles.y > 45 && currentSpriteTrans.localRotation.eulerAngles.y < 135)
        {
            spriteRenderer.sprite = directionalSprites[1];
        }
        else if (currentSpriteTrans.localRotation.eulerAngles.y > 135 && currentSpriteTrans.localRotation.eulerAngles.y < 225)
        {
            spriteRenderer.sprite = directionalSprites[2];
        }
        else if (currentSpriteTrans.localRotation.eulerAngles.y > 225 && currentSpriteTrans.localRotation.eulerAngles.y < 315)
        {
            spriteRenderer.sprite = directionalSprites[3];
        }
    }

    // I simplified the WASD movement into one function which is now called by the PlayerPointerBehaviour script whenever the player clicks on a pointer.
    public IEnumerator MovePlayer(Vector3 moveDirection, bool completeTurn)
    {
        if (hasInputted && queuedInput == Vector3.zero)
        {
            print("Queued");
            queuedInput = moveDirection;
            yield break;
        }

        
        if (!Physics.Raycast(transform.position, moveDirection, 1, obstacleLayer) && canMove)
        {
            hasInputted = true;
            canMove = false;
            prevDir = moveDirection;
            spriteDirectionTrans.localRotation = Quaternion.LookRotation(moveDirection);
            //CheckForSpill();
            Vector3 startPos = transform.position;
            Vector3 nextPos = transform.position + moveDirection;
            //xLean = IncreaseLean(xLean, -(int)moveDirection.z);
            //zLean = IncreaseLean(zLean, (int)moveDirection.x);
            for (int i = 0; i < frames; i++)
            {
                transform.position = Vector3.Lerp(startPos, nextPos, i / (float)frames);
                yield return new WaitForSeconds(animLength / frames);
            }
            transform.position = nextPos;
            //if(completeTurn)
            //{
            //    EnemyMovement.MoveEnemies();
            //    //game.currentTurn += 1;
            //}
            canMove = true;
            hasInputted = false;
            if (queuedInput != Vector3.zero)
            {
                StartCoroutine(MovePlayer(queuedInput, true));
                queuedInput = Vector3.zero;
            }
        }
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
            SpillSoup();
        }
    }

    // This is what tips the pot when the player moves
    private int IncreaseLean(int currentLean, int difference)
    {
        if (difference > 0 && currentLean < maxLean || difference < 0 && currentLean > -maxLean)
            currentLean += difference;

        return currentLean;
    }

    public void SpillSoup()
    {
        Instantiate(spill, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.Euler(-90, 0, 0));
        soupAmount -= soupSpillAmount;
    }
}
