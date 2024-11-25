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
	public float MaxDurability { get => m_MaxDurability; private set => m_MaxDurability = value; }
	[SerializeField] private float m_MaxDurability;
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