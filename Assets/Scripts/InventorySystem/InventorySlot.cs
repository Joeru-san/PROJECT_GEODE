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

    public int RemoveAmount(int amountToRemove)
    {
        if(amountToRemove > amount)
        {
            int amountLacking = amountToRemove - amount;
            amount = 0;
            return amountLacking;
        }
        else
        {
            amount -= amountToRemove;
            return 0;
        }
    }
}
