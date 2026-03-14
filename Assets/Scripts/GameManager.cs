using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum ControlMode
    {
        KeyboardMouse,
        Touch
    }
    public ControlMode controlMode = ControlMode.KeyboardMouse;

    public GameObject MobileControlUI;

    public void ControlModePC()
    {
        controlMode = ControlMode.KeyboardMouse;
    }
    public void ControlModeMobile()
    {
        controlMode = ControlMode.Touch;
        MobileControlUI.SetActive(true);
    }


    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed)
        {
            ToMainMenu();
        }
    }

    public void ToMainMenu()
    {
        //Check for escape button press
        FindFirstObjectByType<SceneFader>().FadeToScene("MainMenu");
    }
}
