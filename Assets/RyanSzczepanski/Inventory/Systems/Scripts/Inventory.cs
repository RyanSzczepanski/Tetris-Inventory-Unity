using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class Inventory
{
    //Inventory Refrences
    public ItemBase ParentItem { get; }
    public SubInventory[] SubInventories { get; }

    public Inventory(ItemBase item)
    {
        ParentItem = item;
        IInventorySO inventoryData = (item.Data as IInventorySO);
        SubInventories = new SubInventory[inventoryData.SubInventories.Length];
        for (int i = 0; i < SubInventories.Length; i++)
        {
            SubInventories[i] =  new SubInventory(inventoryData.SubInventories[i], this);
        }
    }

    public bool TryAddItem(ItemBase itemBase)
    {
        foreach(var subInventory in SubInventories)
        {
            if (subInventory.TryAddItem(itemBase)) { return true; }
        }

        return false;
    }
    //RemoveItem(Item)
}
