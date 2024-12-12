using System;
using System.Collections.Generic;
using Szczepanski.UI;
using UnityEngine;

public interface IInventory
{
    public Inventory Inventory { get; }
    public const ItemTags TAGS = ItemTags.Inventory;
    public static GameObject OpenUI(ItemBase item, Transform windowParent)
    {
        if (item is not IInventory) { throw new System.ArgumentException("Input item does not implement IInventory interface"); }
        if (item.Data is not IInventorySO) { throw new System.ArgumentException("Input item data does not implement IInventorySO interface"); }

        FloatingWindowSettings settings = new FloatingWindowSettings()
        {
            isDraggable = true,
            isResizeable = false,
            minWindowSize = new Vector2(0, 0),
            title = item.Data.ShortName
        };

        GameObject floatingWindow = FloatingWindowFactory.CreateFloatingWindow(windowParent, settings);
        Transform transform = floatingWindow.GetComponent<FloatingWindow>().Content.rectTransform;
        InventoryUIGenerator.GenerateUIObject(transform, item.Data as IInventorySO, item as IInventory, in InventoryUIManager.DRAW_SETTINGS);
        return floatingWindow;
    }
}

public interface IEquipable
{
    public const ItemTags TAGS = ItemTags.Equipable;
}

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
    public static Type[] TagsToTypes(ItemTags tags, bool asScriptableObject)
    {
        List<Type> types = new List<Type>();
        if (asScriptableObject)
        {
            if ((ItemTags.Inventory & tags) != 0) { types.Add(typeof(IInventorySO)); }
            if ((ItemTags.Equipable & tags) != 0) { types.Add(typeof(IEquipableSO)); }
        }
        else
        {
            if ((ItemTags.Inventory & tags) != 0) { types.Add(typeof(IInventory)); }
            if ((ItemTags.Equipable & tags) != 0) { types.Add(typeof(IEquipable)); }
        }
        
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