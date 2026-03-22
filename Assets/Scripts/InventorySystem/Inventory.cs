using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event Action OnInventoryChanged;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public int inventorySize = 15;

    void Awake()
    {
        inventorySlots = Enumerable.Range(0, inventorySize) // We create a number sequence that goes from 0 to inventorySize-1 
        .Select(_ => new InventorySlot(null, 0)) // For each element in the sequence we create an emptySlot
        .ToList(); // The select result will be converted in a list, without this method it remains Enumerable
    }

    public bool AddItem(BaseItem item, int amount)
    {
        // Slot of the item present in the inventory
        foreach(InventorySlot slot in inventorySlots)
        {
            if(slot.item != null && slot.item == item && item.isStackable && slot.amount < item.maxStackSize)
            {
                int amountLeft = slot.AddAmount(amount);
                if(amountLeft > 0)
                {
                    AddItem(item, amountLeft);
                }
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // Item slot not in inventory or full
        foreach(InventorySlot slot in inventorySlots)
        {
            if(slot.item == null)
            {
                slot.item = item;
                slot.amount = amount;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false; // Full inventory
    }
    
    public bool DropItem(BaseItem item, int amount)
    {
        foreach(InventorySlot slot in inventorySlots)
        {
            if(slot.item == item)
            {
                if(slot.amount <= amount)
                {
                    slot.item = null;
                    slot.amount = 0;
                }
                else
                {
                    slot.amount -= amount;
                }
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }
}
