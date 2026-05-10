using UnityEngine;
using Unity.Cinemachine;

// With this script we sync the PanTilt Pan (X axis) of the first person camera with the X axis of the third person camera, so when switching there is a seamless switch
// We also move the rigidbody of the player, so it faces where tha camera rotates
public class PlayerBodyRotator : MonoBehaviour
{
    [SerializeField] Rigidbody playerComponent; 
    [SerializeField] CinemachineCamera firstPersonCamera;
    [SerializeField] CinemachineCamera thirdPersonCamera;
    CinemachinePanTilt fPCPantilt;
    CinemachinePanTilt tPCPantilt;
    CinemachineOrbitalFollow tPCOrbitalFollow;
    float cameraYRotation = 0f;

    void Start()
    {
        fPCPantilt = firstPersonCamera.GetComponent<CinemachinePanTilt>();
        tPCPantilt = thirdPersonCamera.GetComponent<CinemachinePanTilt>();
        tPCOrbitalFollow = thirdPersonCamera.GetComponent<CinemachineOrbitalFollow>();
    }

    void FixedUpdate()
    {
        if (!PlayerMovement.isDead)
        {
            // Instead of using the camera's final orientation (which includes the damping wobble),
            // we read the raw mouse input rotation directly from the camera's component.
            if (tPCPantilt != null)
            {
                cameraYRotation = tPCPantilt.PanAxis.Value;
            }
            else if (tPCOrbitalFollow != null)
            {
                cameraYRotation = tPCOrbitalFollow.HorizontalAxis.Value;
            }
            else
            {
                // Fallback just in case
                cameraYRotation = thirdPersonCamera.State.RawOrientation.eulerAngles.y;
            }

            fPCPantilt.PanAxis.Value = cameraYRotation; // Syncing the x axis of the PanTilt
            playerComponent.MoveRotation(Quaternion.Euler(0f, cameraYRotation, 0f)); // Rotating the player towards the direction the camera is facing
        }
    }
}