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

    public VisualTreeAsset popupMenuInventory;
    public VisualTreeAsset popupMenuInventoryStatsContainer;
    public VisualTreeAsset ghostIcon;

    public InteractPopup interactPopup;

    private bool onFirstGuiUpdate = false;

    private VisualElement uiGameManagerRoot;

    [HideInInspector] public UiGameManager uiGameManager;

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
    }

    public enum MenuType
    {
        GameScene = 0,
        Options = 1,
        Map = 2,
        Interact = 3,
        Crafting = 4,
    }
}
