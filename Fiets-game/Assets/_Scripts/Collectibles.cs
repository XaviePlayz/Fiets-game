using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectibles : MonoBehaviour
{
    public CollectibleSpawner collectibleSpawner;
    public AudioClip collectSound; // The sound to play when collected
    private AudioSource audioSource;

    public GameObject BikeProgress;
    public GameObject[] BikeParts;
    public GameObject[] Transparant;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // If AudioSource is not already attached, add it dynamically
            audioSource = gameObject.AddComponent<AudioSource>();;
        }
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
            Debug.Log("FRONT WHEEL Collected!");

            // Notify the spawner when the collectible is collected
            collectibleSpawner.OnCollectibleCollected();

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
                ScoreManager.Instance.IncreaseScore(125);

                BikeParts[0].SetActive(true);
                Transparant[0].SetActive(false);
                StartCoroutine(ShowBikeProgress());
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

                BikeParts[1].SetActive(true);
                Transparant[1].SetActive(false);
                StartCoroutine(ShowBikeProgress());
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

                BikeParts[2].SetActive(true);
                Transparant[2].SetActive(false);
                StartCoroutine(ShowBikeProgress());
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
                BikeParts[3].SetActive(true);
                Transparant[3].SetActive(false);
                StartCoroutine(ShowBikeProgress());

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

                BikeParts[4].SetActive(true);
                Transparant[4].SetActive(false);
                StartCoroutine(ShowBikeProgress());
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