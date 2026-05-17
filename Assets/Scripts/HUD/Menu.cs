using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] Button firstSelectedButton;

    void Start()
    {
        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (firstSelectedButton != null)
                EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }
}