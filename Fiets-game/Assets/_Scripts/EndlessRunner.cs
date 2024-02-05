using System.Collections;
using UnityEngine;

public class EndlessRunner : MonoBehaviour
{
    public GameObject[] obstaclePatterns; // Array of obstacle patterns
    public float scrollSpeed = 5f;
    public float laneDistance = 2f; // Distance between lanes
    public float initialSpawnDelay = 2f; // Delay before the first pattern spawns
    public float patternSpawnRate = 5f; // Time between pattern spawns

    private float nextSpawnTime;
    private float segmentLength;

    void Start()
    {
        segmentLength = laneDistance; // Adjust this based on your needs
        nextSpawnTime = Time.time + initialSpawnDelay;

        // Initial spawn to start the game
        SpawnObstaclePattern();
        nextSpawnTime = Time.time + patternSpawnRate;
    }

    void Update()
    {
        MoveEnvironment();

        // Check if it's time to spawn a new obstacle pattern
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstaclePattern();
            nextSpawnTime = Time.time + patternSpawnRate;
        }
    }

    void MoveEnvironment()
    {
        // Calculate movement vector along the z-axis
        Vector3 movement = new Vector3(0f, 0f, -1f) * scrollSpeed * Time.deltaTime;

        // Move the environment
        transform.Translate(movement);

        // Move each child obstacle pattern individually
        foreach (Transform child in transform)
        {
            child.Translate(movement);
        }
    }

    void SpawnObstaclePattern()
    {
        // Randomly select an obstacle pattern from the array
        GameObject selectedPattern = obstaclePatterns[Random.Range(0, obstaclePatterns.Length)];

        // Instantiate the selected pattern as a child of the environment
        GameObject newPattern = Instantiate(selectedPattern, transform.position, Quaternion.identity, transform);

        // Calculate the position for the new pattern
        float newPositionZ = newPattern.transform.position.z + segmentLength;

        // Set the position of the new pattern
        newPattern.transform.position = new Vector3(0f, 0f, newPositionZ);
    }
}