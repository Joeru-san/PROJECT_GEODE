using UnityEngine;

[CreateAssetMenu(fileName = "NewItem1", menuName = "Items/Item1")]
public class item1 : BaseItem
{
    public void Awake()
    {
        itemType = ItemType.Item1;
    }
}


