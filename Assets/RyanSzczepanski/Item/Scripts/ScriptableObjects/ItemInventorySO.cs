using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item SO", menuName = "Items/Inventory Item")]
public class ItemInventorySO : ItemBaseSO, IInventorySO
{
    public Vector2Int[] SubInventories => IInventorySO.GetAllSubInventories(SubInventoryArrangements);
    public int StorageSlots => IInventorySO.GetStorageSlotsCount(SubInventoryArrangements);
    public SubInventoryArrangement SubInventoryArrangements => m_SubInventoryArrangements;
    [SerializeField] private SubInventoryArrangement m_SubInventoryArrangements;

    public override ItemTags Tags => IInventorySO.TAG;

    public override ItemBase CreateItem()
    {
        return new ItemInventory(this);
    }
}