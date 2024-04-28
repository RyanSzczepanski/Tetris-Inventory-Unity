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
    public delegate void ItemAddedHandler(object source, SubInventoryItemAddedEventArgs args);
    public delegate void ItemMovedHandler(object source, SubInventoryItemMovedEventArgs args);
    public delegate void ItemRemovedHandler(object source, SubInventoryItemRemovedEventArgs args);
    //Events
    public event ItemAddedHandler ItemAdded;
    public event ItemMovedHandler ItemMoved;
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

    private void OnItemAdded(Item item, ItemData targetData)
    {
        if (ItemAdded is null) { return; }
        ItemAdded(this, new SubInventoryItemAddedEventArgs
        {
            Item = item,
            TargetCellCoordinate = targetData.gridCoordinate,
            TargetIsRotated = targetData.isRotated,
            TargetSubInventory = targetData.subInventory,
        });
    }
    private void OnItemMoved(Item item, ItemData originData, ItemData targetData)
    {
        if (ItemMoved is null) { return; }
        ItemMoved(this, new SubInventoryItemMovedEventArgs
        {
            Item = item,

            OriginCellCoordinate = originData.gridCoordinate,
            OriginIsRotated = originData.isRotated,
            OriginSubInventory = originData.subInventory,

            TargetCellCoordinate = targetData.gridCoordinate,
            TargetIsRotated = targetData.isRotated,
            TargetSubInventory = targetData.subInventory,
        });
    }
    private void OnItemRemoved(Item item)
    {
        if (ItemRemoved is null) { return; }
        ItemRemoved(this, new SubInventoryItemRemovedEventArgs
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
    private bool SlotsOccupiedCheck(Item item, Vector2Int originCellCoordinate)
    {
        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                if (Slots[originCellCoordinate.x+x, originCellCoordinate.y+y].IsOccupied)
                {
                    Debug.LogWarning("Failed Slots Occupied Check");
                    return false;
                }
            }
        }
        return true;
    }
    //TODO: Add Slot Occupied Check
    public bool CanAddItem(Item item, Vector2Int targetCoordinate)
    {
        if (!BoundsCheck(item, targetCoordinate)) { return false; }
        if (!SlotsOccupiedCheck(item, targetCoordinate)) { return false; }
        return true;
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
    private void AddItem(Item item, Vector2Int targetCoordinate)
    {
        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                Slots[targetCoordinate.x + x, targetCoordinate.y + y].InsertItem(item);
            }
        }
        item.Subscribe(this);
        OnItemAdded(item, new ItemData
        {
            subInventory = this,
            gridCoordinate = targetCoordinate,
            isRotated = item.IsRotated
        });
    }
    public bool TryAddItem(Item item)
    {
        if (!CanAddItem(item, out Vector2Int position)) { return false; }
        AddItem(item, position);
        return true;
    }
    public bool TryAddItem(Item item, Vector2Int targetCoordinate)
    {
        if(!CanAddItem(item, targetCoordinate)) { return false; }
        AddItem(item, targetCoordinate);
        return true;
    }

    private void RemoveItem(Item item)
    {
        for (int y = 0; y < Slots.GetLength(1); y++)
        {
            for (int x = 0; x < Slots.GetLength(0); x++)
            {
                if (Slots[x, y].ItemInSlot == item)
                {
                    Slots[x, y].RemoveItem();
                }
            }
        }
        OnItemRemoved(item);
    }
    public bool TryRemoveItem(Item item)
    {
        RemoveItem(item);
        return true;
    }

    public bool TryMoveItem(Item item, SubInventory targetSubInventory, Vector2Int targetCoordinate, Vector2Int originCoordinate, bool originRotatedStatus)
    {
        RemoveItem(item);
        if (!targetSubInventory.TryAddItem(item, targetCoordinate)) {
            item.SetRotate(originRotatedStatus);
            TryAddItem(item, originCoordinate);
            return false;
        }
        return true;
    }
}
