using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubInventoryUIGeneratorTest : MonoBehaviour
{
    public Vector2Int size;
    public InventoryCellDrawSettingsSO drawSettingsSO;

    public InventoryDataTest inventoryDataTest;

    private void Start()
    {
        SubInventoryUIGenerator UIGenerator;
        Inventory inventory = new Inventory(inventoryDataTest);
        foreach (SubInventory subInventory in inventory.SubInventories)
        {
            UIGenerator = new SubInventoryUIGenerator(drawSettingsSO, subInventory);
            UIGenerator.GenerateSubInventoryObject(transform);
        }
        foreach (Slot slot in inventory.SubInventories[0].Slots) { Debug.Log(slot.ItemInSlot); }
        Debug.Log(inventory.SubInventories[0].TryAddItem(new Item() { Size = new Vector2Int(2, 2) }));
        foreach (Slot slot in inventory.SubInventories[0].Slots) { Debug.Log(slot.ItemInSlot); }
        Debug.Log(inventory.SubInventories[0].CanAddItem(new Item() { Size = new Vector2Int(2, 2) }));

    }
}