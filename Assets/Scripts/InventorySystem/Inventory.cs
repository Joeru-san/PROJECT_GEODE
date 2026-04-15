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
        .Select(_ => new InventorySlot(null, 0))    // For each element in the sequence we create an emptySlot
        .ToList();  // The select result will be converted in a list, without this method it remains Enumerable
    }

    /// <summary>
    /// Add an item to the inventory
    /// If the item exists, the amount is added to an existing slot, otherwise it's qadded to the first free slot
    /// </summary>
    /// <param name="item"> The item to add </param>
    /// <param name="amount"> How many items we are adding </param>
    /// <returns> True if the item is succesfully added, false if not </returns>
    public bool AddItem(BaseItem item, int amount)
    {
        // Slot of the item present in the inventory, we add it an exisisting one
        foreach(InventorySlot slot in inventorySlots)
        {
            if(slot.item != null && slot.item == item && item.isStackable && slot.amount < item.maxStackSize)
            {
                int amountLeft = slot.AddAmount(amount);
                if(amountLeft > 0) // Handle if the maxStackSize is reached
                {
                    AddItem(item, amountLeft);
                }
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // Item slot not in inventory or full, add it to the first free slot
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

    /// <summary>
    /// Drop an item in overworld from the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <param name="inventoryIndex"></param>
    /// <param name="worldDrop"> True if the drop is in the overworld, false if not</param>
    /// <returns> True if the item was succesfully dropped, false if not</returns>
    public bool DropItem(BaseItem item, int amount, int inventoryIndex, bool worldDrop = false)
    {
        if(inventorySlots[inventoryIndex].item != null)
        {
            inventorySlots[inventoryIndex].RemoveAmount(amount);
            inventorySlots[inventoryIndex].item = null;

            if(worldDrop)
            {
                GameObject spawnedObject = ObjectPooler.inst.SpawnFromPool(item.name, new Vector3(transform.position.x, transform.position.y+1, transform.position.z) + Vector3.forward, transform.rotation);
                
                if (spawnedObject != null)
                {
                    Item itemComponent = spawnedObject.GetComponent<Item>();
                    if (itemComponent != null)
                        itemComponent.Initialize(amount);
                }
            }

            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Find the total amount of a specific ItemType
    /// </summary>
    /// <param name="item"></param>
    /// <returns> The total amount of that ItemType </returns>
    public int FindTotalItemAmount(ItemType item)
    {
        int totalAmount = 0;
        foreach(InventorySlot slot in inventorySlots)
        {
            if(slot.item != null && slot.item.itemType == item)
            {
                totalAmount += slot.amount;        
            }
        }

        return totalAmount;
    }

    /// <summary>
    /// Remove an item from the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="amount"></param>
    /// <returns> True if item was succesfully remove, false if not</returns>
    public bool RemoveItem(ItemType item, int amount)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (amount <= 0) break;

            if (slot.item != null && slot.item.itemType == item)
            {
                amount = slot.RemoveAmount(amount);

                if (slot.amount <= 0)
                    slot.item = null;
            }
        }

        OnInventoryChanged?.Invoke();
        return amount <= 0;
    }
}