using UnityEngine;

public class EnvironmentMovement : MonoBehaviour
{
    public float scrollSpeed = 5f; // Adjust this speed as needed

    void Update()
    {
        // Move the environment towards the player
        MoveEnvironment();
    }

    void MoveEnvironment()
    {
        // Calculate movement vector
        Vector3 movement = new Vector3(-1f, 0f, 0f) * scrollSpeed * Time.deltaTime;

        // Move the environment
        transform.Translate(movement);

        // Optionally, reset the environment position to create the endless effect
        if (transform.position.x < -10f) // Adjust the reset position as needed
        {
            ResetEnvironmentPosition();
        }
    }

    void ResetEnvironmentPosition()
    {
        // Reset the environment position to create an endless effect
        transform.position = new Vector3(0f, 0f, 0f);
    }
}