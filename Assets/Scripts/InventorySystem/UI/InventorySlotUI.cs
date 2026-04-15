using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// This script sends to the player inventory the informations of the item that is going to be dropped
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image inventoryIcon;
    public TextMeshProUGUI amountText;
    public bool isShown = false;
    public int slotNumber = 0;

    public BaseItem itemReference;

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerInteraction.playerInventory.DropItem(itemReference, int.Parse(amountText.text), slotNumber, true);
    }
}