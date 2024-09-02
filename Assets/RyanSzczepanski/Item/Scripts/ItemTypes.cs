using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ItemBasic : ItemBase
{
    public new ItemBasicSO Data { get; private set; }
    public ItemBasic(ItemBasicSO data) : base(data)
    {

    }
}

public class ItemInventory : ItemBase, IInventory
{
    public new ItemInventorySO Data { get; private set; }
    public Inventory Inventory { get; private set; }

    public ItemInventory(ItemInventorySO data) : base(data)
    {
        Data = data;
        Inventory = new Inventory(data);
        //SubInventoryUIGenerator uIGenerator = new SubInventoryUIGenerator();
    }

    public override string ToString()
    {
        return $"{Data.FullName}\n   Type: {GetType()}\n   Size: {Data.Size}\n   Storage: {Data.StorageSlots}";
    }
}


public struct ItemData
{
    public bool isRotated;
    public Vector2Int gridCoordinate;
    public SubInventory subInventory;
}
