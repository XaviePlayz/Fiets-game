using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Collectibles : MonoBehaviour
{
    public CollectibleSpawner collectibleSpawner;
    public AudioClip collectSound; // The sound to play when collected
    private AudioSource audioSource;

    public GameObject BikeProgress;
    public GameObject[] BikeParts;
    public GameObject[] Transparant;
    public TextMeshProUGUI bikePart;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If AudioSource is not already attached, add it dynamically
            audioSource = gameObject.AddComponent<AudioSource>();;
        }
        BikeProgress.SetActive(false);
    }

    void BikePartCollected()
    {
        // Play the collect sound
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        // Remove the collected collectible from this available spawning list
        collectibleSpawner.RemoveCollectedCollectible();

        // Increase the score with an additional 125
        ScoreManager.Instance.IncreaseScore(125);

        // Notify the spawner when the collectible is collected
        collectibleSpawner.OnCollectiblePassed();

        // Show the progress of your collected bike
        StartCoroutine(ShowBikeProgress());
    }

    IEnumerator ShowBikeProgress()
    {
        BikeProgress.SetActive(true);
        yield return new WaitForSeconds(4);
        BikeProgress.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Front_Wheel"))
        {
            Debug.Log($"{other.gameObject.tag} Collected!");
            // Show the Bike part you Collected in Text
            bikePart.text = "VOORWIEL";

                BikeParts[0].SetActive(true);
            Transparant[0].SetActive(false);
            BikePartCollected();

            Destroy(other.gameObject);
        }
        if (other.CompareTag("Pedal"))
        {
            Debug.Log($"{other.gameObject.tag} Collected!");
            bikePart.text = "PEDALEN";

            BikeParts[1].SetActive(true);
            Transparant[1].SetActive(false);
            BikePartCollected();

            Destroy(other.gameObject);
        }
        if (other.CompareTag("Back_Wheel"))
        {
            Debug.Log($"{other.gameObject.tag} Collected!");
            bikePart.text = "ACHTERWIEL";

            BikeParts[2].SetActive(true);
            Transparant[2].SetActive(false);
            BikePartCollected();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("HandleBar"))
        {
            Debug.Log($"{other.gameObject.tag} Collected!");
            bikePart.text = "STUUR";

            BikeParts[3].SetActive(true);
            Transparant[3].SetActive(false);
            BikePartCollected();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Frame"))
        {
            Debug.Log($"{other.gameObject.tag} Collected!");
            bikePart.text = "FRAME";

            BikeParts[4].SetActive(true);
            Transparant[4].SetActive(false);
            BikePartCollected();
            Destroy(other.gameObject);
        }
        if (other.CompareTag("MissedCollectible"))
        {
            // Get the CollectibleTracker component
            CollectibleTracker collectibleTracker = other.GetComponent<CollectibleTracker>();

            // Check if the component is not null before accessing its properties
            if (collectibleTracker != null && !collectibleTracker.isCollected)
            {
                collectibleSpawner.OnCollectiblePassed();
            }
        }
    }
}