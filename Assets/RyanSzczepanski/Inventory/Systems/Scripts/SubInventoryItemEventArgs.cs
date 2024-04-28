using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubInventoryItemAddedEventArgs : EventArgs
{
    public Item Item { get; set; }

    public Vector2Int TargetCellCoordinate { get; set; }
    public bool TargetIsRotated { get; set; }
    public SubInventory TargetSubInventory { get; set; }

}
public class SubInventoryItemMovedEventArgs : EventArgs
{
    public Item Item { get; set; }

    public Vector2Int OriginCellCoordinate { get; set; }
    public bool OriginIsRotated { get; set; }
    public SubInventory OriginSubInventory { get; set; }

    public Vector2Int TargetCellCoordinate { get; set; }
    public bool TargetIsRotated { get; set; }
    public SubInventory TargetSubInventory { get; set; }

}
public class SubInventoryItemRemovedEventArgs : EventArgs
{
    public Item Item { get; set; }
}


public class ItemRemovedEventArgs
{
    public Item Item { get; set; }
}
