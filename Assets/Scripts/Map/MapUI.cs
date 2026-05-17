using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class MapUI : MonoBehaviour
{
    [SerializeField] Transform mapPanel;

    public static bool isMapOpen = false;
    public bool isDebug = false;
    void Awake()
    {
        PlayerMovement.OnShowMap += ShowPanel;
    }

    public void ShowPanel(PlayerInput playerInput)
    {
        if(mapPanel.gameObject.activeSelf)
        {
            if(isDebug) Debug.Log($"{GetType().Name} map closed");

            mapPanel.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerInput.SwitchCurrentActionMap("Player");   
            CameraController.inst.activeCamera.GetComponent<CinemachineInputAxisController>().enabled = true; 
            
            isMapOpen = false;
        } else
        {
            if(isDebug) Debug.Log($"{GetType().Name} map opened");

            mapPanel.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerInput.SwitchCurrentActionMap("UI");   
            CameraController.inst.activeCamera.GetComponent<CinemachineInputAxisController>().enabled = false; 
            
            isMapOpen = true; 
        }
    }
}