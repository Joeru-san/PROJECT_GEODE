using System.Collections.Generic;
using UnityEngine;

// In the Script Exection Order this goes before the AncientStructure related scripts
public class AncientStructureManager : MonoBehaviour
{
    public static Dictionary<AncientStructure, bool> ancientStructureReferences = new Dictionary<AncientStructure, bool>();
    public static int completedStructures = 0;

    void Start()
    {
        AncientStructure.OnStructureComplete += RegisterStructureStatus;
    }

    public void RegisterStructureStatus(AncientStructure structureReference)
    {
        ancientStructureReferences[structureReference] = true;
        completedStructures++;
        print(completedStructures);
    }
}