using System;
using UnityEngine;

[System.Serializable]
public class ItemBase
{
    [field: SerializeField]
    public ItemBaseSO Data { get; private set; }

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

    public override string ToString()
    {
        return $"{Data.FullName}\n   Type: {GetType()}\n   Size: {Data.Size}";
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
