using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    public Image inventoryIcon;
    public TextMeshProUGUI amountText;
    public bool isShown = false;
    public int slotNumber = 0;
    public BaseItem itemReference;

    public static Action<BaseItem> OnItemSelect;

    void Awake()
    {
        PlayerMovement.OnShowInventory += OnShowInventoryHandler;
    }

    private void OnShowInventoryHandler(PlayerInput input)
    {
        if (!gameObject.activeInHierarchy)
            amountText.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemReference == null) return;
        PlayerInteraction.playerInventory.DropItem(itemReference, int.Parse(amountText.text), slotNumber, true);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (itemReference == null) return;
        PlayerInteraction.playerInventory.DropItem(itemReference, int.Parse(amountText.text), slotNumber, true);
    }

    public void OnSelect(BaseEventData eventData)
    {
        amountText.color = Color.red;
        OnItemSelect?.Invoke(itemReference);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        amountText.color = Color.white;
    }
}