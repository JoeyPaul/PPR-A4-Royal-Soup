using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RealTimeMovement : MonoBehaviour
{
    float moveDistance = 1;
    [SerializeField] Transform potTrans;
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

    GameController game;

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
        game = FindObjectOfType<GameController>();
        arrivedAtKing = false;
        soupSlider.maxValue = soupAmount;
    }
    private void Update()
    {
        // Make the sprite face towards the camera
        SpriteFaceToCamera();

        if (!canMove)
            return;

        // WASD Input 
        if (Input.GetKeyDown(KeyCode.W))
        {
            MovePlayer(transform.forward);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MovePlayer(-transform.forward);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MovePlayer(-transform.right);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MovePlayer(transform.right);
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
        //print(currentSpriteTrans.localRotation.eulerAngles);

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
    public void MovePlayer(Vector3 moveDirection)
    {
        if (!Physics.Raycast(transform.position, moveDirection, 1, obstacleLayer) && canMove)
        {
            canMove = false;
            spriteDirectionTrans.localRotation = Quaternion.LookRotation(moveDirection);
            Vector3 startPos = transform.position;
            Vector3 nextPos = transform.position + moveDirection;
            StartCoroutine(LerpAnim(startPos, nextPos));
        }
    }

    // Almost the same as the MovePlayer function, although it checks for a collision free direction to move in. 
    // It will then move them in the respective direction.
    public void MovePlayerOnly(Vector3 enemyMoveDirection)
    {
        Vector3 moveDirection = GetCollisionFreeDirection(enemyMoveDirection);
        if (!Physics.Raycast(transform.position, moveDirection, 1, obstacleLayer) && canMove)
        {
            canMove = false;
            Vector3 startPos = transform.position;
            Vector3 nextPos = transform.position + moveDirection;
            StartCoroutine(LerpAnim(startPos, nextPos));
        }
    }

    private Vector3 GetCollisionFreeDirection(Vector3 moveDirection)
    {
        // Check 4 cardinal directions for collision objects
        if (!Physics.Raycast(transform.position, transform.right, 1, obstacleLayer))
        {
            return transform.right;
        }
        else if (!Physics.Raycast(transform.position, transform.forward, 1, obstacleLayer))
        {
            return transform.forward;
        }
        else if (!Physics.Raycast(transform.position, -transform.right, 1, obstacleLayer))
        {
            return -transform.right;
        }
        else if (!Physics.Raycast(transform.position, -transform.right, 1, obstacleLayer))
        {
            return -transform.forward;
        }
        return moveDirection;
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
}
