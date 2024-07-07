using GinjaGaming.FinalCharacterController;
using OneFireUi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

[DefaultExecutionOrder(1)]
public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Visual Element Assets")]
    public UIDocument uiManagerDocument;
    public UIDocument gameSceneUIDocument;
    public VisualTreeAsset popupMenuInventory;
    public VisualTreeAsset popupMenuInventoryStatsContainer;
    public VisualTreeAsset ghostIcon;

    public InteractPopup interactPopup;

    private bool onFirstGuiUpdate = false;

    private VisualElement uiGameManagerRoot;
    private VisualElement gameSceneRoot;

    [HideInInspector] public UiGameManager uiGameManager;
    [HideInInspector] public GameSceneUi gameSceneUi;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        uiGameManagerRoot = uiManagerDocument.rootVisualElement;
        uiGameManager = new UiGameManager(uiGameManagerRoot);

        gameSceneRoot = gameSceneUIDocument.rootVisualElement;
        gameSceneRoot.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        gameSceneUi = new GameSceneUi(gameSceneRoot);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (!onFirstGuiUpdate)
        {
            onFirstGuiUpdate = true;
        }   
    }

    public void ToggleOptionsMenu()
    {
        if (optionsMenuUi.rootElement.style.display == DisplayStyle.None)
        {
            optionsMenuUi.rootElement.style.display = DisplayStyle.Flex;
            gameSceneUi.rootElement.style.display = DisplayStyle.None;
            SetPlayerInMenuOptions(MenuType.Options);
        }
        else
        {
            optionsMenuUi.rootElement.style.display = DisplayStyle.None;
            gameSceneUi.rootElement.style.display = DisplayStyle.Flex;
            SetPlayerInMenuOptions(MenuType.GameScene);
        }
    }

    public void SetCursorStateVisible(bool newState)
    {
        if (newState)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    public void SetPlayerInMenuOptions(MenuType menuType)
    {

        if (menuType == MenuType.Options || menuType == MenuType.Map)
        {
            InputManager.Instance.SetMenuControls();
            SetCursorStateVisible(true);
        }
        else if (menuType == MenuType.GameScene)
        {
            InputManager.Instance.SetGameSceneControls();
            SetCursorStateVisible(false);
        }
    }

    public enum MenuType
    {
        GameScene = 0,
        Options = 1,
        Map = 2,
    }
}
