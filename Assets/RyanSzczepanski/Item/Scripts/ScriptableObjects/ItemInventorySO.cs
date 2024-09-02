using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item SO", menuName = "Items/Inventory Item")]
public class ItemInventorySO : ItemBaseSO, IInventorySO
{
    public Vector2Int[] SubInventories { get => IInventorySO.GetAllSubInventories(SubInventoryArrangements); }
    public int StorageSlots { get => IInventorySO.GetStorageSlotsCount(SubInventoryArrangements); }
    [field: SerializeField] public SubInventoryArrangement SubInventoryArrangements { get ; private set; }

    public override ItemBase CreateItem()
    {
        return new ItemInventory(this);
    }
}