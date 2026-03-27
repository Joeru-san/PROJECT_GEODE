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
    
    public bool DropItem(BaseItem item, int amount, bool worldDrop = false)
    {
        foreach(InventorySlot slot in inventorySlots)
        {
            if(slot.item == item)
            {
                slot.RemoveAmount(amount);
                slot.item = null;

                if(worldDrop)
                {
                    GameObject spawnedObject = ObjectPooler.inst.SpawnFromPool(item.name, new Vector3(transform.position.x, transform.position.y+1, transform.position.z+1) + Vector3.forward, transform.rotation, ObjectPooler.inst.transform);
                    
                    // Assign the inventory amount to the spawned item instead of using the random value from Awake.
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
        }
        return false;
    }
}