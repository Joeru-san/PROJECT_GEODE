using UnityEngine;

[CreateAssetMenu(fileName = "NewStructureItem", menuName = "Shop/StrctureShopItem")]
public class StructureShopItem : BasicShopItem
{
    public GameObject structureToSpawn;
    public void Awake()
    {
        typeOfItemRequired.itemType = ItemType.Item1;
    }
}

