using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public float smoothness = 5f; // Adjust this value to control the smoothness

    void Update()
    {
        if (playerTransform != null)
        {
            // Calculate the target position
            Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);

            // Use Vector3.Lerp to smoothly interpolate between the current position and the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);
        }
    }
}