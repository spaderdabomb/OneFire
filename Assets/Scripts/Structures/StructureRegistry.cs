using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class StructureRegistry
{
    public static Dictionary<string, StructureData> structureDictionary = new Dictionary<string, StructureData>();

    static StructureRegistry()
    {
        structureDictionary.Clear();
    }

    public static bool Register(StructureData structure)
    {
        if (structureDictionary.ContainsKey(structure.id))
        {
            return false; // Duplicate found
        }

        structureDictionary[structure.id] = structure;
        return true;
    }

    public static void Unregister(StructureData structure)
    {
        structureDictionary.Remove(structure.id);
    }

    public static StructureData GetStructureData (string id)
    {
        return structureDictionary[id];
    }
}
