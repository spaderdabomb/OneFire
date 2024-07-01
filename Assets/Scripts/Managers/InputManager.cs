using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PlayerInputManager = GinjaGaming.FinalCharacterController.PlayerInputManager;

[DefaultExecutionOrder(-4)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public PlayerInput playerInput;

    public PlayerActionsInput playerActionsInput;
    public LastInputSystem lastInputSystem { get; private set; } = LastInputSystem.KeyboardMouse;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Subscribe to Input changed event
        InputSystem.onDeviceChange += OnDeviceChange;
        playerInput = GetComponent<PlayerInput>();

    }

    private void Update()
    {
        string currentControlScheme = playerInput.currentControlScheme;
    }

    public void SetGameSceneControls()
    {
        PlayerInputManager.Instance.EnableControls();
    }

    public void SetMenuControls()
    {
        PlayerInputManager.Instance.DisableControls();
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added)
        {
            Debug.Log($"New device added: {device}");
        }
        else if (change == InputDeviceChange.Removed)
        {
            Debug.Log($"Device removed: {device}");
        }
    }

    public enum LastInputSystem
    {
        KeyboardMouse,
        Gamepad,
        Touch,
        None
    }
}
