using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/ShopItem")]
public class StructureShopItem : BasicShopItem
{
    public GameObject structureToSpawn;
    public void Awake()
    {
        typeOfItemRequired.itemType = ItemType.Item1;
    }
}

