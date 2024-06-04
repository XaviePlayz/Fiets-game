using System.Collections;
using UnityEngine;

public class CollectibleTracker : MonoBehaviour
{
    public float zIncreaseAmount = 1f;
    public float sphereCastRadius = 0.5f; // Adjust the radius as needed
    public int maxIterations = 10; // Maximum number of iterations in the loop
    public bool isCollected;

    // Delay before starting to check for obstacles (in seconds)
    public float startDelay = 15f;

    void Start()
    {
        // Start the coroutine for the delay
        StartCoroutine(StartCheckingForObstacles());
    }

    IEnumerator StartCheckingForObstacles()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(startDelay);

        // Start checking for obstacles after the delay
        StartCoroutine(CheckObstacleBelow());
    }

    IEnumerator CheckObstacleBelow()
    {
        // Create a sphere cast from the collectible's position downward, considering only the z-axis
        Vector3 castOrigin = transform.position;
        castOrigin.y += sphereCastRadius; // Offset the cast origin slightly above the collectible

        int iterations = 0;

        // Continue checking for obstacles
        while (Physics.CheckSphere(castOrigin, sphereCastRadius, LayerMask.GetMask("Obstacle")))
        {
            // Increase the z-value until the collectible is no longer near an obstacle
            transform.position += new Vector3(0f, 0f, zIncreaseAmount);

            iterations++;

            // Break the loop if the maximum number of iterations is reached
            if (iterations >= maxIterations)
            {
                Debug.LogWarning("Max iterations reached in CheckObstacleBelow. Exiting loop.");
                break;
            }

            // Wait for a short interval before the next check
            yield return null;
        }
    }
}