using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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
    public float scrollSpeed = 0.3f;
    public float laneDistance = 90f; // Distance between lanes
    public Transform playerTransform; // Reference to the player's transform
    public float patternSpawnRate = 10f; // Time between pattern spawns

    // New variables for seamless segment spawning
    public int segmentPoolSize = 2; // Number of segments to preload
    public float offscreenSpawnPositionZ = 50f; // Z position where segments are initially spawned off-camera
    public float spawnMoveDuration = 1f; // Duration for the segments to move into view

    private float nextSpawnTime;
    private float segmentLength;

    private int initialSegmentCount = 1; // Number of segments to spawn initially
    private Vector3 offscreenSpawnPosition;

    private List<GameObject> segmentPool;
    private int segmentSpawnedCount;

    public bool hasStarted;

    void Start()
    {
        segmentPool = new List<GameObject>();
        offscreenSpawnPosition = new Vector3(0f, 0f, offscreenSpawnPositionZ);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set nextSpawnTime to a value greater than the current time plus patternSpawnRate
        nextSpawnTime = Time.time + patternSpawnRate;
        segmentLength = laneDistance; // Adjust this based on your needs

        // Initialize segment pool and spawn initial segments
        InitializeSegmentPool();
        SpawnInitialSegments();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !hasStarted)
        {
            // Trigger the Start Running animation
            PlayerController.Instance.animator.SetBool("StartRunning", true);
            nextSpawnTime = Time.time + patternSpawnRate;  // Set the next spawn time
        }

        if (PlayerController.Instance.animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {
            hasStarted = true;
        }

        // Check if it's time to spawn a new obstacle pattern
        if (hasStarted)
        {
            MoveEnvironment();

            if (hasStarted && Time.time >= nextSpawnTime)
            {
                segmentSpawnedCount++;
                SpawnObstaclePattern();
                patternSpawnRate = 10.82f;
                nextSpawnTime = Time.time + patternSpawnRate;
            }
            //if (segmentSpawnedCount == 10)
            //{
            //    scrollSpeed = 0.35f;
            //}
        }
    }

    void MoveEnvironment()
    {
        // Calculate movement vector along the z-axis based on the player's position
        float targetZ = playerTransform.position.z + offscreenSpawnPositionZ;
        Vector3 movement = new Vector3(0f, 0f, -targetZ) * scrollSpeed * Time.deltaTime;

        // Move the environment
        transform.Translate(movement);

        // Move each child obstacle pattern individually
        foreach (Transform child in transform)
        {
            child.Translate(movement);
        }
    }

    void InitializeSegmentPool()
    {
        // Instantiate and preload a number of segments off-camera
        for (int i = 0; i < segmentPoolSize; i++)
        {
            GameObject segment = Instantiate(obstaclePatterns[Random.Range(0, obstaclePatterns.Length)], offscreenSpawnPosition, Quaternion.identity);
            segment.SetActive(false);
            segmentPool.Add(segment);
        }
    }

    void SpawnInitialSegments()
    {
        // Spawn the initial set of segments
        for (int i = 0; i < initialSegmentCount; i++)
        {
            SpawnSegment();
        }
    }

    void SpawnSegment()
    {
        // Get a segment from the pool
        GameObject segment = GetSegmentFromPool();

        // Set its position to the next adjusted offscreen spawn position (initially off-camera)
        float adjustedOffscreenSpawnPositionZ = offscreenSpawnPositionZ - (segmentSpawnedCount * segmentLength);
        segment.transform.position = new Vector3(0f, 0f, adjustedOffscreenSpawnPositionZ);

        // Set the parent of the segment to the EndlessRunner
        segment.transform.parent = transform;

        // Enable the segment
        segment.SetActive(true);

        // Gradually move the segment to its final on-camera position
        StartCoroutine(MoveSegment(segment, new Vector3(0f, 0f, adjustedOffscreenSpawnPositionZ - segmentLength), spawnMoveDuration));
    }


    GameObject GetSegmentFromPool()
    {
        // Get a disabled segment from the pool
        foreach (var segment in segmentPool)
        {
            if (!segment.activeSelf)
            {
                return segment;
            }
        }
        // If all segments are active, instantiate a new one and add it to the pool
        GameObject newSegment = Instantiate(obstaclePatterns[Random.Range(0, obstaclePatterns.Length)], offscreenSpawnPosition, Quaternion.identity);
        newSegment.SetActive(false);
        segmentPool.Add(newSegment);
        return newSegment;
    }

    IEnumerator MoveSegment(GameObject segment, Vector3 targetPosition, float duration)
    {
        float elapsed = 0f;
        Vector3 initialPosition = segment.transform.position;

        while (elapsed < duration)
        {
            // Move the segment gradually towards the target position
            segment.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the segment is exactly at the target position
        segment.transform.position = targetPosition;
    }

    void SpawnObstaclePattern()
    {
        // Spawn a new obstacle pattern (segment)
        SpawnSegment();
    }
}