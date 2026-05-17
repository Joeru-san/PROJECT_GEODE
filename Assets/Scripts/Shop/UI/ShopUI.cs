using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;

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

    public static bool isShopPanelOpen = false;

    public InputActionReference exitAction;

    BasicShopItem _selectedItem;
    public BasicShopItem[] listOfShopItems;
    ShopSlotUI[] _uiSlots;

    GameObject _shopPlaceReference;

    void Start()
    {
        StructureSpawner.OnShowShop += ShowPanel;
        InitializeUI();

        selectedItemImage.DOFade(0f, 0.1f);
        itemRequiredImage.DOFade(0f, 0.1f);
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
            _uiSlots[i].shopSlotImage.sprite = listOfShopItems[i].shopIcon;
            _uiSlots[i].shopText.text = listOfShopItems[i].shopItemName;
        }
    }

    public void UpdateUI(BasicShopItem clickedItem)
    {
        _selectedItem = clickedItem;

        if (selectedItemImage.color.a == 0f) selectedItemImage.DOFade(1f, 0.1f);
        if (itemRequiredImage.color.a == 0f)  itemRequiredImage.DOFade(1f, 0.1f);

        selectedItemImage.sprite = clickedItem.shopIcon;
        itemRequiredImage.sprite = clickedItem.typeOfItemRequired.inventoryIcon;
        buyButtonText.text = " x" + clickedItem.shopItemCost.ToString();
    }

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

    IEnumerator FeedbackTextChange(string text)
    {
        if(text == null) yield return null;

        feedbackText.text = text;
        buyButtonText.text = "Select an item to buy";
        
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        feedbackText.gameObject.SetActive(false);
    }

    void WipePanel()
    {
        selectedItemImage.DOFade(0f, 0.1f);
        itemRequiredImage.DOFade(0f, 0.1f);
        buyButtonText.text = "Select an item to buy";
        _selectedItem = null;
    }

    void ShowPanel(PlayerInput playerInput, GameObject shopPlaceReference = null)
    {
        if(shopPlaceReference != null) this._shopPlaceReference = shopPlaceReference;
        
        if(shopPanel.gameObject.activeSelf)
        {
            shopPanel.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerInput.SwitchCurrentActionMap("Player");
            CameraController.inst.activeCamera.GetComponent<CinemachineInputAxisController>().enabled = true;
            WipePanel();
            isShopPanelOpen = false;
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            shopPanel.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerInput.SwitchCurrentActionMap("UI");
            CameraController.inst.activeCamera.GetComponent<CinemachineInputAxisController>().enabled = false;
            isShopPanelOpen = true;
            EventSystem.current.SetSelectedGameObject(_uiSlots[0].gameObject);
        }
    }
}