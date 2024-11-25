///------------------------------------///
///    This File Was Auto Generated    ///
/// Using Szczepanski Script Generator ///
///------------------------------------///

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Inventory SO", menuName = "Item/Item Inventory SO")]
public class ItemInventorySO : ItemBaseSO, IInventorySO
{
	#region Properties
	#region IInventorySO
	public Vector2Int[] SubInventories { get => GetAllSubInventories(); }
	public int StorageSlots { get => GetStorageSlotsCount(); }
	public SubInventoryArrangement SubInventoryArrangements { get => m_SubInventoryArrangements; }
	[SerializeField] private SubInventoryArrangement m_SubInventoryArrangements;
	#endregion

	public override ItemTags Tags { get => IInventorySO.TAG; }
	#endregion
	#region Functions
	#region IInventorySO
	public  Vector2Int[] GetAllSubInventories()
	{
		List<Vector2Int> subInventories = new List<Vector2Int>();	
			
		if (!m_SubInventoryArrangements.IsLeaf)	
		{	
			foreach (SubInventoryArrangement child in m_SubInventoryArrangements.childArrangements)	
			{	
				subInventories.AddRange(GetAllSubInventories());	
			}	
		}	
		if (m_SubInventoryArrangements.HasSubInventory)	
		{
			subInventories.Add(m_SubInventoryArrangements.subInventorySize);	
		}	
		return subInventories.ToArray();
	}
	public  int GetStorageSlotsCount()
	{
		int storage = 0;	
		foreach (Vector2Int subInventory in GetAllSubInventories())	
		{	
			storage += subInventory.x * subInventory.y;	
		}	
		return storage;
	}
	#endregion

	public override ItemBase CreateItem()
	{
		return new ItemInventory(this);
	}
	#endregion
}