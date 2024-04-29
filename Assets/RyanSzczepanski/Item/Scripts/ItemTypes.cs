using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ItemBasic
{
    public ItemBasicSO Data { get; private set; }

    public Vector2Int Size { get => new Vector2Int(
        ((1 - Convert.ToInt32(IsRotated)) * Data.size.x) + (Convert.ToInt32(IsRotated) * (Data.size.x * Data.size.y / Data.size.x)),
        ((1 - Convert.ToInt32(IsRotated)) * Data.size.y) + (Convert.ToInt32(IsRotated) * (Data.size.x * Data.size.y / Data.size.y))
    ); }
    public bool IsRotated { get; private set; }

    //Delegates
    public delegate void ItemRemovedHandler(object source, ItemRemovedEventArgs args);
    //Events
    public event ItemRemovedHandler ItemRemoved;
    //Constructor
    public ItemBasic(ItemBasicSO data)
    {
        Data = data;
    }

    public void OnItemRemoved(object source, SubInventoryItemRemovedEventArgs args)
    {
        if (args.Item == this)
        {
            if (ItemRemoved != null)
            {
                ItemRemoved(this, new ItemRemovedEventArgs { Item = this });
            }
        }
    }

    public void Subscribe(SubInventory subInventory)
    {
        subInventory.ItemRemoved += OnItemRemoved;
    }
    public void Unsubscribe(SubInventory subInventory)
    {
        subInventory.ItemRemoved -= OnItemRemoved;
    }

    public void Rotate()
    {
        IsRotated = !IsRotated;
    }
    public void SetRotate(bool rotated)
    {
        IsRotated = rotated;
    }
}

public class ItemInventory : ItemBasic
{
    public ItemInventory(ItemInventorySO data) : base(data)
    {

    }
}


public struct ItemData
{
    public bool isRotated;
    public Vector2Int gridCoordinate;
    public SubInventory subInventory;
}
