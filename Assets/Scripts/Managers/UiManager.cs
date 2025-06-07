using GinjaGaming.FinalCharacterController;
using OneFireUi;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

[DefaultExecutionOrder(-1)]
public class UiManager : SerializedMonoBehaviour
{
    public static UiManager Instance;

    [Header("Visual Element Assets")]
    public UIDocument uiManagerDocument;

    public VisualTreeAsset popupMenuInventory;
    public VisualTreeAsset popupMenuInventoryStatsContainer;
    public VisualTreeAsset ghostIcon;
    public VisualTreeAsset healthBar;
    public VisualTreeAsset collectionsBiomeTab;
    public VisualTreeAsset collectionSlot;
    public VisualTreeAsset raritySlotContainer;

    [Header("UI Textures")]
    public Texture2D questStatusIconDark;
    public Texture2D questStatusIconLight;
    public Texture2D statusLightLit;
    public Texture2D statusLightUnlit;
    public Dictionary<ItemRarity, Sprite> trophyTextures;  
    public Dictionary<ItemRarity, Sprite> trophyTexturesDark;  

    [Header("Damage Numbers")]
    public GameObject damageNumberStandard;

    [Header("UGUI")]
    public Canvas overlayCanvas;
    public Canvas perspectiveCanvas;
    public GameObject effectsContainerUI;
    public InteractPopup interactPopup;
    public UiGameManager uiGameManager;
    private VisualElement uiGameManagerRoot;


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

    private void Start()
    {
        uiGameManagerRoot = uiManagerDocument.rootVisualElement;
        uiGameManager.Init(uiGameManagerRoot);
    }
}

public enum MenuType
{
    GameScene = 0,
    Options = 1,
    Map = 2,
    Interact = 3,
    Crafting = 4,
}
