using Szczepanski.UI;
using UnityEngine;

public interface IInventory
{
    public Inventory Inventory { get; }
    public static ItemTags TAGS = ItemTags.Inventory;
    public static GameObject OpenUI(ItemBase item, Transform windowParent)
    {
        if (item is not IInventory) { throw new System.ArgumentException("Input item does not implement IInventory interface"); }
        if (item.Data is not IInventorySO) { throw new System.ArgumentException("Input item data does not implement IInventorySO interface"); }

        FloatingWindowSettings settings = new FloatingWindowSettings()
        {
            isDraggable = true,
            isResizeable = false,
            minWindowSize = new Vector2(0, 0),
            title = item.Data.ShortName
        };

        GameObject floatingWindow = FloatingWindowFactory.CreateFloatingWindow(windowParent, settings);
        Transform transform = floatingWindow.GetComponent<FloatingWindow>().Content.rectTransform;
        InventoryUIGenerator.GenerateUIObject(transform, item.Data as IInventorySO, item as IInventory, in InventoryUIManager.DRAW_SETTINGS);
        return floatingWindow;
    }
}

public interface IEquipable
{
    public static ItemTags TAGS = ItemTags.Equipable;
}
