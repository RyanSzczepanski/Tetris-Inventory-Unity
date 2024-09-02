using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventorySO
{
    public const ItemTags TAG = ItemTags.Inventory;


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

    public static int GetStorageSlotsCount(SubInventoryArrangement subInventoryArrangement)
    {
        int storage = 0;
        foreach (Vector2Int subInventory in IInventorySO.GetAllSubInventories(subInventoryArrangement))
        {
            storage += subInventory.x * subInventory.y;
        }
        return storage;
    }
}

public interface IEquipableSO
{
    public const ItemTags TAG = ItemTags.Equipable;

}

