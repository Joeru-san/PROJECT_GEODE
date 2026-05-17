using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[System.Serializable]
public class ShopSlotUI : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    [HideInInspector] public BasicShopItem shopItem;
    public Image shopSlotImage;
    public TextMeshProUGUI shopText;
    public static UnityEvent<BasicShopItem> shopSlotClicked = new UnityEvent<BasicShopItem>();

    void Awake()
    {
        StructureSpawner.OnShowShop += OnShowShopHandler;
    }

    private void OnShowShopHandler(PlayerInput input, GameObject shopPlaceReference = null)
    {
        if (!gameObject.activeInHierarchy)
            shopText.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        shopSlotClicked.Invoke(shopItem);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        shopSlotClicked.Invoke(shopItem);
    }

    public void OnSelect(BaseEventData eventData)
    {
        shopText.color = Color.red;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        shopText.color = Color.white;
    }
}