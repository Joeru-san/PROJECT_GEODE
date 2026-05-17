using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemDescription : MonoBehaviour
{
    public Transform descriptionPanel;
    public Image currentSelectedItem;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI totalAmountText;

    void Awake()
    {
        InventorySlotUI.OnItemSelect += UpdatePanel;
        PlayerMovement.OnShowInventory += WipePanel;
    }

    void Start()
    {
        currentSelectedItem.DOFade(0f, 0.1f);
        itemName.text = "";
        descriptionText.text = "";
        totalAmountText.text = "";
    }

    void UpdatePanel(BaseItem itemReference)
    {
        if(itemReference == null)
            return;

        if(currentSelectedItem.color.a == 0f) currentSelectedItem.DOFade(1f, 0.1f);
        
        currentSelectedItem.sprite = itemReference.inventoryIcon;
        itemName.text = itemReference.name;
        descriptionText.text = itemReference.description;

        totalAmountText.text = "Total amount: " + PlayerInteraction.playerInventory.FindTotalItemAmount(itemReference.itemType);
    } 

    void WipePanel(PlayerInput input)
    {
        currentSelectedItem.DOFade(0f, 0.1f);
        itemName.text = "";
        descriptionText.text = "";
        totalAmountText.text = "";
    }
}