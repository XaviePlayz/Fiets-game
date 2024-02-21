using UnityEngine;

public class CollectibleRotator : MonoBehaviour
{
    public float rotationSpeed = 75f;

    void Update()
    {
        // Rotate the object around the y-axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
