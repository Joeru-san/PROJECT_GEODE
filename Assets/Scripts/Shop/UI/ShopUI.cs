using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class ShopUI : MonoBehaviour
{
    public static Action OnStructureBuild;

    public Transform itemDisplayPanel;
    public Transform shopPanel;
    public Image selectedItemImage;
    public Image itemRequiredImage;
    public TextMeshProUGUI buyButtonText;
    public TextMeshProUGUI feedbackText;
    public GameObject slotPrefab;
    
    public TextMeshProUGUI hintText;

    public InputActionReference exitAction;

    BasicShopItem _selectedItem;
    public BasicShopItem[] listOfShopItems;
    ShopSlotUI[] _uiSlots;

    GameObject _shopPlaceReference;

    void Start()
    {
        StructureSpawner.OnShowShop += ShowPanel;
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

    /// <summary>
    /// Create all the slots in the shop based on the size of the listOfShopItems
    /// </summary>
    void InitializeUI()
    {
        int numberOfItems = listOfShopItems.Length;
        _uiSlots = new ShopSlotUI[numberOfItems];
        for (int i = 0; i < numberOfItems; i++)
        {
            GameObject slotInstance = Instantiate(slotPrefab, itemDisplayPanel);
            _uiSlots[i] = slotInstance.GetComponent<ShopSlotUI>();
            
            _uiSlots[i].shopItem = listOfShopItems[i];
            _uiSlots[i].shopSlotImage.sprite = listOfShopItems[i].shopIcon;
            _uiSlots[i].shopText.text = listOfShopItems[i].shopItemName;
        }
    }

    /// <summary>
    /// Change the sprites and the cost based on the clicked item
    /// </summary>
    /// <param name="clickedItem"></param>
    public void UpdateUI(BasicShopItem clickedItem)
    {
        _selectedItem = clickedItem;
        selectedItemImage.sprite = clickedItem.shopIcon;
        itemRequiredImage.sprite = clickedItem.typeOfItemRequired.inventoryIcon;
        buyButtonText.text = " x" + clickedItem.shopItemCost.ToString();
    }

    /// <summary>
    /// Buy an item from the shop
    /// If it's a structure spawn it
    /// </summary>
    public void BuyItem()
    {
        if(_selectedItem.shopItemCost > PlayerInteraction.playerInventory.FindTotalItemAmount(_selectedItem.typeOfItemRequired.itemType))
        {
            StartCoroutine(FeedbackTextChange("Not enough items"));
            return;
        }

        PlayerInteraction.playerInventory.RemoveItem(_selectedItem.typeOfItemRequired.itemType, _selectedItem.shopItemCost);
        buyButtonText.text = "Item buyed!";
        
        if(_selectedItem is StructureShopItem structureShopItem)
        {
            OnStructureBuild?.Invoke();
            _shopPlaceReference.GetComponent<StructureSpawner>().SpawnAndSnapToGround(structureShopItem);
        }
    }

    /// <summary>
    /// Change the Feedback text of the shop
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    IEnumerator FeedbackTextChange(string text)
    {
        if(text == null) yield return null;

        feedbackText.text = text;
        buyButtonText.text = "Select an item to buy";
        
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        feedbackText.gameObject.SetActive(false);
    }


    /// <summary>
    /// When the inventory key is pressed we call this method, we treat this as a toggle
    /// </summary>
    /// <param name="playerInput"> We have PlayerInput as an argument so we can switch the ActionMap </param>
    /// <param name="shopPlaceReference"> Reference to the shop position, in case the player wants to spawn a structure </param>
    void ShowPanel(PlayerInput playerInput, GameObject shopPlaceReference = null)
    {
        if(shopPlaceReference != null) this._shopPlaceReference = shopPlaceReference;
        
        if(shopPanel.gameObject.activeSelf) // If the panel is active, we deactivate it, disabling everythingC related to it
        {
            shopPanel.gameObject.SetActive(false);  // Hide the inventoryPanel
            Cursor.lockState = CursorLockMode.Locked;   // Lock the cursor
            Cursor.visible = false; // Hide the cursor
            playerInput.SwitchCurrentActionMap("Player");   // Load the Player Action Map
            CameraController.inst.activeCamera.GetComponent<CinemachineInputAxisController>().enabled = true;   // Enable the axis controller for the active camera
            buyButtonText.text = "Select an item to buy";
            selectedItemImage.sprite = null;
            
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