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
    private int currentLane = 1; // 0 for left, 1 for middle, 2 for right

    [Header("Jump")]
    public float jumpForce = 10f;
    public float maxJumpHeight = 2f;
    public float descentForce = 2f;
    private bool isJumping = false;
    private bool isGrounded = true;

    [Header("Slide")]
    public float slideDuration = 1f;
    private bool isSliding = false;



    public Animator animator;

    void Start()
    {
        // Initialize the player in the middle lane
        Vector3 initialPosition = new Vector3(currentLane * laneDistance, transform.position.y, transform.position.z);
        transform.position = initialPosition;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (EndlessRunner.Instance.hasStarted)
        {
            HandleInput();

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Start Running"))
            {
                // Check if the animation has reached a specific time (adjust as needed)
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
                {
                    // Trigger the Running animation
                    animator.SetBool("IsRunning", true);
                }
            }
        }
    }

    void HandleInput()
    {
        // Apply a downward force if the player is above the max jump height
        if (transform.position.y > maxJumpHeight)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.down * descentForce, ForceMode.Acceleration);
        }

        // Get input for lane switching
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && currentLane > 0)
        {
            MoveLane(-1); // Move left
        }
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && currentLane < 2)
        {
            MoveLane(1); // Move right
        }

        // Check for jump input
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) && !isJumping)
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
        // Move the player to the adjacent lane
        currentLane += direction;
        currentLane = Mathf.Clamp(currentLane, 0, 2); // Ensure the player stays within valid lanes
        Vector3 targetPosition = new Vector3(currentLane * laneDistance, transform.position.y, transform.position.z);
        transform.position = targetPosition;
    }

    void Jump()
    {
        if (isGrounded) // Check if currently grounded
        {
            animator.SetTrigger("IsJumping");
            // Apply a force to make the player jump
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
            //
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