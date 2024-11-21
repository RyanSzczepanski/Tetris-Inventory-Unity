using UnityEngine;

[CreateAssetMenu(fileName = "New Item Equipable SO", menuName = "Item/Item Equipable SO")]
public class ItemEquipableSO : ItemBaseSO, IEquipableSO
{
	public override ItemTags Tags { get =>  IEquipableSO.TAG;  }
	public override ItemBase CreateItem()
	{
		return new ItemEquipable(this);
	}
}