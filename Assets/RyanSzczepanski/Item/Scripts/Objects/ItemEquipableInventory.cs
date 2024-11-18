using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipableInventory : ItemBase, IInventory
{
    public new ItemEquipableInventorySO Data { get; private set; }
    public Inventory Inventory { get; private set; }

    public ItemEquipableInventory(ItemEquipableInventorySO data) : base(data)
    {
        Data = data;
        Inventory = new Inventory(data);
    }
    public override string ToString()
    {
        return $"{Data.FullName}\n   Type: {GetType()}\n   Size: {Data.Size}\n   EquipmentType: {"TBD"}\n   Storage: {Data.StorageSlots}";
    }
}
