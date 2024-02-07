using System.Collections;
using UnityEngine;

public class EndlessRunner : MonoBehaviour
{
    #region Singleton

    private static EndlessRunner _instance;
    public static EndlessRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EndlessRunner>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(EndlessRunner).Name;
                    _instance = obj.AddComponent<EndlessRunner>();
                }
            }
            return _instance;
        }
    }

    #endregion

    public GameObject[] obstaclePatterns; // Array of obstacle patterns
    public float scrollSpeed = 5f;
    public float laneDistance = 2f; // Distance between lanes
    public Transform spawnPatternLocation; 
    public float patternSpawnRate = 5f; // Time between pattern spawns

    private float nextSpawnTime;
    private float segmentLength;

    public bool hasStarted;

    void Start()
    {
        SpawnObstaclePattern();

        // Set nextSpawnTime to a value greater than the current time plus patternSpawnRate
        nextSpawnTime = Time.time + patternSpawnRate;
        segmentLength = laneDistance; // Adjust this based on your needs
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !hasStarted)
        {
            // Trigger the Start Running animation
            PlayerController.Instance.animator.SetBool("StartRunning", true);

            // Reset the rotation to 0 at the start of Start Running animation
            hasStarted = true;
        }
        if (hasStarted)
        {
            MoveEnvironment();
        }

        // Check if it's time to spawn a new obstacle pattern
        if (hasStarted)
        {
            if (Time.time >= nextSpawnTime)
            {
                SpawnObstaclePattern();
                nextSpawnTime = Time.time + patternSpawnRate;
            }
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
        GameObject newPattern = Instantiate(selectedPattern, spawnPatternLocation.position, Quaternion.identity, transform);

        // Calculate the position for the new pattern
        float newPositionZ = newPattern.transform.position.z + segmentLength;

        // Set the position of the new pattern
        newPattern.transform.position = new Vector3(0f, 0f, newPositionZ);
    }
}