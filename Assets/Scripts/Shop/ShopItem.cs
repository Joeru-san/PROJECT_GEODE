using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/ShopItem")]
public class ShopItem : BasicShopItem
{
    public void Awake()
    {
        typeOfItemRequired = ItemType.Item1;
    }
}

