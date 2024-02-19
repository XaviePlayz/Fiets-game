using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Singleton

    private static PlayerController _instance;
    public static PlayerController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerController>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PlayerController).Name;
                    _instance = obj.AddComponent<PlayerController>();
                }
            }
            return _instance;
        }
    }

    #endregion

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float laneDistance = 4f;
    [SerializeField] private int currentLane = 1; // 0 for left, 1 for middle, 2 for right

    [Header("Jump")]
    public float jumpForce = 10f;
    public float maxJumpHeight = 2f;
    public float descentForce = 2f;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isGrounded = true;

    [Header("Slide")]
    public float slideDuration = 1f;
    [SerializeField] private bool isSliding = false;

    public Animator animator;

    void Start()
    {
        // Adjust the target x-position based on the desired lane positions
        float targetX = currentLane * laneDistance;

        // Set the new position
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = targetPosition;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Start Running"))
        {
            // Check if the animation has reached a specific time (adjust as needed)
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                // Trigger the Running animation
                animator.SetBool("IsRunning", true);
            }
        }

        if (EndlessRunner.Instance.hasStarted)
        {
            HandleInput();
        }
    }

    void HandleInput()
    {
        // Apply a downward force if the player is above the max jump height
        if (transform.position.y > maxJumpHeight)
        {
            // Reduce the vertical velocity to make the descent smoother
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, -descentForce, GetComponent<Rigidbody>().velocity.z);
        }
        else
        {
            // If not above the max jump height, apply a lesser downward force
            GetComponent<Rigidbody>().AddForce(Vector3.down * descentForce, ForceMode.Acceleration);
        }

        // Get input for lane switching
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            MoveLane(-1); // Move left
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            MoveLane(1); // Move right
        }

        // Check for jump input
        if (Input.GetKeyDown(KeyCode.W) && !isJumping || Input.GetKeyDown(KeyCode.UpArrow) && !isJumping)
        {
            if (!isSliding)
            {
                Jump();
            }
            else
            {
                // Cancel sliding immediately and jump with greater force
                CancelSlideJump();
            }
        }

        // Check for slide input
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!isSliding)
            {
                Slide();
            }
        }
    }

    void MoveLane(int direction)
    {
        int targetLane = currentLane + direction;

        // Check for obstacles in the target lane before moving
        if (!CheckObstacleInLane(targetLane, direction))
        {
            // Move the player to the adjacent lane if no obstacle is detected
            currentLane = targetLane;
            currentLane = Mathf.Clamp(currentLane, 0, 2); // Ensure the player stays within valid lanes

            // Calculate the target x-position based on the desired lane positions
            float targetX = currentLane * laneDistance;

            // Start a coroutine to smoothly move to the new position
            StartCoroutine(MoveToLane(targetX));

            // Stop obstacle animation routines if running
            StopCoroutine(ObstacleAnimationRoutineLeft());
            StopCoroutine(ObstacleAnimationRoutineRight());
        }
    }


    bool CheckObstacleInLane(int targetLane, int direction)
    {
        Debug.Log("Checking for obstacles in lane: " + targetLane);

        // Set the check position just above the player to avoid self-collision
        Vector3 checkPosition = new Vector3(targetLane * laneDistance, transform.position.y + 0.1f, transform.position.z);

        // Check for obstacles at the specified position
        Collider[] colliders = Physics.OverlapSphere(checkPosition, 0.75f, LayerMask.GetMask("Obstacle"));

        // If there are colliders in the "Obstacle" layer, an obstacle is detected
        if (colliders.Length > 0)
        {
            Debug.Log("Obstacle detected in lane!");
            if (direction < 0)
            {
                StartCoroutine(ObstacleAnimationRoutineLeft());
            }
            else
            {
                StartCoroutine(ObstacleAnimationRoutineRight());
            }
            return true;
        }

        // No obstacle detected
        return false;
    }

      IEnumerator ObstacleAnimationRoutineLeft()
    {
        // Play the obstacle animation
        animator.SetTrigger("ObstacleLeftEncountered");
        Vector3 targetPosition = new Vector3(currentLane * (laneDistance - 1.5f), transform.position.y, transform.position.z);
        transform.position = targetPosition;
        currentLane--;

        // Wait for the animation duration
        yield return new WaitForSeconds(1f);

        // Reset the animation state
        animator.ResetTrigger("ObstacleLeftEncountered");
    }
    IEnumerator ObstacleAnimationRoutineRight()
    {
        // Play the obstacle animation
        animator.SetTrigger("ObstacleRightEncountered");
        Vector3 targetPosition = new Vector3(currentLane * (laneDistance + 1.5f), transform.position.y, transform.position.z);
        transform.position = targetPosition;
        currentLane++;

        // Wait for the animation duration
        yield return new WaitForSeconds(1f);

        // Reset the animation state
        animator.ResetTrigger("ObstacleRightEncountered");
    }

    IEnumerator MoveToLane(float targetX)
    {
        float elapsedTime = 0f;
        float duration = 0.2f; // Adjust the duration as needed
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        while (elapsedTime < duration)
        {
            // Move gradually towards the target position
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the player is exactly at the target position
        transform.position = targetPosition;
    }


    void Jump()
    {
        if (isGrounded) // Check if currently grounded
        {
            animator.SetTrigger("IsJumping");

            // Apply an impulse force to make the player jump
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0f, GetComponent<Rigidbody>().velocity.z);
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Set the jumping flag to true
            isJumping = true;

            // Set the grounded flag to false
            isGrounded = false;
        }
    }

    void CancelSlideJump()
    {
        // Cancel sliding immediately and jump with greater force
        animator.SetTrigger("IsJumping");
        StopCoroutine(SlideRoutine());
        isSliding = false;

        // Apply a force for canceled slide jump
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Set the jumping flag to true
        isJumping = true;

        // Set the grounded flag to false
        isGrounded = false;
    }

    void Slide()
    {
        if (isGrounded)
        {
            // Start sliding by adjusting the player's scale and activating the isSliding flag
            StartCoroutine(SlideRoutine());
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(Vector3.down * descentForce * 7, ForceMode.Impulse);
            StartCoroutine(SlideRoutine());
        }
    }

    IEnumerator SlideRoutine()
    {
        isSliding = true;
        animator.SetTrigger("IsSliding");

        // Wait for the slide duration
        yield return new WaitForSeconds(slideDuration);

        // Reset the player's scale and deactivate the isSliding flag
        isSliding = false;

        // Set the jumping flag to false after sliding
        isJumping = false;

        // Set the grounded flag to true after sliding
        isGrounded = true;
    }

    // Detect when the player touches an obstacle
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            SceneManager.LoadScene("Main");
        }
    }

    // Detect when the player touches the ground
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
        }
    }

    // Detect when the player leaves the ground
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}