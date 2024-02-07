using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        if (pauseMenuUI.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // Set time scale to 0 to pause the game

        pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Set time scale back to 1 to resume the game

        pauseMenuUI.SetActive(false);
    }

    public void OnResumeButtonClick()
    {
        ResumeGame();
    }

    public void GoToMainMenu()
    {
        // Load the scene named "MainMenu"
        SceneManager.LoadScene("MainMenu");
    }
}