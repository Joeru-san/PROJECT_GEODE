using UnityEngine;

[CreateAssetMenu(fileName = "NewSyntetizedItem", menuName = "Items/SyntetizedItem")]
public class SyntetizedItem : BaseItem
{
    public void Awake()
    {
        itemType = ItemType.Item1;
        minSpawnAmount = maxSpawnAmount;
    }
}


