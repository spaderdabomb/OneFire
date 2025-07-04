using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "OneFire/Recipes/RecipeData")]
public class RecipeData : SerializedScriptableObject
{
    [Header("Details")]
    public string id = Guid.NewGuid().ToString();

    [Header("Classification")]
    public ItemData itemResult;
    public float timeToCraft;
    public Dictionary<ItemData, int> recipe;

    private void OnValidate()
    {
        if (id == null || id == String.Empty)
            id = Guid.NewGuid().ToString();

#if UNITY_EDITOR
        if (itemResult == null)
            Debug.Log($"{this} is missing itemResult - assign in inspector");
#endif
    }
}

public static class RecipeExtensions
{
    private static readonly Dictionary<string, RecipeData> _recipes = new Dictionary<string, RecipeData>();
    public static RecipeData GetRecipeData(string uniqueID)
    {
        if (_recipes.Count <= 0)
        {
            var recipes = Resources.LoadAll<RecipeData>("ScriptableObjects/Recipes");
            foreach (var item in recipes)
            {
                _recipes.Add(item.id, item);
            }
        }

        if (_recipes.TryGetValue(uniqueID, out RecipeData recipeData))
        {
            return recipeData;
        }

        Debug.LogWarning($"Recipe not found: {uniqueID}");
        return null;
    }

    public static RecipeData GetRecipeDataInstantiated(this RecipeData recipeData)
    {
        RecipeData originalRecipeData = GetRecipeData(recipeData.id);
        if (originalRecipeData == null)
            return null;

        RecipeData spawnedRecipeData = ScriptableObject.Instantiate(originalRecipeData);
        return spawnedRecipeData;
    }

    public static RecipeData CloneRecipeData(this RecipeData data)
    {
        RecipeData spawnedData = ScriptableObject.Instantiate(data);
        return spawnedData;
    }
}