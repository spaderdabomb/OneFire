using UnityEngine.UIElements;
using UnityEngine;
using System;

public partial class HealthBar
{
    public VisualElement root;
    public float TotalHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public bool Hidden { get; private set; }
    public GameObject parentObj;
    public HealthBar(VisualElement root, GameObject parentObj, float health)
    {
        this.root = root;
        this.parentObj = parentObj;
        TotalHealth = health;
        CurrentHealth = health;

        AssignQueryResults(root);
        RegisterCallbacks();
        Init();
    }

    private void Init()
    {
        Hidden = false;
        UpdateUi();
    }

    public void RegisterCallbacks()
    {
        root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    public void UnregisterCallbacks()
    {
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        SetPositionToObj();
    }

    public void SetPositionToObj()
    {
        root.style.position = Position.Absolute;

        Vector3 worldPosition = parentObj.transform.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        screenPosition.y = Screen.height - screenPosition.y;

        root.style.left = screenPosition.x;
        root.style.top = screenPosition.y;
    }

    public void UpdateUi()
    {
        healthBarRoot.value = 100f * (CurrentHealth / TotalHealth);
        healthBarRoot.title = CurrentHealth.ToString() + "/" + TotalHealth;
    }


    public void SetHealth(float newHealth)
    {
        CurrentHealth = newHealth;
    }

    public void AddHealth(float deltaHealth)
    {

    }

    public void SubtractHealth(float deltaHealth)
    {
        CurrentHealth -= deltaHealth;
        UpdateUi();
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
