using UnityEngine.UIElements;
using UnityEngine;
using System;

public partial class HealthBar
{
    VisualElement root;
    public float CurrentHealth { get; private set; }
    public GameObject parentObj;
    public HealthBar(VisualElement root, GameObject parentObj, float health)
    {
        this.root = root;
        this.parentObj = parentObj;
        CurrentHealth = health;

        AssignQueryResults(root);
        RegisterCallbacks();
        Init();
    }

    private void Init()
    {

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
        root.style.position = Position.Absolute;

        Vector3 worldPosition = parentObj.transform.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        screenPosition.y = Screen.height - screenPosition.y;

        root.style.left = screenPosition.x;
        root.style.top = screenPosition.y;
    }


    public void SetHealth(float newHealth)
    {
        CurrentHealth = newHealth;
    }

    public void AddHealth()
    {

    }

    public void SubtractHealth()
    {

    }
}
