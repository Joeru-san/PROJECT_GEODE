using System.Collections;
using DG.Tweening;
using UnityEngine;

// In the Script Exection Order this goes after the AncientStructureManager
public class AncientDoor : MonoBehaviour
{
    public int numberOfStructuresNeeded = 0;    // Number of completed ancient structures needed to make the door open
    [SerializeField] GameObject doorMesh;

    void Start()
    {
        AncientStructure.OnStructureComplete += OpenDoor;        
    }

    void OpenDoor(AncientStructure structure)
    {
        if(numberOfStructuresNeeded == AncientStructureManager.completedStructures)
        {
            DayNightController.inst.dayDurationSeconds = 1f;
            StartCoroutine(DelaySceneLoad(10f));
        }
    }
    IEnumerator DelaySceneLoad(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        PauseManager.inst.LoadScene("MainScene");
    }
}