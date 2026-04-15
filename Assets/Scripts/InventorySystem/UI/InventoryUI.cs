using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory; // The Inventory component of the player
    public GameObject slotPrefab; // The prefab of the slots
    public Transform slotPanel; // The Panel to put the slots into

    InventorySlotUI[] _uiSlots;

    void Start()
    {
        inventory.OnInventoryChanged += UpdateUI; // Subscribe to the inventory changed event
        PlayerMovement.OnShowInventory += ShowInventory; // Subscribe to the inventory show event
        InitializeUI();
    }
    
    /// <summary>
    /// Create all the UI Slots based on the inventory size
    /// </summary>
    void InitializeUI()
    {
        _uiSlots = new InventorySlotUI[inventory.inventorySize]; // Create as many _uiSlots as the size of the inventory
        for (int i = 0; i < inventory.inventorySize; i++) // Loop through the slots
        {
            GameObject slotInstance = Instantiate(slotPrefab, slotPanel); // Instantiate the slot prefab
            _uiSlots[i] = slotInstance.GetComponent<InventorySlotUI>(); // Assign the insantiated prefab to the _uiSlots array
        }
        UpdateUI(); // This method (InitializeUI) is called only in the Start, so the UI has to be updated so it can be displayed
    }

    /// <summary>
    /// This method is called automatically whenever the inventory changes.
    /// </summary>
    void UpdateUI()
    {
        for (int i = 0; i < _uiSlots.Length; i++) // Loop through the slots
        {
            if (inventory.inventorySlots[i].item != null)   // If the slot has an item
            {
                _uiSlots[i].inventoryIcon.sprite = inventory.inventorySlots[i].item.inventoryIcon; 
                _uiSlots[i].inventoryIcon.enabled = true;
                _uiSlots[i].amountText.text = inventory.inventorySlots[i].amount.ToString();
                _uiSlots[i].isShown = true;
                _uiSlots[i].itemReference = inventory.inventorySlots[i].item;
                _uiSlots[i].slotNumber = i;
            }
            else // If the slot is null we don't display it
            {
                _uiSlots[i].inventoryIcon.enabled = false;  
                _uiSlots[i].amountText.text = "";
                _uiSlots[i].isShown = false;
            }
        }
    }

    /// <summary>
    /// When the inventory key is pressed we call this method, we treat this as a toggle
    /// </summary>
    /// <param name="playerInput"> We have PlayerInput as an argument so we can switch the ActionMap </param>
    void ShowInventory(PlayerInput playerInput) 
    {
        if(slotPanel.gameObject.activeSelf) // If the panel is active, we deactivate it, disabling everything related to it
        {
            slotPanel.gameObject.SetActive(false);  // Hide the inventoryPanel
            Cursor.lockState = CursorLockMode.Locked;   // Lock the cursor
            Cursor.visible = false; // Hide the cursor
            playerInput.SwitchCurrentActionMap("Player");   // Load the Player Action Map
            CameraController.inst.activeCamera.GetComponent<CinemachineInputAxisController>().enabled = true;   // Enable the axis controller for the active camera
        }else
        {
            slotPanel.gameObject.SetActive(true);   // Show the inventoryPanel
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Show the cursor
            playerInput.SwitchCurrentActionMap("UI");   // Load the UI Action Map
            CameraController.inst.activeCamera.GetComponent<CinemachineInputAxisController>().enabled = false;  // isable the axis controller so the player can't move the camera while moving the mouse
        }
    }
}