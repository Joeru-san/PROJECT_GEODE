using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class StructureSpawner : MonoBehaviour
{
    public static Action<PlayerInput, GameObject> OnShowShop;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float heightRaycastDistance = 100f;

    PlayerInput playerInputReference;
    bool _inShop = false;

    // Take the reference to the player input to see if the player interacts with the structure
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _inShop = true;
            playerInputReference = other.transform.parent.GetComponent<PlayerInput>();
        }
    }

    void Update()
    {
        if((InputSystem.actions.FindAction("Interact").WasPressedThisFrame() || InputSystem.actions.FindAction("Cancel").WasPressedThisFrame()) && _inShop)
        {
            OnShowShop?.Invoke(playerInputReference, this.gameObject);
        }
    }

    // Remove the reference to the player input to avoid complications
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            _inShop = false;
            playerInputReference = null;
            Destroy(playerInputReference);
        }
    }

    /// <summary>
    /// Spawn and snap to ground a structure
    /// </summary>
    /// <param name="itemToSpawn"> The Structure item we want to spawn</param>
    public void SpawnAndSnapToGround(StructureShopItem itemToSpawn)
    {
        GameObject spawnedObject = Instantiate(itemToSpawn.structureToSpawn, spawnPoint.position, spawnPoint.rotation, transform);

        Collider _objCollider = spawnedObject.GetComponent<Collider>();
        float _halfHeight = _objCollider != null ? _objCollider.bounds.extents.y : 0f;

        Ray ray = new Ray(spawnedObject.transform.position + Vector3.up * 0.5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit _hit, heightRaycastDistance))
        {
            spawnedObject.transform.position = _hit.point + Vector3.up * _halfHeight;
        }
        else
        {
            Debug.LogWarning("SnapToGround: No ground found below the spawn point.");
        }

        OnShowShop?.Invoke(playerInputReference, null);
        playerInputReference = null;
        Destroy(playerInputReference);
        gameObject.GetComponent<Collider>().enabled = false; // Disabling to avoid complications
        enabled = false;
    }

    void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.color = Color.green;
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(box.center, box.size);
        Gizmos.matrix = old;
        Gizmos.color = Color.white;
    }
}