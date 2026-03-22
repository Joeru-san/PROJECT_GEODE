[System.Serializable]
public class InventorySlot
{
    public BaseItem item;
    public int amount;

    public InventorySlot(BaseItem item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public int AddAmount(int amountToAdd)
    {                       
        int finalAmount = amount + amountToAdd;
        if(finalAmount <= item.maxStackSize)
        {
            amount = finalAmount; 
            return 0;
        }else if (finalAmount > item.maxStackSize)
        {               
            int amountLeft = finalAmount - item.maxStackSize;
            amount += amountToAdd - amountLeft;
            return amountLeft;
        }
        return -1;
    }
}
