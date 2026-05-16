using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    public Image inventoryIcon;
    public TextMeshProUGUI amountText;
    public bool isShown = false;
    public int slotNumber = 0;
    public BaseItem itemReference;

    void Awake()
    {
        PlayerMovement.OnShowInventory += OnShowInventoryHandler;
    }

    private void OnShowInventoryHandler(PlayerInput input)
    {
        // Only reset color if the inventory is closing (panel becoming inactive)
        if (!gameObject.activeInHierarchy)
            amountText.color = Color.white;
    }

    // Mouse click
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemReference == null) return;
        PlayerInteraction.playerInventory.DropItem(itemReference, int.Parse(amountText.text), slotNumber, true);
    }

    // Controller A/Cross button
    public void OnSubmit(BaseEventData eventData)
    {
        if (itemReference == null) return;
        PlayerInteraction.playerInventory.DropItem(itemReference, int.Parse(amountText.text), slotNumber, true);
    }

    // Controller navigates TO this slot
    public void OnSelect(BaseEventData eventData)
    {
        amountText.color = Color.red;
    }

    // Controller navigates AWAY from this slot
    public void OnDeselect(BaseEventData eventData)
    {
        amountText.color = Color.white;
    }
}