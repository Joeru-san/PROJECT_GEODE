using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ShopSlotUI : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public BasicShopItem shopItem;
    public Image shopSlotImage;
    public TextMeshProUGUI shopText;
    public static UnityEvent<BasicShopItem> shopSlotClicked = new UnityEvent<BasicShopItem>();

    public void OnPointerClick(PointerEventData eventData)
    {
        shopSlotClicked.Invoke(shopItem);
    }
}
