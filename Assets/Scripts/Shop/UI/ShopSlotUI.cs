using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ShopSlotUI : MonoBehaviour, IPointerClickHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    [HideInInspector] public BasicShopItem shopItem;
    public Image shopSlotImage;
    public TextMeshProUGUI shopText;
    public static UnityEvent<BasicShopItem> shopSlotClicked = new UnityEvent<BasicShopItem>();

    // Mouse click
    public void OnPointerClick(PointerEventData eventData)
    {
        shopSlotClicked.Invoke(shopItem);
    }

    // Controller A/Cross button
    public void OnSubmit(BaseEventData eventData)
    {
        shopSlotClicked.Invoke(shopItem);
    }

    // Controller navigates TO this slot
    public void OnSelect(BaseEventData eventData)
    {
        shopText.color = Color.red;
    }

    // Controller navigates AWAY from this slot
    public void OnDeselect(BaseEventData eventData)
    {
        shopText.color = Color.white;
    }
}
