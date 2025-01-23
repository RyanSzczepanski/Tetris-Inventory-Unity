using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ItemTags
{
    Basic = 0,
    Inventory = 1 << 0,
    Equipable = 1 << 1,
    Option3 = 1 << 2,
}

public static class ItemTagsUtils
{
    public static Type[] TagsToTypes(ItemTags tags)
    {
        List<Type> types = new List<Type>();
        if ((ItemTags.Inventory & tags) != 0) { types.Add(typeof(IInventorySO)); }
        return types.ToArray();
    }
    public static ItemTags TypesToTags(Type type)
    {
        ItemTags tags = ItemTags.Basic;
        if (type.BaseType != typeof(ItemBaseSO)) { Debug.LogWarning($"Type: {type} does not derive from ItemBaseSO"); }
        foreach (Type interfaceType in type.GetInterfaces())
        {
            tags |= (ItemTags)interfaceType.GetField("TAG")?.GetRawConstantValue();
        }
        return tags;
    }
    public static ItemTags TypesToTags(Type[] types)
    {
        ItemTags tags = ItemTags.Basic;
        foreach (Type type in types)
        {
            if(type.BaseType != typeof(ItemBaseSO)) { Debug.LogWarning($"Type: {type} does not derive from ItemBaseSO"); continue; }
            foreach (Type interfaceType in (type.GetInterfaces()))
            {
                tags |= (ItemTags)interfaceType.GetField("TAG")?.GetRawConstantValue();
            }
        }
        return tags;
    }
    public static string[] TagsToStringArr(ItemTags tags)
    {
        return tags.ToString().Split(", ");
    }
}