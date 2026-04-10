using System.Collections.Generic;
using UnityEngine;

public class AncientStructureManager : MonoBehaviour
{
    public static Dictionary<AncientStructure, bool> ancientStructureReferences = new Dictionary<AncientStructure, bool>();

    public void printDictionary()
    {
        foreach(AncientStructure index in ancientStructureReferences.Keys)
        {
            if(ancientStructureReferences[index])
                Debug.Log(index.name + ancientStructureReferences[index]);
        }
    }
}