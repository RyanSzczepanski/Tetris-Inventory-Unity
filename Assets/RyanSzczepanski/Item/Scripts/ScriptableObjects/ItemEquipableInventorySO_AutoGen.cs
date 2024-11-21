using UnityEngine;

namespace TestNamespace
{
	[CreateAssetMenu(fileName = "New Item Equipable Inventory SO", menuName = "Item/Item Equipable Inventory SO")]
	public class ItemEquipableInventorySO : ItemBaseSO, IEquipableSO, IInventorySO
	{
		public override ItemTags Tags { get =>  IEquipableSO.TAG | IInventorySO.TAG;  }

		public override ItemBase CreateItem()
		{
			return new ItemEquipableInventory(this);
		}

	}

}
