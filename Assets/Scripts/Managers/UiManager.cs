using GinjaGaming.FinalCharacterController;
using OneFireUi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Visual Element Assets")]
    public UIDocument optionsUIDocument;
    public UIDocument gameSceneUIDocument;

    private bool onFirstGuiUpdate = false;

    [HideInInspector] VisualElement optionsRoot;
    [HideInInspector] VisualElement gameSceneRoot;

    [HideInInspector] OptionsMenuUi optionsMenuUi;
    [HideInInspector] GameSceneUi gameSceneUi;

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
        optionsRoot = optionsUIDocument.rootVisualElement;
        optionsRoot.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        optionsMenuUi = new OptionsMenuUi(optionsRoot);

        gameSceneRoot = gameSceneUIDocument.rootVisualElement;
        gameSceneRoot.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        gameSceneUi = new GameSceneUi(gameSceneRoot);
    }

    private void OnDisable()
    {

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
        if (optionsMenuUi.root.style.display == DisplayStyle.None)
        {
            optionsMenuUi.root.style.display = DisplayStyle.Flex;
            gameSceneUi.root.style.display = DisplayStyle.None;
            SetPlayerInMenuOptions(MenuType.Options);
        }
        else
        {
            optionsMenuUi.root.style.display = DisplayStyle.None;
            gameSceneUi.root.style.display = DisplayStyle.Flex;
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
