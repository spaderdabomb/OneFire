using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using OneFireUI;

namespace OneFireUi
{
    public partial class GhostIcon : IUiToolkitElement
    {
        public VisualElement root;
        public Vector2 StartDragPosition { get; private set; } = Vector2.zero;
        public GhostIcon(VisualElement root)
        {
            this.root = root;
            AssignQueryResults(root);
            InitLayout();
            RegisterCallbacks();
        }

        public void InitLayout()
        {
            UiManager.Instance.optionsMenuUi.root.Add(root);

            root.style.display = DisplayStyle.None;
            root.style.position = Position.Absolute;
        }

        public void OnGeometryChanged(GeometryChangedEvent evt)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterCallbacks()
        {
            root.RegisterCallback<PointerMoveEvent>(InventoryManager.Instance.MoveDragHandler);
            root.RegisterCallback<PointerUpEvent>(InventoryManager.Instance.EndDragHandler);
        }

        public void UnregisterCallbacks()
        {
            root.UnregisterCallback<PointerMoveEvent>(InventoryManager.Instance.MoveDragHandler);
            root.UnregisterCallback<PointerUpEvent>(InventoryManager.Instance.EndDragHandler);
        }

        public void SetGhostIconLabel(string label)
        {
            ghostIconLabel.text = label;
        }

        public void ShowGhostIcon(PointerDownEvent evt, Texture2D bgTexture, int stackCount)
        {
            root.style.position = Position.Absolute;
            root.style.display = DisplayStyle.Flex;
            ghostIcon.style.backgroundImage = bgTexture;
            SetGhostIconLabel(stackCount.ToString());

            float positionLeft = root.WorldToLocal(evt.position).x - root.resolvedStyle.width / 2;
            float positionTop = root.WorldToLocal(evt.position).y - root.resolvedStyle.height / 2;
            root.style.left = positionLeft;
            root.style.top = positionTop;
            StartDragPosition = new Vector2(positionLeft, positionTop);

            root.CapturePointer(evt.pointerId);
        }

        public void HideGhostIcon()
        {
            root.style.left = 0f;
            root.style.top = 0f;
            root.style.display = DisplayStyle.None;
        }
    }
}
