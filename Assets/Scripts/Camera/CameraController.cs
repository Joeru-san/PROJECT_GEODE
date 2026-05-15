using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    int _activeCameraPriorityModifier = 69420; // Modifier offset to change the camera priority

    [HideInInspector] public CinemachineVirtualCameraBase activeCamera; 
    public Camera MainCamera;
    public CinemachineVirtualCameraBase firstPersonCamera;
    public CinemachineVirtualCameraBase thirdPersonCamera;

    public static CameraController inst {get; private set;}
    public static bool isFirstPerson = false; 

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
        ChangeCamera(); // Calling it the first time so we can already se the priorities
    }

    /// <summary>
    /// Change the active camera between First Person and Third Person
    /// </summary>
    public void ChangeCamera()
    {
        if(thirdPersonCamera == activeCamera) // If in third person
        {
            SetCameraPriorities(thirdPersonCamera, firstPersonCamera);
            isFirstPerson = true;
        } else if(firstPersonCamera == activeCamera)    // If in first person person
        {
            SetCameraPriorities(firstPersonCamera, thirdPersonCamera);
            isFirstPerson = false;
        } else // Fallback case
        {
            firstPersonCamera.Priority += _activeCameraPriorityModifier; 
            activeCamera = firstPersonCamera;
        }
    }

    /// <summary>
    /// Set priorities of the cameras
    /// </summary>
    /// <param name="currentCameraMode"></param>
    /// <param name="newCameraMode"></param>
    void SetCameraPriorities(CinemachineVirtualCameraBase currentCameraMode, CinemachineVirtualCameraBase newCameraMode)
    {
        currentCameraMode.Priority -= _activeCameraPriorityModifier;
        newCameraMode.Priority += _activeCameraPriorityModifier;
        activeCamera = newCameraMode;
    }

    public void ChangeGainAllCameras(float newGain)
    {
        CinemachineVirtualCameraBase[] allCameras = { firstPersonCamera, thirdPersonCamera };
        
        foreach (var camera in allCameras)
        {
            if (camera == null) continue;
            var axisController = camera.GetComponent<CinemachineInputAxisController>();
            if (axisController == null) continue;

            for (int i = 0; i < axisController.Controllers.Count; i++)
            {
                var controller = axisController.Controllers[i];
                if (controller.Name == "Look Orbit X" || controller.Name == "Look X (Pan)")
                {
                    controller.Input.Gain = newGain;
                    axisController.Controllers[i] = controller;
                }
                else if (controller.Name == "Look Orbit Y" || controller.Name == "Look Y (Tilt)")
                {
                    controller.Input.Gain = -newGain;
                    axisController.Controllers[i] = controller;
                }
            }
        }
    }
}
