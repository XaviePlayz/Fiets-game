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
    private bool movedLeft;
    private bool movedRight;
    private bool isLaneSwitchingCooldown;
    private float laneSwitchCooldownDuration = 0.15f; // Cooldown duration in seconds

    [Header("Jump")]
    public float jumpForce = 10f;
    public float maxJumpHeight = 2f;
    public float gravity = 9.8f; // Standard Earth gravity
    public float descentForce = 1f;
    public float jumpCooldown = 1f;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isGrounded = true;
    private bool canJump = true;

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
            // Check if the animation has reached a specific time
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
        if (!PauseMenu.Instance.isPaused)
        {
            // Apply a downward force if the player is above the max jump height
            if (transform.position.y > maxJumpHeight)
            {
                // Reduce the vertical velocity to make the descent smoother
                float descentVelocity = -descentForce * Time.deltaTime;
                GetComponent<Rigidbody>().velocity += Vector3.up * descentVelocity;
            }
            else
            {
                // If not above the max jump height, apply a lesser downward force
                float gravityVelocity = gravity * Time.deltaTime;
                GetComponent<Rigidbody>().velocity += Vector3.down * gravityVelocity;
            }

            // Get input for lane switching
            if ((Input.GetKeyDown(KeyCode.A) && !isLaneSwitchingCooldown || Input.GetKeyDown(KeyCode.LeftArrow) && !isLaneSwitchingCooldown))
            {
                MoveLane(-1); // Move left
            }
            else if ((Input.GetKeyDown(KeyCode.D) && !isLaneSwitchingCooldown || Input.GetKeyDown(KeyCode.RightArrow) && !isLaneSwitchingCooldown))
            {
                MoveLane(1); // Move right
            }

            // Joystick movement for lane switching
            float horizontalInput = Input.GetAxis("Horizontal");
            if (horizontalInput < -0.5f && !movedLeft && !isLaneSwitchingCooldown)
            {
                MoveLane(-1); // Move left
                movedLeft = true;
                movedRight = false;
            }
            else if (horizontalInput > 0.5f && !movedRight && !isLaneSwitchingCooldown)
            {
                MoveLane(1); // Move right
                movedRight = true;
                movedLeft = false;
            }

            // Reset movement flags when the joystick is in neutral position
            if (horizontalInput > -0.5f && horizontalInput < 0.5f)
            {
                movedLeft = false;
                movedRight = false;
            }

            // Check for jump input
            float verticalInput = Input.GetAxis("Vertical");
            if ((Input.GetKeyDown(KeyCode.W) && !isJumping && canJump || Input.GetKeyDown(KeyCode.UpArrow)) && !isJumping && canJump || Input.GetKeyDown(KeyCode.JoystickButton0) && !isJumping && canJump || verticalInput > 0.5f && !isJumping && canJump)
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

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.JoystickButton2) || verticalInput < -0.5f)
            {
                StartCoroutine(RemainSliding());
            }

            // Check for slide input
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.JoystickButton2) || verticalInput < -0.5f)
            {
                if (!isSliding)
                {
                    Slide();
                }
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

            // Start the lane switching cooldown
            StartCoroutine(LaneSwitchCooldown());
        }
    }

    IEnumerator LaneSwitchCooldown()
    {
        isLaneSwitchingCooldown = true;
        yield return new WaitForSeconds(laneSwitchCooldownDuration);
        isLaneSwitchingCooldown = false;
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
        if (!isJumping)
        {
            animator.SetTrigger("IsJumping");

            // Apply an impulse force to make the player jump
            Vector3 jumpVelocity = Vector3.up * CalculateJumpVelocity();
            GetComponent<Rigidbody>().velocity += jumpVelocity;

            isJumping = true;
            isGrounded = false;

            // Start the jump cooldown
            StartCoroutine(JumpCooldown());
        }
    }

    float CalculateJumpVelocity()
    {
        // Calculate the initial upward velocity for the jump
        // You can adjust the formula as needed for your game feel
        float initialVelocity = Mathf.Sqrt(2f * jumpForce * gravity);

        return initialVelocity;
    }
    IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    void CancelSlideJump()
    {
        // Cancel sliding immediately and jump with greater force
        animator.SetTrigger("IsJumping");
        StopCoroutine(SlideRoutine());
        isSliding = false;

        // Apply a force for canceled slide jump (same as regular jump force)
        GetComponent<Rigidbody>().AddForce(Vector3.up * CalculateJumpVelocity(), ForceMode.Impulse);

        isJumping = true;
        isGrounded = false;
    }

    void Slide()
    {
        if (isGrounded && !isSliding)
        {
            StartCoroutine(SlideRoutine());
        }
        else if (!isGrounded)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.down * descentForce * 7, ForceMode.Impulse);
            StartCoroutine(SlideRoutine());
        }
    }

    IEnumerator SlideRoutine()
    {
        if (!isSliding)
        {
            isSliding = true;
            isJumping = false;
            isGrounded = true;
            animator.SetTrigger("IsSliding");

            // Wait for the slide duration
            yield return new WaitForSeconds(0.7f);

            isSliding = false;
            animator.ResetTrigger("IsSliding");
        }
    }

    IEnumerator RemainSliding()
    {
        isSliding = true;
        animator.SetTrigger("StaySliding");
        yield return new WaitForSeconds(0.2f);
        isSliding = false;
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