// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace OneFireUi
{
    partial class OptionsMenuUi
    {
        private VisualElement optionsUiRoot;
        private VisualElement optionsUiBg;
        private QuickEye.UIToolkit.TabGroup optionsTabGroup;
        private QuickEye.UIToolkit.Tab inventoryTab;
        private QuickEye.UIToolkit.Tab skillsTab;
        private QuickEye.UIToolkit.Tab objectivesTab;
        private QuickEye.UIToolkit.Tab achievementsTab;
        private QuickEye.UIToolkit.Tab settingsTab;
        private TemplateContainer inventoryMenuUi;
        private VisualElement skiillsMenuUi;
        private VisualElement objectiveMenuUi;
        private VisualElement achievementsMenuUi;
        private VisualElement settingsMenuUi;
        private TemplateContainer exitButtonUXML;
    
        protected void AssignQueryResults(VisualElement root)
        {
            optionsUiRoot = root.Q<VisualElement>("OptionsUiRoot");
            optionsUiBg = root.Q<VisualElement>("OptionsUiBg");
            optionsTabGroup = root.Q<QuickEye.UIToolkit.TabGroup>("OptionsTabGroup");
            inventoryTab = root.Q<QuickEye.UIToolkit.Tab>("InventoryTab");
            skillsTab = root.Q<QuickEye.UIToolkit.Tab>("SkillsTab");
            objectivesTab = root.Q<QuickEye.UIToolkit.Tab>("ObjectivesTab");
            achievementsTab = root.Q<QuickEye.UIToolkit.Tab>("AchievementsTab");
            settingsTab = root.Q<QuickEye.UIToolkit.Tab>("SettingsTab");
            inventoryMenuUi = root.Q<TemplateContainer>("InventoryMenuUi");
            skiillsMenuUi = root.Q<VisualElement>("SkiillsMenuUi");
            objectiveMenuUi = root.Q<VisualElement>("ObjectiveMenuUi");
            achievementsMenuUi = root.Q<VisualElement>("AchievementsMenuUi");
            settingsMenuUi = root.Q<VisualElement>("SettingsMenuUi");
            exitButtonUXML = root.Q<TemplateContainer>("ExitButtonUXML");
        }
    }
}
