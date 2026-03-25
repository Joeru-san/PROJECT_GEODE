using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] float thirdPersonInteractionDistance;
    [SerializeField] float firstPersonInteractionDistance;
    [SerializeField] LayerMask layerToHit;

    private Item _currentTargetedItem;

    private Inventory _playerInventory;

    void Awake()
    {
        _playerInventory = GetComponent<Inventory>();
    }

    void Update()   
    {
        float interactionDistance = CameraController.isFirstPerson ? firstPersonInteractionDistance : thirdPersonInteractionDistance;
        Ray raycastFromCamera = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit outHit;

        if (Physics.Raycast(raycastFromCamera, out outHit, interactionDistance, layerToHit))
        {
            Debug.DrawRay(raycastFromCamera.origin, raycastFromCamera.direction * outHit.distance, Color.green);

            Item hitItem = outHit.transform.GetComponent<Item>();

            if (hitItem != null)
            {
                if (_currentTargetedItem != null && _currentTargetedItem != hitItem)
                    _currentTargetedItem.HideTooltip();

                _currentTargetedItem = hitItem;
                _currentTargetedItem.ShowTooltip();
            }
            else
            {
                ClearCurrentTarget();
            }

            Vector3 targetPoint = new Vector3(Camera.main.transform.position.x, hitItem.toolTip.transform.position.y, Camera.main.transform.position.z);
        
            hitItem.toolTip.transform.LookAt(targetPoint);
            
            hitItem.toolTip.transform.Rotate(0, 180, 0);

            if(InputSystem.actions.FindAction("Interact").WasPressedThisFrame())
            {
                if(_playerInventory.AddItem(hitItem.scriptableObjectType, hitItem.amount))
                {
                    Debug.Log("OGGETTO AGGIUNTO");
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

    void ClearCurrentTarget()
    {
        if (_currentTargetedItem != null)
        {
            _currentTargetedItem.HideTooltip();
            _currentTargetedItem = null;
        }
    }
}