using UnityEngine;

public interface IInventorySO
{
    public const ItemTags TAG = ItemTags.Inventory;

    public Vector2Int[] SubInventories { get; }
    public SubInventoryArrangement SubInventoryArrangements { get; }
}

public interface IEquipableSO
{
    public const ItemTags TAG = ItemTags.Equipable;

    public float MaxDurability { get; }
}