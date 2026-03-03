using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCameraBase _activeCamera;
    int _activeCameraPriorityModifier = 69420;

    public Camera MainCamera;
    public CinemachineVirtualCameraBase firstPersonCamera;
    public CinemachineVirtualCameraBase thirdPersonCamera;
    

    void Start()
    {
        ChangeCamera();
    }

    void Update()
    {
        if(PlayerMovement.changeCameraWasPressedThisFrame)
        {
            ChangeCamera();
        }
    }

    void ChangeCamera()
    {
        if(thirdPersonCamera == _activeCamera)
        {
            SetCameraPriorities(thirdPersonCamera, firstPersonCamera);
        } else if(firstPersonCamera == _activeCamera)
        {
            SetCameraPriorities(firstPersonCamera, thirdPersonCamera);
        } else // Parte solo allo start o con un errore degli altri if
        {
            thirdPersonCamera.Priority += _activeCameraPriorityModifier; 
            _activeCamera = thirdPersonCamera;
        }
    }

    void SetCameraPriorities(CinemachineVirtualCameraBase currentCameraMode, CinemachineVirtualCameraBase newCameraMode)
    {
        currentCameraMode.Priority -= _activeCameraPriorityModifier;
        newCameraMode.Priority += _activeCameraPriorityModifier;
        _activeCamera = newCameraMode;
    }
}
