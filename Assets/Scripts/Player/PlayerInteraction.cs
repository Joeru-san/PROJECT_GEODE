using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInteraction))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] float thirdPersonInteractionDistance;
    [SerializeField] float firstPersonInteractionDistance;
    [SerializeField] LayerMask layerToHit;
    [SerializeField] Transform interactIcon;

    private Item _currentTargetedItem;

    public static Inventory playerInventory;

    void Awake()
    {
        playerInventory = GetComponent<Inventory>();
    }

    void Update()   
    {
        float interactionDistance = CameraController.isFirstPerson ? firstPersonInteractionDistance : thirdPersonInteractionDistance;   // Check which interactiond distance to use
        Ray raycastFromCamera = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); 
        RaycastHit outHit;

        if (Physics.Raycast(raycastFromCamera, out outHit, interactionDistance, layerToHit))
        {
            Debug.DrawRay(raycastFromCamera.origin, raycastFromCamera.direction * outHit.distance, Color.green);

            interactIcon.gameObject.SetActive(true);

            Item hitItem = outHit.transform.GetComponent<Item>(); // Get the reference of the item component

            if (hitItem != null)
            {
                if (_currentTargetedItem != null && _currentTargetedItem != hitItem)
                    _currentTargetedItem.HideTooltip(); // Hide the tooltip of the previous targeted item

                _currentTargetedItem = hitItem; // Change to the current targeted item
                _currentTargetedItem.ShowTooltip(); // Show the tooltip of the currently targeted item
            }
            else
            {
                ClearCurrentTarget(); 
            }

            // Where the camera need to face toward
            Vector3 targetPoint = new Vector3(Camera.main.transform.position.x, hitItem.toolTip.transform.position.y, Camera.main.transform.position.z);
        
            hitItem.toolTip.transform.LookAt(targetPoint);
            
            hitItem.toolTip.transform.Rotate(0, 180, 0);

            if(InputSystem.actions.FindAction("Interact").WasPressedThisFrame())
            {
                if(playerInventory.AddItem(hitItem.scriptableObjectType, hitItem.amount))
                {
                    hitItem.OnObjectDestroy();
                }
                else
                {
                    Debug.Log("OGGETTO NON AGGIUNTO, INVENTARIO PIENO");
                }
            }
        }
        else
        {
            Debug.DrawRay(raycastFromCamera.origin, raycastFromCamera.direction * interactionDistance, Color.red);
            ClearCurrentTarget();
        }
    }

    /// <summary>
    /// Hide the tooltip and dereference the current targeted item
    /// </summary>
    void ClearCurrentTarget()
    {
        if (_currentTargetedItem != null)
        {
            _currentTargetedItem.HideTooltip();
            _currentTargetedItem = null;
            interactIcon.gameObject.SetActive(false);
        }
    }
}