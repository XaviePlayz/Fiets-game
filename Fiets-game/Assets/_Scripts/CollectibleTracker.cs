using UnityEngine;

public class CollectibleTracker : MonoBehaviour
{
    public float zIncreaseAmount = 1f;
    public float sphereCastRadius = 0.5f; // Adjust the radius as needed
    public int maxIterations = 10; // Maximum number of iterations in the loop

    void Update()
    {
        // Check if the collectible is above an obstacle and increase its z-value if needed
        CheckObstacleBelow();
    }

    void CheckObstacleBelow()
    {
        // Create a sphere cast from the collectible's position downward, considering only the z-axis
        Vector3 castOrigin = transform.position;
        castOrigin.y += sphereCastRadius; // Offset the cast origin slightly above the collectible

        Ray ray = new Ray(castOrigin, Vector3.down);
        RaycastHit hit;

        int iterations = 0;

        // Check if the sphere cast hits an object with the "Obstacle" tag
        if (iterations <= maxIterations)
        {
            while (Physics.SphereCast(ray, sphereCastRadius, out hit, 1f, LayerMask.GetMask("Obstacle")))
            {
                // Increase the z-value until the collectible is no longer above an obstacle
                transform.position += new Vector3(0f, 0f, zIncreaseAmount);

                iterations++;

                // Break the loop if the maximum number of iterations is reached
                if (iterations >= maxIterations)
                {
                    Debug.LogWarning("Max iterations reached in CheckObstacleBelow. Exiting loop.");
                    break;
                }
            }
        }     
    }
}