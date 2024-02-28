using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectibles : MonoBehaviour
{
    public CollectibleSpawner collectibleSpawner;
    public AudioClip collectSound; // The sound to play when collected
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If AudioSource is not already attached, add it dynamically
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Front_Wheel"))
        {
            Debug.Log("FRONT WHEEL Collected!");

            // Notify the spawner when the collectible is collected
            collectibleSpawner.OnCollectibleCollected();

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
                ScoreManager.Instance.IncreaseScore(125);
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Pedal"))
        {
            Debug.Log("PEDAL Collected!");

            // Notify the spawner when the collectible is collected
            collectibleSpawner.OnCollectibleCollected();

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
                ScoreManager.Instance.IncreaseScore(125);
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Back_Wheel"))
        {
            Debug.Log("BACK WHEEL Collected!");

            // Notify the spawner when the collectible is collected
            collectibleSpawner.OnCollectibleCollected();

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
                ScoreManager.Instance.IncreaseScore(125);
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("HandleBar"))
        {
            Debug.Log("HANDLEBAR Collected!");

            // Notify the spawner when the collectible is collected
            collectibleSpawner.OnCollectibleCollected();

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
                ScoreManager.Instance.IncreaseScore(125);
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Frame"))
        {
            Debug.Log("FRAME Collected!");

            // Notify the spawner when the collectible is collected
            collectibleSpawner.OnCollectibleCollected();

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
                ScoreManager.Instance.IncreaseScore(125);
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("MissedCollectible"))
        {
            Debug.Log("Collectible MISSED!");

            // Notify the spawner when the collectible is missed
            collectibleSpawner.OnCollectibleMissed();
        }
    }
}