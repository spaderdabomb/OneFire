using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageableObject : SerializedMonoBehaviour
{
    public HealthBar HealthBar { get; private set; } = null;
    private static float timeToHealthDisappear = 1f;
    [SerializeField] private GameObject healthBarPosition;
    [SerializeField] public IDamageable damageable;

    private float _timeRemainingHealthDisappear = 0f;

    private void Update()
    {
        if (HealthBar == null)
            return;

        HealthBar.SetPositionToObj();
        UpdateHidden();
    }

    private void UpdateHidden()
    {
        if (GameObjectManager.Instance.playerInteract.damageableObjects.Contains(gameObject))
        {
            _timeRemainingHealthDisappear = timeToHealthDisappear;
            HealthBar.Show();
        }
        else
        {
            _timeRemainingHealthDisappear -= Time.deltaTime;
            if (_timeRemainingHealthDisappear <= 0f)
            {
                HealthBar.Hide();
            }
        }
    }

    public virtual void Damage()
    {
        if (!GameObjectManager.Instance.playerInteract.damageableObjects.Contains(gameObject))
            return;

        float damage = GameObjectManager.Instance.playerStats.CalculateDamage(gameObject);
        if (HealthBar == null)
            MakeHealthBar();

        HealthBar.SubtractHealth(damage);
        if (HealthBar.CurrentHealth <= 0)
        {
            if (GetComponent<IRespawnable>() != null)
                DeactivateObject();
            else
                DestroyObject();
        }
    }

    public virtual void DeactivateObject()
    {
        GameObjectManager.Instance.playerInteract.damageableObjects.Remove(gameObject);
        HealthBar.root.parent.Remove(HealthBar.root);
        HealthBar = null;
        gameObject.SetActive(false);
    }

    public virtual void DestroyObject()
    {
        GameObjectManager.Instance.playerInteract.damageableObjects.Remove(gameObject);
        HealthBar.root.parent.Remove(HealthBar.root);
        HealthBar = null;
        Destroy(gameObject);
    }

    private void MakeHealthBar()
    {
        VisualElement healthBarInst = UiManager.Instance.healthBar.Instantiate();
        HealthBar = new HealthBar(healthBarInst, healthBarPosition, damageable.BaseHealth);
        UiManager.Instance.uiGameManager.root.Add(healthBarInst);
    }
}

public interface IDamageable
{
    float BaseHealth { get; }
}

public interface IRespawnable
{
    float RespawnTime { get;}
}
