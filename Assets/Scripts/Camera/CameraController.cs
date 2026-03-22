using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    int _activeCameraPriorityModifier = 69420;

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
        ChangeCamera();
    }

    public void ChangeCamera()
    {
        if(thirdPersonCamera == activeCamera)
        {
            SetCameraPriorities(thirdPersonCamera, firstPersonCamera);
            isFirstPerson = true;
        } else if(firstPersonCamera == activeCamera)
        {
            SetCameraPriorities(firstPersonCamera, thirdPersonCamera);
            isFirstPerson = false;
        } else 
        {
            thirdPersonCamera.Priority += _activeCameraPriorityModifier; 
            activeCamera = thirdPersonCamera;
        }
    }

    void SetCameraPriorities(CinemachineVirtualCameraBase currentCameraMode, CinemachineVirtualCameraBase newCameraMode)
    {
        currentCameraMode.Priority -= _activeCameraPriorityModifier;
        newCameraMode.Priority += _activeCameraPriorityModifier;
        activeCamera = newCameraMode;

    }
}
