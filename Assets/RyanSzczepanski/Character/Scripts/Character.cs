using UnityEngine;
using Szczepanski.UI;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public ItemBaseSO fillItem;
    public EquipmentSlot equipmentSlot;
    public ItemBaseSO itemBaseSO;
    public Transform parent;

    [SerializeField] GraphicRaycaster m_Raycaster;

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
                title = itemBaseSO.ShortName
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
                if (!itemInventory.Inventory.TryAddItem(fillItem.CreateItem())) { break; }
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