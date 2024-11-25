///------------------------------------///
///    This File Was Auto Generated    ///
/// Using Szczepanski Script Generator ///
///------------------------------------///

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Basic SO", menuName = "Item/Item Basic SO")]
public class ItemBasicSO : ItemBaseSO
{
	#region Properties
	#region Base
	public override ItemTags Tags { get => ItemTags.Basic; }
	#endregion
	#endregion
	#region Functions
	#region Base
	public override ItemBase CreateItem()
	{
		return new ItemBasic(this);
	}
	#endregion
	#endregion
}