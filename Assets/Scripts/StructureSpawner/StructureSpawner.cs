using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class StructureSpawner : MonoBehaviour
{
    [SerializeField] GameObject structureToSpawn;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float heightRaycastDistance = 100f;

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(InputSystem.actions.FindAction("Interact").WasPressedThisFrame())
            {
                GameObject spawnedObject = Instantiate(structureToSpawn, spawnPoint.position, spawnPoint.rotation, transform);
                
            }
        }
    }

    void SnapToGround(GameObject obj)
    {
        Collider _objCollider = obj.GetComponent<Collider>();
        float _halfHeight = _objCollider != null ? _objCollider.bounds.extents.y : 0f;

        Ray ray = new Ray(obj.transform.position + Vector3.up * 0.5f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit _hit, heightRaycastDistance))
        {
            obj.transform.position = _hit.point + Vector3.up * _halfHeight;
        }
        else
        {
            Debug.LogWarning("SnapToGround: No ground found below the spawn point.");
        }
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