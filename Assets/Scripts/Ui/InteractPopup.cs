using GinjaGaming.FinalCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static UnityEditor.Progress;

public class InteractPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keyBindLabel;
    [SerializeField] TextMeshProUGUI interactObjectText;
    private PlayerControls playerInput;
    private string currentInputKey = "";
    private PlayerInteract playerInteract;

    private void Start()
    {
        playerInput = new();

        if (InputManager.Instance.playerInputDummy.currentControlScheme == "Keyboard&Mouse")
        {
            currentInputKey = playerInput.PlayerActionsMap.Interact.GetBindingDisplayString();
        }
        else if (InputManager.Instance.playerInputDummy.currentControlScheme == "Gamepad")
        {
            currentInputKey = playerInput.PlayerActionsMap.Interact.GetBindingDisplayString(InputBinding.MaskByGroup("Gamepad"));
        }
    }

    public void ShowPopup(string objectPretextStr, string objectTextStr, Color keyBindColor)
    {
        if (!InputManager.Instance.playerActionsInput.enabled)
            return;

        gameObject.SetActive(true);
        string keybindHexColor = "#" + ColorUtility.ToHtmlStringRGBA(keyBindColor);
        keyBindLabel.text = "[" + "<color=" + keybindHexColor + ">" + currentInputKey + "</color>" + "] " + objectPretextStr;
        interactObjectText.text = objectTextStr;
    }

    public void HidePopup()
    {
        gameObject.SetActive(false);
    }
}
