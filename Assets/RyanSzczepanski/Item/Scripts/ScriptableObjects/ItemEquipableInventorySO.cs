using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipable Inventory Item SO", menuName = "Items/Equipable Inventory Item")]
public class ItemEquipableInventorySO : ItemBaseSO, IInventorySO, IItemEquipableSO
{
    public Vector2Int[] SubInventories { get => IInventorySO.GetAllSubInventories(SubInventoryArrangements); }
    public int StorageSlots { get => IInventorySO.GetStorageSlotsCount(SubInventoryArrangements); }
    public SubInventoryArrangement SubInventoryArrangements => m_SubInventoryArrangements;
    [SerializeField] private SubInventoryArrangement m_SubInventoryArrangements;

    public override ItemTags Tags => IInventorySO.TAG | IItemEquipableSO.TAG;
}
