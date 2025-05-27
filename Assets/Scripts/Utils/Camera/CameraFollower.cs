using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Camera targetCamera;

    void Update()
    {
        if (targetCamera != null)
        {
            transform.position = targetCamera.transform.position;
            transform.rotation = targetCamera.transform.rotation;
        }
    }
}
