using UnityEditor;
using UnityEngine;

namespace Raygeas
{
    [ExecuteInEditMode]

    public class WindSettings : MonoBehaviour
    {
        //Wind variables
        [SerializeField] [Range(0.0F, 5.0F)] private float force = 1.0f;
        [SerializeField] [Range(0.0F, 5.0F)] private float wavesScale = 1.0f;
        [SerializeField] [Range(0.0F, 5.0F)] private float flowDensity = 1.0f;
        private Vector3 windDirection;

        //Arrow variables
        
        private Color arrowColor = Color.green;

        void Update()
        {
            windDirection = transform.forward;

            //Set wind settings
            Shader.SetGlobalFloat("RAYGlobalWindForce", force);
            Shader.SetGlobalFloat("RAYGlobalFlowDensity", flowDensity);
            Shader.SetGlobalFloat("RAYGlobalWavesScale", wavesScale);
            Shader.SetGlobalVector("RAYGlobalDirection", windDirection);
        }

#if UNITY_EDITOR
        //Draw arrow
        void OnDrawGizmos()
        {
            if (Selection.activeGameObject == gameObject)
            {
                float arrowLength = 1.5f;
                float arrowWidth = 3f;
                Handles.color = arrowColor;

                Vector3 arrowEnd = transform.position + transform.forward * arrowLength;

                //Draw arrow body
                Handles.DrawLine(transform.position - transform.forward * arrowLength, arrowEnd);

                // Draw arrowhead
                Handles.DrawAAConvexPolygon(
                    new Vector3[] {
                arrowEnd - transform.up * arrowWidth / 4,
                arrowEnd + transform.forward * arrowWidth,
                arrowEnd + transform.up * arrowWidth / 4
                    }
                );
                Handles.DrawAAConvexPolygon(
                    new Vector3[] {
                arrowEnd - transform.right * arrowWidth / 4,
                arrowEnd + transform.forward * arrowWidth,
                arrowEnd + transform.right * arrowWidth / 4
                    }
                );
            }
        }
#endif
    }
}
