using UnityEngine;
using Szczepanski.UI;
using System.Dynamic;
using Unity.VisualScripting;
using System.Runtime.InteropServices;

public class Character : MonoBehaviour
{
    public ItemBasicSO fillItem;
    public EquipmentSlot equipmentSlot;
    public ItemBaseSO itemBaseSO;
    public Transform parent;

    private void Awake()
    {
        ItemInventory itemInventory = (ItemInventory)itemBaseSO.CreateItem();
        equipmentSlot.TryEquipItem(itemInventory);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ItemInventory itemInventory = (ItemInventory)itemBaseSO.CreateItem();
            equipmentSlot.TryEquipItem(itemInventory);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            FloatingWindowSettings settings = new FloatingWindowSettings()
            {
                isDraggable = true,
                isResizeable = false,
                minWindowSize = new Vector2(0, 0),
                title = itemBaseSO.ShortName
            };

            GameObject floatingWindow = FloatingWindowFactory.CreateFloatingWindow(parent, settings);
            Transform transform = floatingWindow.GetComponent<FloatingWindow>().Content.rectTransform;
            InventoryUIGenerator.GenerateUIObject(transform, equipmentSlot.item as ItemInventory, in InventoryUIManager.DRAW_SETTINGS);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ItemInventory itemInventory = equipmentSlot.item as ItemInventory;

            for (int i = 0; i < 10000; i++)
            {
                ItemBasic newItem = (ItemBasic)fillItem.CreateItem();
                if (!itemInventory.Inventory.TryAddItem(newItem)) { break; }
            }
        }
    }
}

[System.Serializable]
public class EquipmentSlot
{
    public EquipmentType equipmentType;
    public ItemBase item;

    public bool TryEquipItem(ItemBase item)
    {
        if (item is IInventory)
        {
            this.item = item;
            return true;
        }
        return false;
    }
}

public enum EquipmentType
{
    Backpack = 0,
}