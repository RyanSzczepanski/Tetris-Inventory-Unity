using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item SO", menuName = "Items/Inventory Item")]
public class ItemInventorySO : ItemBaseSO, IInventorySO
{
    public Vector2Int[] SubInventories { get => IInventorySO.GetAllSubInventories(SubInventoryArrangements); }
    public int StorageSlots { get
        {
            int storage = 0;
            foreach (Vector2Int subInventory in IInventorySO.GetAllSubInventories(SubInventoryArrangements))
            {
                storage += subInventory.x * subInventory.y;
            }
            return storage;
        }
    }
    [field: SerializeField] public SubInventoryArrangement SubInventoryArrangements { get ; private set; }

    public override ItemBase CreateItem()
    {
        return new ItemInventory(this);
    }
}