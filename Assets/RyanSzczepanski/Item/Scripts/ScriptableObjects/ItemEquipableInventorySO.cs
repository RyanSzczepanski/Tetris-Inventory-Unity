///------------------------------------///
///    This File Was Auto Generated    ///
/// Using Szczepanski Script Generator ///
///------------------------------------///

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Equipable Inventory SO", menuName = "Item/Item Equipable Inventory SO")]
public class ItemEquipableInventorySO : ItemBaseSO, IEquipableSO, IInventorySO
{
	#region Properties
	#region IEquipableSO
	public EquipmentType EquipmentType { get => m_EquipmentType; private set => m_EquipmentType = value; }
	[SerializeField] private EquipmentType m_EquipmentType;
	#endregion
	#region IInventorySO
	public SubInventoryArrangement SubInventoryArrangements { get => m_SubInventoryArrangements; }
	[SerializeField] private SubInventoryArrangement m_SubInventoryArrangements;
	public int StorageSlots { get => GetStorageSlotsCount(); }
	public Vector2Int[] SubInventorySizes { get => GetAllSubInventorySizes(m_SubInventoryArrangements); }
	#endregion
	#region Base
	public override ItemTags Tags { get => IEquipableSO.TAG | IInventorySO.TAG; }
	#endregion
	#endregion
	#region Functions
	#region IInventorySO
	private int GetStorageSlotsCount()
	{
		int storage = 0;
		foreach (Vector2Int subInventory in GetAllSubInventorySizes(m_SubInventoryArrangements))
		{
			storage += subInventory.x * subInventory.y;
		}
		return storage;
	}
	private Vector2Int[] GetAllSubInventorySizes(SubInventoryArrangement subInventoryArrangement)
	{
		List<Vector2Int> subInventories = new List<Vector2Int>();
		
		if (!subInventoryArrangement.IsLeaf)
		{
			foreach (SubInventoryArrangement child in subInventoryArrangement.childArrangements)
			{
				subInventories.AddRange(GetAllSubInventorySizes(child));
			}
		}
		if (subInventoryArrangement.HasSubInventory)
		{
			subInventories.Add(subInventoryArrangement.subInventorySize);
		}
		return subInventories.ToArray();
	}
	#endregion
	#region Base
	public override ItemBase CreateItem()
	{
		return new ItemEquipableInventory(this);
	}
	#endregion
	#endregion
}