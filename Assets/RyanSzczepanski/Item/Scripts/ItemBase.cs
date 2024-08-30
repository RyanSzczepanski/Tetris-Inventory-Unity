using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemBase
{
    [field: SerializeField]
    public ItemBaseSO Data { get; private set; }

    public Vector2Int Size
    {
        get => new Vector2Int(
        ((1 - Convert.ToInt32(IsRotated)) * Data.size.x) + (Convert.ToInt32(IsRotated) * (Data.size.x * Data.size.y / Data.size.x)),
        ((1 - Convert.ToInt32(IsRotated)) * Data.size.y) + (Convert.ToInt32(IsRotated) * (Data.size.x * Data.size.y / Data.size.y)));
    }
    public bool IsRotated { get; private set; }

    public delegate void ItemRemovedHandler(object source, ItemRemovedEventArgs args);
    public delegate void ItemRotatedHandler(object source, bool itemRotated);
    public event ItemRemovedHandler ItemRemoved;
    public event ItemRotatedHandler ItemRotated;

    public ItemBase(ItemBaseSO data)
    {
        Data = data;
    }

    private void OnItemRemoved(object source, SubInventoryItemRemovedEventArgs args)
    {
        if (args.Item != this) { return; }
        ItemRemoved?.Invoke(this, new ItemRemovedEventArgs { Item = this });
    }
    public void Rotate()
    {
        IsRotated = !IsRotated;
        ItemRotated?.Invoke(this, IsRotated);
    }
    public void Rotate(bool rotated)
    {
        IsRotated = rotated;
        ItemRotated?.Invoke(this, IsRotated);
    }

    public void Subscribe(SubInventory subInventory)
    {
        subInventory.ItemRemoved += OnItemRemoved;
    }
    public void Unsubscribe(SubInventory subInventory)
    {
        subInventory.ItemRemoved -= OnItemRemoved;
    }
}
