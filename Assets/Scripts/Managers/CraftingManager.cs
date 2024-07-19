using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

public class CraftingManager : SerializedMonoBehaviour
{
    public static CraftingManager Instance;

    [SerializeField] private VisualTreeAsset craftingSlotAsset;
    [SerializeField] private VisualTreeAsset craftingContainerAsset;
    [SerializeField] private VisualTreeAsset craftingMenuAsset;

    public CraftingMenu PlayerCraftingMenu { get; private set; } = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }

    public void ShowCraftingMenu()
    {
        VisualElement craftingMenuClone = craftingMenuAsset.CloneTree();
        PlayerCraftingMenu = new CraftingMenu(craftingMenuClone);

        UiManager.Instance.uiGameManager.PlayerInteractionMenu.root.Clear();
        UiManager.Instance.uiGameManager.PlayerInteractionMenu.root.Add(craftingMenuClone);
    }
}
