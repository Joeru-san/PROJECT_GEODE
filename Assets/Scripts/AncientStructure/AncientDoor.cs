using DG.Tweening;
using UnityEngine;

// In the Script Exection Order this goes after the AncientStructureManager
public class AncientDoor : MonoBehaviour
{
    public int numberOfStructuresNeeded = 0;
    [SerializeField] GameObject doorMesh;

    void Start()
    {
        AncientStructure.OnStructureComplete += OpenDoor;        
    }

    void OpenDoor(AncientStructure structure)
    {
        if(numberOfStructuresNeeded == AncientStructureManager.completedStructures)
        {
            Debug.Log($"{gameObject.name} ACTIVATED");
            doorMesh.transform.DOMove(doorMesh.transform.position + Vector3.up * 5f, 1f);
        }
    }
}