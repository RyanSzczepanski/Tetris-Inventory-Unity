using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
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
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Slots[x,y] = new Slot(this);
            }
        }
        this.ParentInventory = parentInventory;
    }

    public void OnItemAdded(Item item, Vector2Int originCellCoordinate)
    {
        if (ItemAdded is null) { return; }
        ItemAdded(this, new SubInventoryItemEventArgs
        {
            Item = item,
            OriginCellCoordinate = originCellCoordinate
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

    private bool BoundsCheck(Item item, Vector2Int originCellCoordinate)
    {
        return (
            originCellCoordinate.x + item.Size.x <= Size.x &&
            originCellCoordinate.x >= 0 &&
            originCellCoordinate.y + item.Size.y <= Size.y &&
            originCellCoordinate.y >= 0
        );
    }
    //TODO: Add Slot Occupied Check
    public bool CanAddItem(Item item, Vector2Int position)
    {
        if (BoundsCheck(item, new Vector2Int(position.x, position.y))) { return true; }
        return false;
    }
    public bool CanAddItem(Item item)
    {
        Vector2Int currentCoordinate = new Vector2Int();
        for (currentCoordinate.y = 0; currentCoordinate.y < Size.y; currentCoordinate.y++)
        {
            for (currentCoordinate.x = 0; currentCoordinate.x < Size.y; currentCoordinate.x++)
            {
                if(CanAddItem(item, currentCoordinate)) { return true; }
            }
        }
        return false;
    }
    public bool CanAddItem(Item item, out Vector2Int validCoordinate)
    {
        validCoordinate = new Vector2Int();
        for (validCoordinate.y = 0; validCoordinate.y < Size.y; validCoordinate.y++)
        {
            for (validCoordinate.x = 0; validCoordinate.x < Size.y; validCoordinate.x++)
            {
                if (CanAddItem(item, validCoordinate)) { return true; }
            }
        }
        return false;
    }
    private void AddItem(Item item, Vector2Int position)
    {
        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                Slots[position.x + x, position.y + y].InsertItem(item);
            }
        }
        OnItemAdded(item, position);
    }
    public bool TryAddItem(Item item)
    {
        if (!CanAddItem(item, out Vector2Int position)) { return false; }
        AddItem(item, position);
        return true;
    }
    public bool TryAddItem(Item item, Vector2Int position)
    {
        if(!CanAddItem(item, position)) { return false; }
        AddItem(item, position);
        return true;
    }
    //Void AddItem(Item, TargetSlotCoordinate)
    //Void RemoveItem(Item)
    //Void MoveItem(Item, TargetSubInventory, TargetSlotCoordinate)
    //Item GetItemInSlot(TargetSlotCoordinate)
}
