using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item SO", menuName = "Items/Inventory Item")]
public class ItemInventorySO : ItemBaseSO, IItemInventorySO
{
    public Vector2Int[] SubInventories => IItemInventorySO.GetAllSubInventories(SubInventoryArrangements);
    public int StorageSlots => IItemInventorySO.GetStorageSlotsCount(SubInventoryArrangements);
    public SubInventoryArrangement SubInventoryArrangements => m_SubInventoryArrangements;
    [SerializeField] private SubInventoryArrangement m_SubInventoryArrangements;

    public override ItemTags Tags => IItemInventorySO.TAG;


    public override ItemBase CreateItem()
    {
        return new ItemInventory(this);
    }
}