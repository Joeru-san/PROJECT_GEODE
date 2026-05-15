using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShowKeybind : MonoBehaviour
{
    [Header("References")]
    [SerializeField] List<string> keys;
    [SerializeField] List<Sprite> spritesToShow;
    public Dictionary<string, Sprite> sprites;

    [SerializeField] private InputActionReference inputActionToShow;
    [SerializeField] private Image hudImage;
    [SerializeField] private TextMeshProUGUI textToShow;

    void Awake()
    {
        PlayerMovement.OnControlSchemeChange += ChangeValues;

        sprites = new Dictionary<string, Sprite>();

        if (keys.Count != spritesToShow.Count)
        {
            Debug.LogWarning("Keys and Sprites lists are not the same size!");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
            sprites.Add(keys[i], spritesToShow[i]);
    }

    void Start()
    {
        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
            ChangeValues(playerInput);
    }

    void OnDestroy()
    {
        PlayerMovement.OnControlSchemeChange -= ChangeValues;
    }

    void ChangeValues(PlayerInput input)
    {
        if (sprites.TryGetValue(input.currentControlScheme, out Sprite sprite) && hudImage != null)
            hudImage.sprite = sprite;

        int bindingIndex = inputActionToShow.action.GetBindingIndex(
            InputBinding.MaskByGroup(input.currentControlScheme)
        );

        if (bindingIndex != -1 && textToShow != null)
            textToShow.text = "\t" + inputActionToShow.action.GetBindingDisplayString(bindingIndex).ToString();
    }
}