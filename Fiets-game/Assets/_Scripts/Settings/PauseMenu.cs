using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    #region Singleton

    private static PauseMenu _instance;
    public static PauseMenu Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PauseMenu>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PauseMenu).Name;
                    _instance = obj.AddComponent<PauseMenu>();
                }
            }
            return _instance;
        }
    }
    #endregion

    public GameObject pauseMenuUI;
    public bool isPaused;
    public bool isInOptions;

    public GameObject resumeButton;
    public GameObject optionsButton;
    public GameObject saveOptionsButton;


    private void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
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
        isPaused = true;
        Time.timeScale = 0f; // Set time scale to 0 to pause the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        if (CheckForController.Instance.isControllerConnected)
        {
            EventSystem.current.SetSelectedGameObject(resumeButton);
        }
        else if (!CheckForController.Instance.isControllerConnected)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Set time scale back to 1 to resume the game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        if (CheckForController.Instance.isControllerConnected)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        if (!CheckForController.Instance.isControllerConnected)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
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

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}