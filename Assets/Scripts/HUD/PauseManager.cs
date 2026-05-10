using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager inst { get; private set; }

    [Header("Settings")]
    [Tooltip("Can the game be paused right now? Set to false during death, cutscenes, etc.")]
    public bool canPause = true;

    [Header("Input")]
    public InputActionReference pauseAction;

    [Header("Events")]
    [Tooltip("Triggered when the game is paused. Hook up your UI Panel and Audio changes here!")]
    public UnityEvent onPause;
    
    [Tooltip("Triggered when the game is resumed.")]
    public UnityEvent onResume;

    public bool IsPaused { get; private set; }

    void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(gameObject);
            return;
        }
        inst = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ResumeGame();
    }

    void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPauseAction;
    }

    void OnDisable()
    {
        pauseAction.action.performed -= OnPauseAction;
        pauseAction.action.Disable();
    }

    void OnPauseAction(InputAction.CallbackContext context)
    {
        if (canPause)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (!canPause) return;

        IsPaused = true;
        Time.timeScale = 0f;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        onPause?.Invoke();
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        onResume?.Invoke();
    }

    public void LoadScene(string sceneName)
    {
        ResumeGame(); 
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
