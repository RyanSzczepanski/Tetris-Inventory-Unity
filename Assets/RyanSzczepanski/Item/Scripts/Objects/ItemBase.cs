using System;
using Szczepanski.UI;
using UnityEngine;

[System.Serializable]
public class ItemBase
{
    [field: SerializeField]
    public ItemBaseSO Data { get; private set; }

    public SubInventory ParentSubInventory { get; private set; }
    public bool IsRotated { get; private set; }

    public delegate void ItemRemovedHandler(object source, ItemRemovedEventArgs args);
    public delegate void ItemRotatedHandler(object source, bool itemRotated);
    public event ItemRemovedHandler ItemRemoved;
    public event ItemRotatedHandler ItemRotated;

    public ItemBase(ItemBaseSO data)
    {
        Data = data;
    }

    public void OnItemAdded(object source, ItemAddedEventArgs args)
    {
        Rotate(args.IsRotated);
        ParentSubInventory = args.SubInventory;
    }
    public void OnItemRemoved(object source, ItemRemovedEventArgs args)
    {
        //if (args.Item != this) { return; }
        ItemRemoved?.Invoke(this, new ItemRemovedEventArgs {
            Item = this,
            SubInventory = args.SubInventory,
        });
    }
    private void Rotate()
    {
        IsRotated = !IsRotated;
        ItemRotated?.Invoke(this, IsRotated);
    }
    private void Rotate(bool rotated)
    {
        IsRotated = rotated;
        ItemRotated?.Invoke(this, IsRotated);
    }

    public ContextMenuOption[] ContextMenuOptions()
    {
        return new ContextMenuOption[2]
        {
            new ContextMenuOption { optionText = "Inspect", OnSelected = () => throw new System.NotImplementedException("Inspect Item Not Implemented")},
            new ContextMenuOption { optionText = "Discard", OnSelected = () => ParentSubInventory.TryRemoveItem(this)},

        };
    }

    public override string ToString()
    {
        return $"{Data.FullName}\n   Type: {GetType()}\n   Size: {Data.Size}";
    }

    public void Subscribe(SubInventory subInventory)
    {
        Debug.Log("DEPRICATE");
        //subInventory.ItemRemoved += OnItemRemoved;
    }
    public void Unsubscribe(SubInventory subInventory)
    {
        Debug.Log("DEPRICATE");
        //subInventory.ItemRemoved -= OnItemRemoved;
    }
}
