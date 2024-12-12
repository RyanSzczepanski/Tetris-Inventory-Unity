///------------------------------------///
///    This File Was Auto Generated    ///
/// Using Szczepanski Script Generator ///
///------------------------------------///

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Equipable SO", menuName = "Item/Item Equipable SO")]
public class ItemEquipableSO : ItemBaseSO, IEquipableSO
{
	#region Properties
	#region IEquipableSO
	public EquipmentType EquipmentType { get => m_EquipmentType; private set => m_EquipmentType = value; }
	[SerializeField] private EquipmentType m_EquipmentType;
	#endregion
	#region Base
	public override ItemTags Tags { get => IEquipableSO.TAG; }
	#endregion
	#endregion
	#region Functions
	#region Base
	public override ItemBase CreateItem()
	{
		return new ItemBase(this);
	}
	#endregion
	#endregion
}