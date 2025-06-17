using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace NS.RomanLib
{
    [UxmlElement]
    public partial class RadialFillElement : VisualElement, INotifyValueChanged<float>
    {
        protected float m_value = float.NaN;

        [UxmlAttribute] public float width { get; set; } = 20f;
        [UxmlAttribute] public float height { get; set; } = 20f;
        [UxmlAttribute] public Color fillColor { get; set; } = Color.white;
        [UxmlAttribute] public float angleOffset { get; set; } = 0f;
        [UxmlAttribute] public string overlayImagePath { get; set; } = "";
        [UxmlAttribute] public float overlayImageScale { get; set; } = 1f;
        [UxmlAttribute] public FillDirection fillDirection { get; set; } = FillDirection.Clockwise;
        [UxmlAttribute] public float value
        {
            get => Mathf.Clamp(m_value, 0, 1);
            set
            {
                if (Mathf.Approximately(m_value, value)) return;
                if (panel != null)
                {
                    using var pooled = ChangeEvent<float>.GetPooled(m_value, value);
                    pooled.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(pooled);
                }
                else
                {
                    SetValueWithoutNotify(value);
                }
            }
        }

        public void SetValueWithoutNotify(float newValue)
        {
            m_value = newValue;
            radialFill.MarkDirtyRepaint();
        }

        public enum FillDirection
        {
            Clockwise,
            AntiClockwise
        }

        private float radius => Mathf.Min(width, height) / 2;

        public VisualElement radialFill;
        public VisualElement overlayImage;

        public RadialFillElement()
        {
            name = "radial-fill-element";
            radialFill = new VisualElement { name = "radial-fill" };
            overlayImage = new VisualElement { name = "overlay-image" };
            radialFill.generateVisualContent += OnGenerateVisualContent;

            Clear();
            VisualElement radialBoundary = new VisualElement { name = "radial-boundary" };
            radialBoundary.style.height = height;
            radialBoundary.style.width = width;
            radialBoundary.style.overflow = Overflow.Hidden;

            radialFill.style.flexGrow = 1;
            overlayImage.style.flexGrow = 1;
            overlayImage.style.scale = new Scale(new Vector2(overlayImageScale, overlayImageScale));
            overlayImage.style.backgroundImage = null;

#if UNITY_EDITOR
            var tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(overlayImagePath);
            if (tex != null)
                overlayImage.style.backgroundImage = tex;
#endif

            radialFill.Add(overlayImage);
            radialFill.transform.rotation = Quaternion.Euler(0, 0, angleOffset);
            overlayImage.transform.rotation = Quaternion.Euler(0, 0, -angleOffset);
            radialBoundary.Add(radialFill);
            Add(radialBoundary);
            style.height = height;
            style.width = width;
        }

        public void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            float clampedValue = Mathf.Clamp(m_value, 0, 1);
            int triCount = clampedValue * 360 < 120 ? 3 : (clampedValue * 360 < 240 ? 4 : 5);
            int indiceCount = triCount == 3 ? 3 : (triCount == 4 ? 6 : 9);

            MeshWriteData mwd = mgc.Allocate(triCount, indiceCount);
            Vector3 origin = new Vector3(width / 2, height / 2, 0);
            float diameter = 4 * radius;
            float degrees = ((clampedValue * 360) - 90) / Mathf.Rad2Deg;
            float direction = fillDirection == FillDirection.AntiClockwise ? -1f : 1f;

            mwd.SetNextVertex(new Vertex { position = origin, tint = fillColor });
            mwd.SetNextVertex(new Vertex { position = origin + new Vector3(0, -diameter, Vertex.nearZ), tint = fillColor });

            mwd.SetNextIndex(0);
            mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)2 : (ushort)1);

            if (clampedValue * 360 <= 120)
            {
                mwd.SetNextVertex(new Vertex { position = origin + new Vector3(Mathf.Cos(degrees) * diameter * direction, Mathf.Sin(degrees) * diameter, Vertex.nearZ), tint = fillColor });
                mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)1 : (ushort)2);
            }
            else if (clampedValue * 360 <= 240)
            {
                mwd.SetNextVertex(new Vertex { position = origin + new Vector3(Mathf.Cos(30 / Mathf.Rad2Deg) * diameter * direction, Mathf.Sin(30 / Mathf.Rad2Deg) * diameter, Vertex.nearZ), tint = fillColor });
                mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)1 : (ushort)2);
                mwd.SetNextVertex(new Vertex { position = origin + new Vector3(Mathf.Cos(degrees) * diameter * direction, Mathf.Sin(degrees) * diameter, Vertex.nearZ), tint = fillColor });
                mwd.SetNextIndex(0);
                mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)3 : (ushort)2);
                mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)2 : (ushort)3);
            }
            else
            {
                mwd.SetNextVertex(new Vertex { position = origin + new Vector3(Mathf.Cos(30 / Mathf.Rad2Deg) * diameter * direction, Mathf.Sin(30 / Mathf.Rad2Deg) * diameter, Vertex.nearZ), tint = fillColor });
                mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)1 : (ushort)2);
                mwd.SetNextVertex(new Vertex { position = origin + new Vector3(Mathf.Cos(150 / Mathf.Rad2Deg) * diameter * direction, Mathf.Sin(150 / Mathf.Rad2Deg) * diameter, Vertex.nearZ), tint = fillColor });
                mwd.SetNextIndex(0);
                mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)3 : (ushort)2);
                mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)2 : (ushort)3);

                if (clampedValue * 360 >= 360)
                {
                    mwd.SetNextIndex(0);
                    mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)1 : (ushort)3);
                    mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)3 : (ushort)1);
                }
                else
                {
                    mwd.SetNextVertex(new Vertex { position = origin + new Vector3(Mathf.Cos(degrees) * diameter * direction, Mathf.Sin(degrees) * diameter, Vertex.nearZ), tint = fillColor });
                    mwd.SetNextIndex(0);
                    mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)4 : (ushort)3);
                    mwd.SetNextIndex(fillDirection == FillDirection.AntiClockwise ? (ushort)3 : (ushort)4);
                }
            }
        }
    }
}
