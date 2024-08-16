using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldTree : MonoBehaviour
{
    private HealthBar _healthBar = null;
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

        if (_healthBar == null)
            MakeHealthBar();
    }

    private void MakeHealthBar()
    {
        VisualElement healthBarInst = UiManager.Instance.healthBar.Instantiate();
        _healthBar = new HealthBar(healthBarInst, gameObject, 100f);
        UiManager.Instance.uiGameManager.root.Add(healthBarInst);
    }
}
