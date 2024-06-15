using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class AudioVolumeController : MonoBehaviour
{
    [Header("Volume")]
    public Slider volumeSlider;
    private const string VolumeKey = "Volume";

    [Header("Scenes")]
    public GameObject pauseMenu;
    public GameObject options;
    float gameVolume;

    private void Start()
    {
        // Load Game
        pauseMenu.SetActive(false);
        options.SetActive(false);

        // Load the saved volume value
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        // Attach a listener to the slider's OnValueChanged event
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OpenSettings()
    {
        PauseMenu.Instance.isInOptions = true;

        if (CheckForController.Instance.isControllerConnected)
        {
            EventSystem.current.SetSelectedGameObject(PauseMenu.Instance.saveOptionsButton);
        }
        else if (!CheckForController.Instance.isControllerConnected)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        pauseMenu.SetActive(false);
        options.SetActive(true);
        SetVolume(gameVolume);

        // Load the saved volume value
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void ConfirmSettings()
    {
        PauseMenu.Instance.isInOptions = false;

        if (CheckForController.Instance.isControllerConnected)
        {
            EventSystem.current.SetSelectedGameObject(PauseMenu.Instance.optionsButton);
        }
        else if (!CheckForController.Instance.isControllerConnected)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        pauseMenu.SetActive(true);
        options.SetActive(false);
        SetVolume(gameVolume);
    }

    public void OnVolumeChanged(float volume)
    {
        // Set the volume for all audio sources
        SetVolume(volume);
        gameVolume = volume;

        // Save the volume value
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }

    public void SetVolume(float volume)
    {
        // Find all instances of AudioSource in the scene
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        // Set the volume for all audio sources
        foreach (var audioSource in audioSources)
        {
            audioSource.volume = volume;
        }
    }
}