using System.Collections;
using System.Collections.Generic;
using Szczepanski.UI;
using UnityEngine;

public class ItemEquipableInventory : ItemBase, IInventory, IEquipable
{
    public new ItemEquipableInventorySO Data { get; private set; }
    public Inventory Inventory { get; private set; }

    //TODO: Make this more modular using the interfaces
    //Inspect should be the same for every type of item without having to rewrite it
    public override ContextMenuOption[] ContextMenuOptions
    {
        get => new ContextMenuOption[4]
        {
            new ContextMenuOption { optionText = "Inspect", OnSelected = () => Debug.Log(this)},
            new ContextMenuOption { optionText = "Open", OnSelected = () => IInventory.OpenUI(this, InventoryUIManager.CANVAS.transform)},
            new ContextMenuOption { optionText = "Equip", OnSelected = () => throw new System.NotImplementedException("Equip Item Not Implemented")},
            new ContextMenuOption { optionText = "Discard", OnSelected = () => ParentSubInventory.TryRemoveItem(this)},
        };
    }

    public ItemEquipableInventory(ItemEquipableInventorySO data) : base(data)
    {
        Data = data;
        Inventory = new Inventory(this);
    }
    public override string ToString()
    {
        return $"{Data.FullName}\n   Type: {GetType()}\n   Size: {Data.Size}\n   EquipmentType: {"TBD"}\n   Storage: {Data.StorageSlots}";
    }
}
