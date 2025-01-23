using System;
using UnityEngine;

public class SubInventoryItemAddedEventArgs : EventArgs
{
    public ItemBase Item { get; set; }

    public Vector2Int TargetCellCoordinate { get; set; }
    public bool TargetIsRotated { get; set; }
    public SubInventory TargetSubInventory { get; set; }

}
public class SubInventoryItemMovedEventArgs : EventArgs
{
    public ItemBase Item { get; set; }

    public Vector2Int OriginCellCoordinate { get; set; }
    public bool OriginIsRotated { get; set; }
    public SubInventory OriginSubInventory { get; set; }

    public Vector2Int TargetCellCoordinate { get; set; }
    public bool TargetIsRotated { get; set; }
    public SubInventory TargetSubInventory { get; set; }

}
public class SubInventoryItemRemovedEventArgs : EventArgs
{
    public ItemBase Item { get; set; }
    public SubInventory SubInventory { get; set; }
}


public class ItemAddedEventArgs
{
    public ItemBase Item { get; set; }
    public SubInventory SubInventory { get; set; }
    public bool IsRotated { get; set; }
}
public class ItemRemovedEventArgs
{
    public ItemBase Item { get; set; }
    public SubInventory SubInventory { get; set; }
}
