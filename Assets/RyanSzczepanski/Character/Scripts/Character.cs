using UnityEngine;
using Szczepanski.UI;
using UnityEngine.UI;
using UnityEngine.Profiling;
using TMPro;

public class Character : MonoBehaviour
{
    public ItemBaseSO fillItem;
    public EquipmentSlot equipmentSlot;
    public ItemBaseSO itemBaseSO;
    public Transform parent;

    private void Awake()
    {
        equipmentSlot.TryEquipItem(itemBaseSO.CreateItem());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            equipmentSlot.TryEquipItem(itemBaseSO.CreateItem());
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            FloatingWindowSettings settings = new FloatingWindowSettings()
            {
                isDraggable = true,
                isResizeable = false,
                minWindowSize = new Vector2(0, 0),
                title = itemBaseSO.ShortName,
                font = Resources.Load<TMP_FontAsset>("Font/KodeMono-Bold SDF")
            };
            GameObject floatingWindow = FloatingWindowFactory.CreateFloatingWindow(parent, settings);
            Transform transform = floatingWindow.GetComponent<FloatingWindow>().Content.rectTransform;
            InventoryUIGenerator.GenerateUIObject(transform, equipmentSlot.item.Data as IInventorySO, equipmentSlot.item as IInventory, in InventoryUIManager.DRAW_SETTINGS);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            IInventory itemInventory = equipmentSlot.item as IInventory;

            for (int i = 0; i < 10000; i++)
            {
                ItemBase newItem = (ItemBase)fillItem.CreateItem();
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