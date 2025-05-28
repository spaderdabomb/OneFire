using UnityEngine.UIElements;
using UnityEngine;
using JSAM;
using System.Runtime.CompilerServices;

public partial class CollectionsBiomeTab
{
    public VisualElement root;
    public BiomeData biomeData;
    public CollectionsBiomeTab(VisualElement root, BiomeData biomeData, int tabIndex)
    {
        AssignQueryResults(root);
        InitBiomeTab();
        RegisterCallbacks();

        this.root = root;
        this.biomeData = biomeData;
        tabRoot.tabIndex = tabIndex;
    }

    private void InitBiomeTab()
    {

    }

    public void RegisterCallbacks()
    {
        tabRoot.RegisterValueChangedCallback(TabIndexChanged);
        tabRoot.RegisterCallback<PointerEnterEvent>(OnHover);
    }

    public void UnregisterCallbacks()
    {
        tabRoot.UnregisterValueChangedCallback(TabIndexChanged);
        tabRoot.UnregisterCallback<PointerEnterEvent>(OnHover);
    }

    public void TabIndexChanged(ChangeEvent<bool> value)
    {
        UiManager.Instance.uiGameManager.OptionsMenuUi.menuCollections.SetBiomeTabIndex(tabRoot.tabIndex);
        AudioManager.PlaySound(MainLibrarySounds.ConfirmTick);
    }

    public void SetTabSelectedValue(bool value)
    {
        tabRoot.SetValueWithoutNotify(value);
    }

    public void OnHover(PointerEnterEvent evt)
    {
        AudioManager.PlaySound(MainLibrarySounds.WoodenTick);
    }
}