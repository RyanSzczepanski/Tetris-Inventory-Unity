using UnityEditor.Experimental;
using UnityEngine;

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

    private void OnItemAdded(ItemBase item, ItemData targetData)
    {
        //if (ItemAdded is null) { return; }
        item.OnItemAdded(this, new ItemAddedEventArgs() {
            Item = item,
            SubInventory = targetData.subInventory,
            IsRotated = targetData.isRotated
        });
        ItemAdded?.Invoke(this, new SubInventoryItemAddedEventArgs
        {
            Item = item,
            TargetCellCoordinate = targetData.gridCoordinate,
            TargetIsRotated = targetData.isRotated,
            TargetSubInventory = targetData.subInventory,
        });
    }
    private void OnItemMoved(ItemBase item, ItemData originData, ItemData targetData)
    {
        //if (ItemMoved is null) { return; }
        ItemMoved?.Invoke(this, new SubInventoryItemMovedEventArgs
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
    private void OnItemRemoved(ItemBase item)
    {

        //if (ItemRemoved is null) { return; }
        ItemRemoved?.Invoke(this, new SubInventoryItemRemovedEventArgs
        {
            Item = item,
            SubInventory = this
        });
        item.OnItemRemoved(this, new ItemRemovedEventArgs {
            Item = item,
            SubInventory = this
        });
    }

    private bool BoundsCheck(ItemBase item, Vector2Int originCellCoordinate, bool isRotated)
    {
        Vector2Int itemSize = isRotated ? new Vector2Int(item.Data.Size.y, item.Data.Size.x) : item.Data.Size;
        return (
            originCellCoordinate.x + itemSize.x <= Size.x &&
            originCellCoordinate.x >= 0 &&
            originCellCoordinate.y + itemSize.y <= Size.y &&
            originCellCoordinate.y >= 0
        );
    }
    private bool SlotsOccupiedCheck(ItemBase item, Vector2Int originCellCoordinate, bool isRotated, bool ignoreSelf = true)
    {
        Vector2Int itemSize = isRotated ? new Vector2Int(item.Data.Size.y, item.Data.Size.x) : item.Data.Size;

        for (int y = 0; y < itemSize.y; y++)
        {
            for (int x = 0; x < itemSize.x; x++)
            {
                if (Slots[originCellCoordinate.x + x, originCellCoordinate.y + y].ItemInSlot == item && ignoreSelf) { continue; }
                if (Slots[originCellCoordinate.x + x, originCellCoordinate.y + y].IsOccupied)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public Vector2Int GetItemOriginSlot(ItemBase item)
    {
        for (int y = 0; y < Slots.GetLength(1); y++)
        {
            for (int x = 0; x < Slots.GetLength(0); x++)
            {
                if (Slots[x, y].ItemInSlot == item)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    public bool CanAddItem(ItemBase item, Vector2Int targetCoordinate, bool isRotated)
    {
        if (!BoundsCheck(item, targetCoordinate, isRotated)) { return false; }
        if (!SlotsOccupiedCheck(item, targetCoordinate, isRotated)) { return false; }
        return true;
    }
    public bool CanAddItem(ItemBase item)
    {
        Vector2Int currentCoordinate = new Vector2Int();
        for (currentCoordinate.y = 0; currentCoordinate.y < Size.y; currentCoordinate.y++)
        {
            for (currentCoordinate.x = 0; currentCoordinate.x < Size.x; currentCoordinate.x++)
            {
                if (CanAddItem(item, currentCoordinate, false)) { return true; }
                if (CanAddItem(item, currentCoordinate, true)) { return true; }
            }
        }
        return false;
    }
    public bool CanAddItem(ItemBase item, out Vector2Int validCoordinate, out bool isRotated)
    {
        validCoordinate = new Vector2Int();
        isRotated = false;
        for (validCoordinate.y = 0; validCoordinate.y < Size.y; validCoordinate.y++)
        {
            for (validCoordinate.x = 0; validCoordinate.x < Size.x; validCoordinate.x++)
            {
                if (CanAddItem(item, validCoordinate, false)) { return true; }
                if (CanAddItem(item, validCoordinate, true)) { isRotated = true; return true; }
            }
        }
        return false;
    }

    private void AddItem(ItemBase item, Vector2Int targetCoordinate, bool isRotated)
    {
        Vector2Int itemSize = isRotated ? new Vector2Int(item.Data.Size.y, item.Data.Size.x) : item.Data.Size;
        for (int y = 0; y < itemSize.y; y++)
        {
            for (int x = 0; x < itemSize.x; x++)
            {
                Slots[targetCoordinate.x + x, targetCoordinate.y + y].InsertItem(item);
            }
        }
        OnItemAdded(item, new ItemData
        {
            subInventory = this,
            gridCoordinate = targetCoordinate,
            isRotated = isRotated
        });
    }
    public bool TryAddItem(ItemBase item)
    {
        if (!CanAddItem(item, out Vector2Int position, out bool isRotated)) { return false; }
        AddItem(item, position, isRotated);
        return true;
    }
    public bool TryAddItem(ItemBase item, Vector2Int targetCoordinate, bool isRotated)
    {
        if (!CanAddItem(item, targetCoordinate, isRotated)) { return false; }
        AddItem(item, targetCoordinate, isRotated);
        return true;
    }

    private void RemoveItem(ItemBase item)
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
    public bool TryRemoveItem(ItemBase item)
    {
        if (item == null) { return false; }
        RemoveItem(item);
        return true;
    }

    public bool CanMoveItem(ItemBase item, SubInventory targetSubInventory, Vector2Int targetCoordinate, bool isRotated)
    {
        if (!targetSubInventory.BoundsCheck(item, targetCoordinate, isRotated)) { return false; }
        if (!targetSubInventory.SlotsOccupiedCheck(item, targetCoordinate, isRotated)) { return false; }
        return true;
    }

    public void MoveItem(ItemBase item, SubInventory targetSubInventory, Vector2Int targetCoordinate, bool isRotated)
    {
        RemoveItem(item);
        targetSubInventory.AddItem(item, targetCoordinate, isRotated);
        Vector2Int itemSize = isRotated ? new Vector2Int(item.Data.Size.y, item.Data.Size.x) : item.Data.Size;
        for (int y = 0; y < itemSize.y; y++)
        {
            for (int x = 0; x < itemSize.x; x++)
            {
                targetSubInventory.Slots[targetCoordinate.x + x, targetCoordinate.y + y].InsertItem(item);
            }
        }
    }

    public bool TryMoveItem(ItemBase item, SubInventory targetSubInventory, Vector2Int targetCoordinate, bool isRotated, Vector2Int originCoordinate, bool originRotatedStatus)
    {
        if (!CanMoveItem(item, targetSubInventory, targetCoordinate, isRotated)) { return false; }
        MoveItem(item, targetSubInventory, targetCoordinate, isRotated);
        return true;
    }
}
