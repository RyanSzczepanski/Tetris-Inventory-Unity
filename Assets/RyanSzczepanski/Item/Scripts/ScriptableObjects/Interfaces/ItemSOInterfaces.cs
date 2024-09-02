using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventorySO
{

    public Vector2Int[] SubInventories { get; }
    public SubInventoryArrangement SubInventoryArrangements { get; }

    public static Vector2Int[] GetAllSubInventories(SubInventoryArrangement subInventoryArrangement)
    {
        List<Vector2Int> subInventories = new List<Vector2Int>();

        if (!subInventoryArrangement.IsLeaf)
        {
            foreach (SubInventoryArrangement child in subInventoryArrangement.childArrangements)
            {
                subInventories.AddRange(GetAllSubInventories(child));
            }
        }
        if (subInventoryArrangement.HasSubInventory)
        {
            subInventories.Add(subInventoryArrangement.subInventorySize);
        }
        return subInventories.ToArray();
    }
}
