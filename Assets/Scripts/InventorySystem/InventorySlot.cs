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

    /// <summary>
    /// Adds a specified amount to the current stack 
    /// If the amount exceeds the maximum stack size, the stack is filled and the leftover amount is returned
    /// </summary>
    /// <param name="amountToAdd"> The amount to add to the stack </param>
    /// <returns> 0 if the full amount fit, otherwise the leftover amount that didn't fit </returns>
    public int AddAmount(int amountToAdd)
    {
        int finalAmount = amount + amountToAdd;

        // If the total fits within the stack, add it directly
        if (finalAmount <= item.maxStackSize)
        {
            amount = finalAmount;
            return 0;
        }

        // Otherwise, fill the stack and return the overflow
        int amountLeft = finalAmount - item.maxStackSize;
        amount = item.maxStackSize;
        return amountLeft;
    }

    /// <summary>
    /// Removes a specified amount from the current stack
    /// If the amount to remove exceeds what is available, the stack is emptied and the deficit is returned
    /// </summary>
    /// <param name="amountToRemove"> The amount to remove from the stack </param>
    /// <returns> 0 if the full amount was removed, otherwise the missing amount that couldn't be removed </returns>
    public int RemoveAmount(int amountToRemove)
    {
        // If trying to remove more than available, empty the stack and return the deficit
        if (amountToRemove > amount)
        {
            int amountLacking = amountToRemove - amount;
            amount = 0;
            return amountLacking;
        }

        // Otherwise, subtract normally
        amount -= amountToRemove;
        return 0;
    }
}
