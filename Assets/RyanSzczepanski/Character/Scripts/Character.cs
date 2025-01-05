using UnityEngine;
using Szczepanski.UI;
using UnityEngine.UI;
using System;

public class Character : MonoBehaviour
{
    public ItemBaseSO fillItem;
    [NonSerialized] public EquipmentSlot[] equipmentSlots = {
        new EquipmentSlot { equipmentType = EquipmentType.Helmet },
        new EquipmentSlot { equipmentType = EquipmentType.PlateCarrier },
        new EquipmentSlot {equipmentType = EquipmentType.Backpack}
    };
    public ItemBaseSO itemBaseSO;
    public Transform parent;

    [SerializeField] GraphicRaycaster m_Raycaster;

    private void Awake()
    {
        //equipmentSlot.TryEquipItem(itemBaseSO.CreateItem());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TryEquipItem(itemBaseSO.CreateItem());
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
            InventoryUIGenerator.GenerateUIObject(transform, equipmentSlots[2].item.Data as IInventorySO, equipmentSlots[2].item as IInventory, in InventoryUIManager.DRAW_SETTINGS);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //IInventory itemInventory = equipmentSlot.item as IInventory;

            for (int i = 0; i < 10000; i++)
            {
                //if (!itemInventory.Inventory.TryAddItem(fillItem.CreateItem())) { break; }
            }
        }
    }

    public void TryEquipItem(ItemBase itemBase)
    {
        IEquipableSO Equipable = itemBase.Data as IEquipableSO;
        if (Equipable == null)
        {
            throw new ArgumentException($"Item [{itemBase.Data.GetType()}] does not implement [IEquipableSO]");
        }
        equipmentSlots[(int)Equipable.EquipmentType].TryEquipItem(itemBase);
    }
}

[System.Serializable]
public struct EquipmentSlot
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
    Helmet = 0,
    PlateCarrier,
    Backpack,
}