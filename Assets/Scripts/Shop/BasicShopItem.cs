using UnityEngine;

public class BasicShopItem : ScriptableObject
{
    public string shopItemName = "";

    public Sprite shopIcon;

    public int shopItemCost;
    public ItemType typeOfItemRequired;

    [TextArea(15,20)]
    public string shopItemDescription;
}