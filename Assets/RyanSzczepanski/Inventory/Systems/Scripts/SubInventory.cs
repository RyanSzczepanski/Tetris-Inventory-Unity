using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubInventory
{
    //Inventory Refrences
    public Inventory ParentInventory { get; }
    public Slot[,] Slots { get; }
    //Data
    public Vector2Int Size { get => new Vector2Int(Slots.GetLength(0), Slots.GetLength(1)); }
    //Delegates
    public delegate void ItemAddedHandler(object source, SubInventoryItemEventArgs args);
    public delegate void ItemRemovedHandler(object source, SubInventoryItemEventArgs args);
    //Events
    public event ItemAddedHandler ItemAdded;
    public event ItemRemovedHandler ItemRemoved;
    //Constructor
    public SubInventory(Vector2Int size, Inventory parentInventory)
    {
        this.Slots = new Slot[size.x, size.y];
        this.ParentInventory = parentInventory;
    }

    public void OnItemAdded(Item item, Vector2Int originCoordinate)
    {
        if (ItemAdded is null) { return; }
        ItemAdded(this, new SubInventoryItemEventArgs
        {
            Item = item,
            OriginGridCoordinate = originCoordinate
        });
    }
    public void OnItemRemoved(Item item)
    {
        if (ItemRemoved is null) { return; }
        ItemAdded(this, new SubInventoryItemEventArgs
        {
            Item = item,
        });
    }


    //Void AddItem(Item)
    //Void AddItem(Item, TargetSlotCoordinate)
    //Void RemoveItem(Item)
    //Void MoveItem(Item, TargetSubInventory, TargetSlotCoordinate)
    //Item GetItemInSlot(TargetSlotCoordinate)
}
