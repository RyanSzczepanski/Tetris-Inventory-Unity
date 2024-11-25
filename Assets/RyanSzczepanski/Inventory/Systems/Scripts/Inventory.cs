using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    //Inventory Refrences
    public SubInventory[] SubInventories { get; }

    public Inventory(IInventorySO inventoryData)
    {
        SubInventories = new SubInventory[inventoryData.SubInventorySizes.Length];
        for (int i = 0; i < inventoryData.SubInventorySizes.Length; i++)
        {
            SubInventories[i] =  new SubInventory(inventoryData.SubInventorySizes[i], this);
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
