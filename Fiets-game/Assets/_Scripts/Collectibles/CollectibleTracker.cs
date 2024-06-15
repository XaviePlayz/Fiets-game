using System.Collections;
using UnityEngine;

public class CollectibleTracker : MonoBehaviour
{
    public float zIncreaseAmount = 1f;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            StartCoroutine(CheckObstacleBelow());
        }
    }

    IEnumerator CheckObstacleBelow()
    {
        yield return new WaitForSeconds(3);
        // Increase the z-value until the collectible is no longer near an obstacle
        transform.position += new Vector3(0f, 0f, zIncreaseAmount);

        // Wait for a short interval before the next check
        yield return null;
    }
}