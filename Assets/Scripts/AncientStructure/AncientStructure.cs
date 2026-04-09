using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
public class AncientStructure : MonoBehaviour
{
    public ItemType itemTypeNeeded;

    int _numberOfItemsLoaded = 0;
    public int numberOfItemsToLoad;

    public TextMeshPro amountText;
    public TextMeshPro hintText;
    
    bool _isInteracted = false;

    void Start()
    {
        amountText.text = _numberOfItemsLoaded + " / " + numberOfItemsToLoad;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if((InputSystem.actions.FindAction("Interact").WasPressedThisFrame() || InputSystem.actions.FindAction("Cancel").WasPressedThisFrame()) && !_isInteracted)
            {
                int tmpItems = PlayerInteraction.playerInventory.FindTotalItemAmount(itemTypeNeeded);
                if(tmpItems > 0)
                {
                    int itemsNeeded = numberOfItemsToLoad - _numberOfItemsLoaded;
                    int itemsToDeposit = Mathf.Min(tmpItems, itemsNeeded);

                    _numberOfItemsLoaded += itemsToDeposit;
                    PlayerInteraction.playerInventory.RemoveItem(itemTypeNeeded, itemsToDeposit);
                    amountText.text = _numberOfItemsLoaded + " / " + numberOfItemsToLoad;

                    if (_numberOfItemsLoaded >= numberOfItemsToLoad)
                    {
                        GetComponent<Collider>().enabled = false;
                        hintText.text = "AncientStructure complete";
                    }
                } else
                {
                    StartCoroutine(ErrorText(2f));
                }
            }
        }
    }

    IEnumerator ErrorText(float duration)
    {
        _isInteracted = true;
        
        Color originalColor = hintText.color;
        hintText.text = "NO ITEMS IN INVENTORY";
        hintText.color = Color.red;

        hintText.transform.DOShakePosition(0.5f, strength: 0.2f, vibrato: 30, randomness: 180f, fadeOut: false);
        yield return new WaitForSeconds(duration);

        hintText.color = originalColor;
        hintText.text = "Press the interaction button to deposit the items";

        _isInteracted = false;
    }
}