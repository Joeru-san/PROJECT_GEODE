using UnityEngine;

[CreateAssetMenu(fileName = "NewWorldItem", menuName = "Items/WorldItem")]
public class WorldItem : BaseItem
{
    public void Awake()
    {
        itemType = ItemType.Item1;
    }
}


