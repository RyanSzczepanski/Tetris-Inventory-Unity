using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item SO", menuName = "Items/Inventory Item")]
public class ItemInventorySO : ItemBaseSO, IInventorySO
{
    public Vector2Int[] SubInventories { get => IInventorySO.GetAllSubInventories(SubInventoryArrangements); }
    [field: SerializeField]
    public SubInventoryArrangement SubInventoryArrangements { get ; private set; }

    new public ItemInventory CreateItem()
    {
        return new ItemInventory(this);
    }
}