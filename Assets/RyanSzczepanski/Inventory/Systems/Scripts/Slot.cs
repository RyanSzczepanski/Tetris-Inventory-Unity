using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot
{
    public SubInventory ParentSubInventory { get; }
    public ItemBase ItemInSlot { get; private set; }
    public bool IsOccupied { get => ItemInSlot != null; }

    public Slot(SubInventory parentSubInventory)
    {
        ParentSubInventory = parentSubInventory;
    }

    public void InsertItem(ItemBase InsertedItem)
    {
        ItemInSlot = InsertedItem;
    }
    public void RemoveItem()
    {
        ItemInSlot = null;
    }
}
