using UnityEngine;

public enum ItemType
{
    Item1,
    Item2, 
    Item3
}

public class BaseItem : ScriptableObject
{
    public new string name = "";

    public ItemType itemType;

    public Sprite inventoryIcon;
    
    public int minSpawnAmount;
    public int maxSpawnAmount;
    
    public Mesh itemMesh;
    public Material itemMaterial;

    [TextArea(15,20)]
    public string description;

    public bool isStackable = true;
    public int maxStackSize = 32;
}
