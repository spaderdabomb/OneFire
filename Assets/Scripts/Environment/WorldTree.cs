using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldTree : MonoBehaviour
{
    private void OnEnable()
    {
        InputManager.Instance.playerActionsInput.OnPunchHit += DamageTree;
    }

    private void OnDisable()
    {
        InputManager.Instance.playerActionsInput.OnPunchHit -= DamageTree;
    }

    private void DamageTree()
    {
        if (!GameObjectManager.Instance.playerInteract.interactingObjects.Contains(gameObject))
            return;

        VisualElement healthBarInst = UiManager.Instance.healthBar.Instantiate();
        HealthBar healthBar = new HealthBar(healthBarInst, gameObject, 100f);
        UiManager.Instance.uiGameManager.root.Add(healthBarInst);
    }
}
