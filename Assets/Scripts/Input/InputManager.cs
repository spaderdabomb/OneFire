using GinjaGaming.FinalCharacterController;
using OneFireUI;
using System;
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

    public PlayerInput playerInputDummy;
    public UiInputManager uiInputManager;
    public InventoryControlsInput inventoryControlsInput;

    public PlayerActionsInput playerActionsInput;
    public LastInputSystem lastInputSystem { get; private set; } = LastInputSystem.KeyboardMouse;

    [SerializeField] private InputActionAsset[] inputActionAssets;
    private Dictionary<string, ActionCallbacks> callbackMap = new Dictionary<string, ActionCallbacks>();

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
        playerInputDummy = GetComponent<PlayerInput>();

        EnableAllInputs();
    }

    private void OnDisable()
    {
        DisableAllInputs();
    }

    private void Start()
    {
        SetGameSceneControls();
    }

    private void Update()
    {
        string currentControlScheme = playerInputDummy.currentControlScheme;
    }

    public void SetGameSceneControls()
    {
        PlayerInputManager.Instance.EnableControls();
        uiInputManager.uiControls.Enable();
        inventoryControlsInput.InventoryControls.Disable();
    }

    public void SetMenuControls()
    {
        PlayerInputManager.Instance.DisableControls();
        uiInputManager.uiControls.Enable();
        inventoryControlsInput.InventoryControls.Enable();
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

    private struct ActionCallbacks
    {
        public Action<InputAction.CallbackContext> started;
        public Action<InputAction.CallbackContext> performed;
        public Action<InputAction.CallbackContext> canceled;
    }

    public void EnableAllInputs()
    {
        foreach (var asset in inputActionAssets)
        {
            asset.Enable();
            foreach (var actionMap in asset.actionMaps)
            {
                actionMap.Enable();
            }
        }
    }

    public void DisableAllInputs()
    {
        foreach (var asset in inputActionAssets)
        {
            asset.Disable();
        }
    }

    public void EnableInputAsset(string assetName)
    {
        InputActionAsset asset = FindInputAsset(assetName);
        if (asset != null)
        {
            asset.Enable();
            foreach (var actionMap in asset.actionMaps)
            {
                actionMap.Enable();
            }
        }
    }

    public void DisableInputAsset(string assetName)
    {
        InputActionAsset asset = FindInputAsset(assetName);
        if (asset != null) asset.Disable();
    }

    public void EnableActionMap(string assetName, string actionMapName)
    {
        InputActionAsset asset = FindInputAsset(assetName);
        if (asset != null)
        {
            InputActionMap actionMap = asset.FindActionMap(actionMapName);
            if (actionMap != null) actionMap.Enable();
        }
    }

    public void DisableActionMap(string assetName, string actionMapName)
    {
        InputActionAsset asset = FindInputAsset(assetName);
        if (asset != null)
        {
            InputActionMap actionMap = asset.FindActionMap(actionMapName);
            if (actionMap != null) actionMap.Disable();
        }
    }

    public void RegisterCallback(string actionName, Action<InputAction.CallbackContext> startedCallback = null,
                                 Action<InputAction.CallbackContext> performedCallback = null,
                                 Action<InputAction.CallbackContext> canceledCallback = null)
    {
        foreach (var asset in inputActionAssets)
        {
            InputAction action = asset.FindAction(actionName);
            if (action != null)
            {
                if (startedCallback != null) action.started += startedCallback;
                if (performedCallback != null) action.performed += performedCallback;
                if (canceledCallback != null) action.canceled += canceledCallback;

                callbackMap[actionName] = new ActionCallbacks
                {
                    started = startedCallback,
                    performed = performedCallback,
                    canceled = canceledCallback
                };
                return;
            }
        }
        Debug.LogWarning($"Action '{actionName}' not found in any input asset.");
    }

    public void UnregisterCallback(string actionName)
    {
        if (callbackMap.TryGetValue(actionName, out var callbacks))
        {
            foreach (var asset in inputActionAssets)
            {
                InputAction action = asset.FindAction(actionName);
                if (action != null)
                {
                    if (callbacks.started != null) action.started -= callbacks.started;
                    if (callbacks.performed != null) action.performed -= callbacks.performed;
                    if (callbacks.canceled != null) action.canceled -= callbacks.canceled;

                    callbackMap.Remove(actionName);
                    return;
                }
            }
        }
        Debug.LogWarning($"No callbacks registered for action '{actionName}'.");
    }

    private InputActionAsset FindInputAsset(string assetName)
    {
        return Array.Find(inputActionAssets, asset => asset.name == assetName);
    }

    public enum LastInputSystem
    {
        KeyboardMouse,
        Gamepad,
        Touch,
        None
    }
}
