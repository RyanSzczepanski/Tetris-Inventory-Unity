using UnityEngine;

[CreateAssetMenu(fileName = "New Equipable Inventory Item SO", menuName = "Items/Equipable Inventory Item")]
public class ItemEquipableInventorySO : ItemBaseSO, IInventorySO, IEquipableSO
{
    public Vector2Int[] SubInventories { get => IInventorySO.GetAllSubInventories(SubInventoryArrangements); }
    public int StorageSlots { get => IInventorySO.GetStorageSlotsCount(SubInventoryArrangements); }
    public SubInventoryArrangement SubInventoryArrangements => m_SubInventoryArrangements;
    [SerializeField] private SubInventoryArrangement m_SubInventoryArrangements;

    public override ItemTags Tags => IInventorySO.TAG | IEquipableSO.TAG;

    public override ItemBase CreateItem()
    {
        return new ItemEquipableInventory(this);
    }
}
