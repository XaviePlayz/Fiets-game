using UnityEngine;
using UnityEngine.EventSystems;

public class CheckForController : MonoBehaviour
{
    #region Singleton

    private static CheckForController _instance;
    public static CheckForController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CheckForController>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(CheckForController).Name;
                    _instance = obj.AddComponent<CheckForController>();
                }
            }
            return _instance;
        }
    }
    #endregion

    public bool isControllerConnected = false;

    void Start()
    {
        isControllerConnected = false;
    }

    void Update()
    {
        string[] joystickNames = Input.GetJoystickNames();

        // Check if any joystick is currently connected
        bool foundController = false;
        foreach (var name in joystickNames)
        {
            if (!string.IsNullOrEmpty(name))
            {
                foundController = true;
                break;
            }
        }

        // Failsafe check for controller reconnection through input movement
        // This check now only considers joystick axis values, not keyboard inputs
        if (!foundController)
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
            {
                foundController = true;
            }
        }

        // If a controller is connected
        if (foundController)
        {
            if (!isControllerConnected)
            {
                Debug.Log("Controller is connected.");

                if (PauseMenu.Instance.isPaused && !PauseMenu.Instance.isInOptions)
                {
                    if (isControllerConnected)
                    {
                        EventSystem.current.SetSelectedGameObject(PauseMenu.Instance.resumeButton);
                    }
                    else if (!isControllerConnected)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
                if (PauseMenu.Instance.isInOptions)
                {
                    if (isControllerConnected)
                    {
                        EventSystem.current.SetSelectedGameObject(PauseMenu.Instance.saveOptionsButton);
                    }
                    else if (!isControllerConnected)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
            }
            isControllerConnected = true;            
        }
        else
        {
            if (isControllerConnected)
            {
                Debug.Log("Controller is disconnected.");
            }
            isControllerConnected = false;
        }
    }
}