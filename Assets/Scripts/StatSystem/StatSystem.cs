using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private float currentValue;
    private List<StatModifier> modifiers = new List<StatModifier>();
    
    public float BaseValue => baseValue;
    public float CurrentValue => isDirty ? CalculateValue() : currentValue;
    
    private bool isDirty = true;
    
    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        this.currentValue = baseValue;
    }
    
    public void SetBaseValue(float value)
    {
        baseValue = value;
        isDirty = true;
    }
    
    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        modifiers.Sort((a, b) => a.Order.CompareTo(b.Order));
        isDirty = true;
    }
    
    public void RemoveModifier(StatModifier modifier)
    {
        modifiers.Remove(modifier);
        isDirty = true;
    }
    
    private float CalculateValue()
    {
        float finalValue = baseValue;
        float percentAdd = 0;
        
        foreach (var modifier in modifiers)
        {
            switch (modifier.Type)
            {
                case StatModType.Flat:
                    finalValue += modifier.Value;
                    break;
                case StatModType.PercentAdd:
                    percentAdd += modifier.Value;
                    break;
                case StatModType.PercentMult:
                    finalValue *= (1 + modifier.Value);
                    break;
            }
        }
        
        finalValue *= (1 + percentAdd);
        currentValue = finalValue;
        isDirty = false;
        
        return finalValue;
    }
}

[System.Serializable]
public class StatModifier
{
    public float Value { get; private set; }
    public StatModType Type { get; private set; }
    public int Order { get; private set; }
    public object Source { get; private set; }
    
    public StatModifier(float value, StatModType type, int order = 0, object source = null)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }
}

public enum StatModType
{
    Flat = 100,      // +5 damage
    PercentAdd = 200, // +10% damage (additive)
    PercentMult = 300 // +10% damage (multiplicative)
}