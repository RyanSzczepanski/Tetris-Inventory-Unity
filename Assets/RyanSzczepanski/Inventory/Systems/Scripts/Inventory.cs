using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    //Inventory Refrences
    public SubInventory[] SubInventories { get; }

    //TODO: InventoryData should come from Inventory Interface
    public Inventory(InventoryDataTest inventoryDataTest)
    {
        SubInventories = new SubInventory[inventoryDataTest.subInventories.Length];
        for (int i = 0; i < inventoryDataTest.subInventories.Length; i++)
        {
            SubInventories[i] =  new SubInventory(inventoryDataTest.subInventories[i], this);
        }
    }
    //AddItem(Item)
    //RemoveItem(Item)
}
