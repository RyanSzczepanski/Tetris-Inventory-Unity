using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubInventoryUIGeneratorTest : MonoBehaviour
{
    public Vector2Int size;
    public InventoryCellDrawSettingsSO drawSettingsSO;

    SubInventory subInventory = new SubInventory();

    private void Start()
    {
        subInventory.Size = size;
        SubInventoryUIGenerator UIGenerator = new SubInventoryUIGenerator(drawSettingsSO, subInventory);
        UIGenerator.GenerateSubInventoryObject(transform);
    }
}