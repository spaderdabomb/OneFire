using UnityEngine.UIElements;
using UnityEngine;

public partial class StructurePlacementMessage : MonoBehaviour
{
    public VisualElement root;
    private float elapsedTime = 0.0f;

    [SerializeField] private float maxOpacity = 1.0f;
    [SerializeField] private float minOpacity = 0.25f;
    [SerializeField] private float animationDuration = 1.0f;

    public void Init(VisualElement root)
    {
        AssignQueryResults(root);
        this.root = root;
    }

    private void Update()
    {
        UpdateBackgroundOpacity();
    }

    public void UpdateBackgroundOpacity()
    {
        elapsedTime += Time.deltaTime;
        float time = Mathf.PingPong(elapsedTime / animationDuration, 1.0f);
        float targetOpacity = Mathf.Lerp(minOpacity, maxOpacity, time);
        root.style.opacity = targetOpacity;
    }

    public void Show()
    {
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }
}
