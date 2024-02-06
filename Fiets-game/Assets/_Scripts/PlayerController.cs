using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float slideDuration = 1f;
    public float laneDistance = 4f; // Distance between lanes

    private int currentLane = 1; // 0 for left, 1 for middle, 2 for right
    private bool isSliding = false;

    void Start()
    {
        // Initialize the player in the middle lane
        Vector3 initialPosition = new Vector3(currentLane * laneDistance, transform.position.y, transform.position.z);
        transform.position = initialPosition;
    }

    void Update()
    {
        // Get input for lane switching
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) && currentLane > 0)
        {
            MoveLane(-1); // Move left
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) && currentLane < 2)
        {
            MoveLane(1); // Move right
        }

        // Check for jump input
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }

        // Check for slide input
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
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
        if (!isSliding)
        {
            // Apply a force to make the player jump
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Slide()
    {
        if (!isSliding)
        {
            // Start sliding by adjusting the player's scale and activating the isSliding flag
            StartCoroutine(SlideRoutine());
        }
    }

    IEnumerator SlideRoutine()
    {
        isSliding = true;

        // Adjust the player's scale to make it smaller (simulate sliding)
        transform.localScale = new Vector3(1f, 0.5f, 1f);

        // Wait for the slide duration
        yield return new WaitForSeconds(slideDuration);

        // Reset the player's scale and deactivate the isSliding flag
        transform.localScale = Vector3.one;
        isSliding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            SceneManager.LoadScene("Main");
        }
    }
}