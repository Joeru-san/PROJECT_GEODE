using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class AncientStructure : MonoBehaviour
{

    [SerializeField] Material activeMaterial;
    [SerializeField] MeshRenderer ancientStructureMeshRenderer;
    [SerializeField] Transform interactIcon;
    public static Action<AncientStructure> OnStructureComplete; // Event that is fired when the structure is completed

    public ItemType itemTypeNeeded; // The ItemType needed by the structure to be activated

    int _numberOfItemsLoaded = 0;   // How much items we loaded into the ancient structure
    public int numberOfItemsToLoad; // How many items we have to load to make the structure activate

    public TextMeshPro amountText;  // The display text to show the progress of the structure to the player
    public TextMeshPro hintText;    // The display text to guide the player in the structure usage
    
    bool _isInteracted = false; // For avoiding having continuos interactions when pressing multiple times the interaction button

    void Start()
    {
        amountText.text = _numberOfItemsLoaded + " / " + numberOfItemsToLoad; // Set the amount text to display the basic informations
        AncientStructureManager.ancientStructureReferences.Add(this, false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            interactIcon.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            interactIcon.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(InputSystem.actions.FindAction("Interact").WasPressedThisFrame() && !_isInteracted)
            {
                int itemsInInventory = PlayerInteraction.playerInventory.FindTotalItemAmount(itemTypeNeeded);   // Take all the items in the inventory and  put it in a tmp variable
                if(itemsInInventory > 0) // If there are items in the inventory
                {
                    LoadItems(itemsInInventory);
                } else  // If there are no items in the inventory, inform the player of the error
                {
                    StartCoroutine(ErrorText(2f, "NO ITEMS IN INVENTORY"));
                }
            }
        }
    }

    /// <summary>
    /// Load the items in the structure
    /// </summary>
    /// <param name="itemsToLoad"> Number of items that will be loaded in the structure</param>
    void LoadItems(int itemsToLoad)
    {
        int itemsNeeded = numberOfItemsToLoad - _numberOfItemsLoaded;   // Calculate the items needed to finish the structure
        int itemsToDeposit = Mathf.Min(itemsToLoad, itemsNeeded);  // By taking the min number between these two we avoid going over the treshold of numberOfItemsToLoad (situations like 70 / 60)

        _numberOfItemsLoaded += itemsToDeposit;
        PlayerInteraction.playerInventory.RemoveItem(itemTypeNeeded, itemsToDeposit);   // Remove the items in the inventory
        amountText.text = _numberOfItemsLoaded + " / " + numberOfItemsToLoad;   // Update the display text for the player

        if (_numberOfItemsLoaded >= numberOfItemsToLoad)    // If all the items are loaded
        {
            GetComponent<Collider>().enabled = false;   // Disabling collider to avoid retrigger the function
            hintText.text = "AncientStructure complete"; // Inform the player that the structure is completed
            OnStructureComplete?.Invoke(this);
            ancientStructureMeshRenderer.GetComponent<MeshRenderer>().material = activeMaterial;
        }
    }

    /// <summary>
    /// Inform the player of an error in the ancient structure
    /// </summary>
    /// <param name="duration"> How much the text will stay in error </param>
    /// <param name="errorText"> The text to display </param>
    /// <returns></returns>
    IEnumerator ErrorText(float duration, string errorText = "")
    {
        _isInteracted = true; // Make the interaction not trigger continously
        
        Color originalColor = hintText.color;   // Saving it so we can reassign it later
        hintText.text = errorText; 
        hintText.color = Color.red;

        hintText.transform.DOShakePosition(0.5f, strength: 0.2f, vibrato: 30, randomness: 180f, fadeOut: false); // Little animation to take the player attention
        yield return new WaitForSeconds(duration);

        // Reverting the text back to normal
        hintText.color = originalColor;
        hintText.text = "Press the interaction button to deposit the items";

        _isInteracted = false; // Renabling the interaction
    }
}