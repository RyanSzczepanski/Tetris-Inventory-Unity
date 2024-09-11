using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipableInventorySO : ItemBaseSO, IItemInventorySO, IItemEquipableSO
{
    public Vector2Int[] SubInventories { get => IItemInventorySO.GetAllSubInventories(SubInventoryArrangements); }
    public int StorageSlots { get => IItemInventorySO.GetStorageSlotsCount(SubInventoryArrangements); }
    [field: SerializeField] public SubInventoryArrangement SubInventoryArrangements { get; private set; }

    public override ItemTags Tags => IItemInventorySO.TAG | IItemEquipableSO.TAG;
}
