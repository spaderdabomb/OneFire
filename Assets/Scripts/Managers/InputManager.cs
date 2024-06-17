using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetGameSceneControls()
    {
        PlayerInputManager.Instance.EnableControls();
    }

    public void SetMenuControls()
    {
        PlayerInputManager.Instance.DisableControls();
    }
}
