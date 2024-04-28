using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubInventoryUIGeneratorTest : MonoBehaviour
{
    public InventoryCellDrawSettingsSO drawSettingsSO;

    public InventoryDataTest inventoryDataTest;

    private Inventory inventory;

    public void GenerateNewInventory()
    {
        inventory = new Inventory(inventoryDataTest);
    }
    public void GenerateInventoryUI()
    {
        SubInventoryUIGenerator UIGenerator;
        foreach (SubInventory subInventory in inventory.SubInventories)
        {
            UIGenerator = new SubInventoryUIGenerator(drawSettingsSO, subInventory);
            UIGenerator.GenerateSubInventoryObject(transform, drawSettingsSO);
        }
    }


    private void Start()
    {
        GenerateNewInventory();
        GenerateInventoryUI();
    }
}