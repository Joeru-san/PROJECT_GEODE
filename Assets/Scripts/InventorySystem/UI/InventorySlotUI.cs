using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image inventoryIcon;
    public TextMeshProUGUI amountText;
    public bool isShown = false;

    public BaseItem itemReference;

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerInteraction.playerInventory.DropItem(itemReference, int.Parse(amountText.text), true);
    }
}