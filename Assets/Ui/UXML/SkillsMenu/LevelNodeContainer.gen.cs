// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class LevelNodeContainer
{
    private VisualElement levelNodeContainer;
    private Button levelNode;
    private Label levelNodeLabel;
    private VisualElement levelPath;
    
    protected void AssignQueryResults(VisualElement root)
    {
        levelNodeContainer = root.Q<VisualElement>("LevelNodeContainer");
        levelNode = root.Q<Button>("LevelNode");
        levelNodeLabel = root.Q<Label>("LevelNodeLabel");
        levelPath = root.Q<VisualElement>("LevelPath");
    }
}
