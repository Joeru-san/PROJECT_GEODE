using System.Collections.Generic;
using UnityEngine;

// In the Script Exection Order this goes before the AncientStructure related scripts
public class AncientStructureManager : MonoBehaviour
{
    // The AncientStructure in the scene register themselves in this dictionary
    public static Dictionary<AncientStructure, bool> ancientStructureReferences = new Dictionary<AncientStructure, bool>();
    public static int completedStructures = 0;  // Having a separate variable so we don't have to iterate the dictionary checking how many structures are completed

    void Start()
    {
        AncientStructure.OnStructureComplete += RegisterStructureStatus;
    }

    /// <summary>
    /// Function to update the Ancient Structures status to true if they are completed
    /// </summary>
    /// <param name="structureReference"> Reference of the Ancient Structure, needed to upgrade the dictionary</param>
    public void RegisterStructureStatus(AncientStructure structureReference)
    {
        ancientStructureReferences[structureReference] = true;
        completedStructures++;
        Debug.Log(completedStructures);
    }
}