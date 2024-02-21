using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectibles : MonoBehaviour
{
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
            other.gameObject.SetActive(false);
            Debug.Log("FRONT WHEEL Collected!");

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
        if (other.CompareTag("Pedal"))
        {
            other.gameObject.SetActive(false);
            Debug.Log("PEDAL Collected!");

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
        if (other.CompareTag("Back_Wheel"))
        {
            other.gameObject.SetActive(false);
            Debug.Log("BACK WHEEL Collected!");

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
        if (other.CompareTag("HandleBar"))
        {
            other.gameObject.SetActive(false);
            Debug.Log("HANDLEBAR Collected!");

            // Play the collect sound
            if (collectSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(collectSound);
            }
        }
    }
}