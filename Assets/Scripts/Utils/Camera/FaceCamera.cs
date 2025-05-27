using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.LookAt(GameObjectManager.Instance.playerCamera.transform);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        }
    }
}
