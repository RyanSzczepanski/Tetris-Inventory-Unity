using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot
{
    public bool IsOccupied { get => ItemInSlot != null; }
    public Item ItemInSlot { get; private set; }

    public void InsertItem(Item InsertedItem)
    {
        ItemInSlot = InsertedItem;
    }
    public void RemoveItem()
    {
        ItemInSlot = null;
    }
}
