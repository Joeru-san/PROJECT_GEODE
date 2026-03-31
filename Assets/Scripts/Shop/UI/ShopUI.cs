using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ShopUI : MonoBehaviour
{
    public Transform itemDisplayPanel;
    public Transform shopPanel;
    public Image selectedItemImage;
    public TextMeshProUGUI buyButtonText;
    public GameObject slotPrefab;
    public BasicShopItem[] listOfShopItems;
    ShopSlotUI[] _uiSlots;

    void Start()
    {
        StructureSpawner.OnShowShop += ShowInventory;
        InitializeUI();
    }

    void OnEnable()
    {
        ShopSlotUI.shopSlotClicked.AddListener(UpdateUI);
    }

    void OnDisable()
    {
        ShopSlotUI.shopSlotClicked.RemoveListener(UpdateUI);
    }

    void InitializeUI()
    {
        int numberOfItems = listOfShopItems.Length;
        _uiSlots = new ShopSlotUI[numberOfItems];
        for (int i = 0; i < numberOfItems; i++)
        {
            GameObject slotInstance = Instantiate(slotPrefab, itemDisplayPanel);
            _uiSlots[i] = slotInstance.GetComponent<ShopSlotUI>();
            
            _uiSlots[i].shopItem = listOfShopItems[i];
            _uiSlots[i].shopSlotImage.sprite =listOfShopItems[i].shopIcon;
            _uiSlots[i].shopText.text = listOfShopItems[i].shopItemName;
        }
    }

    public void UpdateUI(BasicShopItem clickedItem)
    {
        selectedItemImage.sprite = clickedItem.shopIcon;
        buyButtonText.text = " x" + clickedItem.shopItemCost.ToString();
    }

    void ShowInventory(PlayerInput playerInput) // We have PlayerInput as an argument so we can switch the ActionMap
    {
        if(shopPanel.gameObject.activeSelf) // If the panel is active, we deactivate it, disabling everything related to it
        {
            shopPanel.gameObject.SetActive(false);  // Hide the inventoryPanel
            Cursor.lockState = CursorLockMode.Locked;   // Lock the cursor
            Cursor.visible = false; // Hide the cursor
            playerInput.SwitchCurrentActionMap("Player");   // Load the Player Action Map
            CameraController.inst.activeCamera.GetComponent<CinemachineInputAxisController>().enabled = true;   // Enable the axis controller for the active camera
        }else
        {
            shopPanel.gameObject.SetActive(true);   // Show the inventoryPanel
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Show the cursor
            playerInput.SwitchCurrentActionMap("UI");   // Load the UI Action Map
            CameraController.inst.activeCamera.GetComponent<CinemachineInputAxisController>().enabled = false;  // isable the axis controller so the player can't move the camera while moving the mouse
        }
    }
}