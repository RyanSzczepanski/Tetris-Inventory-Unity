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
	public float MaxDurability { get => m_MaxDurability; private set => m_MaxDurability = value; }
	[SerializeField] private float m_MaxDurability;
	#endregion
	#region IInventorySO
	public Vector2Int[] SubInventories { get => GetAllSubInventories(); }
	public int StorageSlots { get => GetStorageSlotsCount(); }
	public SubInventoryArrangement SubInventoryArrangements { get => m_SubInventoryArrangements; }
	[SerializeField] private SubInventoryArrangement m_SubInventoryArrangements;
	#endregion
	#region Base
	public override ItemTags Tags { get => IEquipableSO.TAG | IInventorySO.TAG; }
	#endregion
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
	#region Base
	public override ItemBase CreateItem()
	{
		return new ItemEquipableInventory(this);
	}
	#endregion
	#endregion
}