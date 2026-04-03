using UnityEngine;

public class BasicShopItem : ScriptableObject
{
    public string shopItemName = "";

    public Sprite shopIcon;

    public int shopItemCost;
    public BaseItem typeOfItemRequired;

    [TextArea(15,20)]
    public string shopItemDescription;
}