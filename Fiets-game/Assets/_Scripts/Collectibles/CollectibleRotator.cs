using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CollectibleRotator : MonoBehaviour
{
    public float rotationSpeed = 75f;
    public float transformX;
    public bool freezePostition;

    private void Start()
    {
        transformX = transform.position.x;
    }
    void Update()
    {
        // Rotate the object around the y-axis
        if (freezePostition)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            transform.position = new Vector3(transformX, transform.position.y, transform.position.z);
        }
        else
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
