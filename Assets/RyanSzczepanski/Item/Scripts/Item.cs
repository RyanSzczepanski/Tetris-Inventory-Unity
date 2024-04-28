using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Item
{
    private Vector2Int size;
    public Vector2Int Size { get => new Vector2Int(
        ((1 - Convert.ToInt32(IsRotated)) * size.x) + (Convert.ToInt32(IsRotated) * (size.x * size.y / size.x)),
        ((1 - Convert.ToInt32(IsRotated)) * size.y) + (Convert.ToInt32(IsRotated) * (size.x * size.y / size.y))
    ); }
    public bool IsRotated { get; private set; }


    //Delegates
    public delegate void ItemRemovedHandler(object source, ItemRemovedEventArgs args);
    //Events
    public event ItemRemovedHandler ItemRemoved;
    //Constructor
    public Item(Vector2Int size)
    {
        this.size = size;
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

public struct ItemData
{
    public bool isRotated;
    public Vector2Int gridCoordinate;
    public SubInventory subInventory;
}
