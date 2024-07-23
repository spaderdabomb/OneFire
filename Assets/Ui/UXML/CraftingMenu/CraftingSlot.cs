using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

public partial class CraftingSlot
{
    public VisualElement root;
    public RecipeData recipeData;
    private CraftingSlotContainer parentContainer;

    private int slotIndex;
    public CraftingSlot(VisualElement root, RecipeData recipeData, int slotIndex, CraftingSlotContainer parentContainer)
    {
        AssignQueryResults(root);

        this.root = root;
        this.recipeData = recipeData;
        this.slotIndex = slotIndex;
        this.parentContainer = parentContainer;

        Init();
        RegisterCallbacks();
    }

    private void Init()
    {
        slotIcon.style.backgroundImage = recipeData.itemResult.itemSprite.texture;
    }

    public void RegisterCallbacks()
    {
        root.RegisterCallback<PointerEnterEvent>(PointerEnterSlot);
        root.RegisterCallback<PointerLeaveEvent>(PointerLeaveSlot);
        root.RegisterCallback<PointerDownEvent>(ClickedSlot);
    }

    public void UnregisterCallbacks()
    {
        root.UnregisterCallback<PointerEnterEvent>(PointerEnterSlot);
        root.UnregisterCallback<PointerLeaveEvent>(PointerLeaveSlot);
        root.UnregisterCallback<PointerDownEvent>(ClickedSlot);
    }

    public void PointerEnterSlot(PointerEnterEvent evt)
    {
        AudioManager.PlaySound(MainLibrarySounds.WoodenTick);
    }

    public void PointerLeaveSlot(PointerLeaveEvent evt)
    {

    }

    public void ClickedSlot(PointerDownEvent evt)
    {
        parentContainer.parentMenu.RecipeSelected(slotIndex);
    }
}
