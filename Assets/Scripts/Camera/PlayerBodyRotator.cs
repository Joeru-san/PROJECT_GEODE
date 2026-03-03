using UnityEngine;
using Unity.Cinemachine;

public class PlayerBodyRotator : MonoBehaviour
{
    [SerializeField] Rigidbody playerComponent; 
    [SerializeField] CinemachineCamera firstPersonCamera;
    [SerializeField] CinemachineCamera thirdPersonCamera;
    CinemachinePanTilt fPCPantilt;
    float cameraYRotation = 0f;

    void Start()
    {
        fPCPantilt = firstPersonCamera.GetComponent<CinemachinePanTilt>();
    }

    void FixedUpdate()
    {
        if (!PlayerMovement.isDead)
        {
            cameraYRotation = thirdPersonCamera.State.RawOrientation.eulerAngles.y;
            fPCPantilt.PanAxis.Value = cameraYRotation;
            playerComponent.MoveRotation(Quaternion.Euler(0f, cameraYRotation, 0f));
        }
    }
}