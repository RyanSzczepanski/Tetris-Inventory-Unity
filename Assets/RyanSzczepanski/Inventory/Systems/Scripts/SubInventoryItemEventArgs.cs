using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubInventoryItemEventArgs : EventArgs
{
    public Item Item { get; set; }
    public Vector2Int OriginCellCoordinate { get; set; }
}
