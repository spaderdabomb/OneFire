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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }

    public void ShowCraftingMenu()
    {

    }
}
