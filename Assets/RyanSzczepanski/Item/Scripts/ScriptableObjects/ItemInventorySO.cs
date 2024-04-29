using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item SO", menuName = "Items/Inventory Item")]
public class ItemInventorySO : ItemBasicSO, IInventorySO
{
    private Vector2Int[] subInventories;
    public Vector2Int[] SubInventories
    {
        get
        {
            if (subInventories.Length == 0)
            {
                subInventories = IInventorySO.GetAllSubInventories(SubInventoryArrangements);
            }

            return subInventories;
        }
    }
    [field: SerializeField]
    public SubInventoryArrangement SubInventoryArrangements { get ; set ; }
}


public interface IInventorySO
{

    public Vector2Int[] SubInventories { get; }
    public SubInventoryArrangement SubInventoryArrangements { get; set; }

    public static Vector2Int[] GetAllSubInventories(SubInventoryArrangement subInventoryArrangement)
    {
        List<Vector2Int> subInventories = new List<Vector2Int>();

        if (!subInventoryArrangement.IsLeaf)
        {
            foreach (SubInventoryArrangement child in subInventoryArrangement.childArrangements)
            {
                subInventories.AddRange(GetAllSubInventories(child));
            }
        }
        if (subInventoryArrangement.HasSubInventory)
        {
            subInventories.Add(subInventoryArrangement.subInventorySize);
        }
        return subInventories.ToArray();
    }
}