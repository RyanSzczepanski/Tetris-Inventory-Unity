using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipableInventorySO : ItemBaseSO, IInventorySO, IEquipableSO
{
    public Vector2Int[] SubInventories { get => IInventorySO.GetAllSubInventories(SubInventoryArrangements); }
    public int StorageSlots { get => IInventorySO.GetStorageSlotsCount(SubInventoryArrangements); }
    [field: SerializeField] public SubInventoryArrangement SubInventoryArrangements { get; private set; }
}
