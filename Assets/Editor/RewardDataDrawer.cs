#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using RewardSystem;

[CustomPropertyDrawer(typeof(BaseRewardData), true)]
public class RewardDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        // Show type selector at the top
        var typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        ShowTypeSelector(typeRect, property);
        
        // Show the actual object fields below
        var objectRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 5, 
                                position.width, position.height - EditorGUIUtility.singleLineHeight - 5);
        
        if (property.managedReferenceValue != null)
        {
            EditorGUI.PropertyField(objectRect, property, new GUIContent("Reward Data"), true);
        }
        
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + 5; // Type selector height
        
        if (property.managedReferenceValue != null)
        {
            height += EditorGUI.GetPropertyHeight(property, true);
        }
        
        return height;
    }
    
    private void ShowTypeSelector(Rect position, SerializedProperty property)
    {
        var typeNames = new string[] { "None", "Item", "Stat Bonus", "Recipe", "Currency" };
        var currentIndex = GetCurrentTypeIndex(property);
        
        EditorGUI.BeginChangeCheck();
        var newIndex = EditorGUI.Popup(position, "Reward Type", currentIndex, typeNames);
        
        if (EditorGUI.EndChangeCheck() && newIndex != currentIndex)
        {
            CreateNewRewardData(property, newIndex);
        }
    }
    
    private int GetCurrentTypeIndex(SerializedProperty property)
    {
        if (property.managedReferenceValue == null) return 0;
        
        return property.managedReferenceValue.GetType().Name switch
        {
            nameof(ItemRewardData) => 1,
            nameof(StatBonusRewardData) => 2,
            nameof(RecipeRewardData) => 3,
            nameof(CurrencyRewardData) => 4,
            _ => 0
        };
    }
    
    private void CreateNewRewardData(SerializedProperty property, int typeIndex)
    {
        BaseRewardData newReward = typeIndex switch
        {
            1 => new ItemRewardData(),
            2 => new StatBonusRewardData(),
            3 => new RecipeRewardData(),
            4 => new CurrencyRewardData(),
            _ => null
        };
        
        property.managedReferenceValue = newReward;
        property.serializedObject.ApplyModifiedProperties();
    }
}
#endif