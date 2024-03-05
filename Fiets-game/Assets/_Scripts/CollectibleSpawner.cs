using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public GameObject[] collectiblePrefabs; // Array of collectible prefabs
    [SerializeField] private List<int> availableCollectibles = new List<int>(); // List to track available collectibles

    public float initialZSpawnDistance = 10f; // Initial distance away from the player on the z-axis

    private int randomCollectibleIndex;
    private int totalNumberOfCollectibles = 0; // Variable to track the total number of collectibles spawned
    private GameObject lastSpawnedCollectible; // Reference to the last spawned collectible

    void Start()
    {
        // Initialize the list of available collectibles
        InitializeAvailableCollectibles();

        // Spawn the first collectible
        SpawnCollectible();
    }

    void InitializeAvailableCollectibles()
    {
        // Add indices of collectibles to the list
        for (int i = 0; i < collectiblePrefabs.Length; i++)
        {
            availableCollectibles.Add(i);
        }
    }

    void SpawnCollectible()
    {
        // Check if there are available collectibles
        if (availableCollectibles.Count == 0)
        {
            Debug.LogWarning("No more available collectibles.");
            return;
        }

        // Get a random index from the available collectibles
        int randomIndex = Random.Range(0, availableCollectibles.Count);

        randomCollectibleIndex = randomIndex;
        int collectibleIndex = availableCollectibles[randomIndex];       

        // Choose a random x-position from the available x-positions
        float[] xPositions = { -2f, 2f, 6f };
        float randomXPosition = xPositions[Random.Range(0, xPositions.Length)];

        // Calculate the z-position based on the initial distance and the total number of collectibles spawned
        float distanceBetweenCollectibles = 280f; // Adjust this value for your desired spacing
        float zPosition = initialZSpawnDistance + totalNumberOfCollectibles * distanceBetweenCollectibles;

        // Generate the spawn position
        Vector3 spawnPosition = new Vector3(randomXPosition, 2.6f, zPosition);


        // Find an existing GameObject in the hierarchy to use as the parent
        GameObject existingParent = GameObject.Find("Collectibles");

        if (existingParent != null)
        {
            // Instantiate the collectible as a child of CollectibleParent
            GameObject collectible = Instantiate(collectiblePrefabs[collectibleIndex], spawnPosition, Quaternion.identity, existingParent.transform);

            // Ensure the correct x-position is set
            collectible.transform.localPosition = new Vector3(randomXPosition, 2.6f, zPosition);

            // Increment the total number of collectibles spawned
            totalNumberOfCollectibles++;

            // Update the reference to the last spawned collectible
            lastSpawnedCollectible = collectible;
        }
        else
        {
            Debug.LogError("ExistingParent not found in the hierarchy.");
        }
    }

    // Remove the chosen collectible index from the available list if it got collected
    public void RemoveCollectedCollectible()
    {
        availableCollectibles.RemoveAt(randomCollectibleIndex);
    }

    // Called when the Player passes the collectible
    public void OnCollectiblePassed()
    {
        // Spawn the next collectible when the current one is collected
        SpawnCollectible();
    }
}