using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public interface IInventorySOEditor
{
    public static VisualElement GenerateInspector(VisualTreeAsset asset, IInventorySO IInventory)
    {
        var inventoryFoldout = new Foldout() { viewDataKey = "InventoryFoldout", text = "Inventory", name = "inventory-foldout"};
        VisualElement content = asset.Instantiate();
        InventoryCellDrawSettingsSO drawSettingsSO = AssetDatabase.LoadAssetAtPath<InventoryCellDrawSettingsSO>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SubInventory UI Draw Settings")[0]));
        InventoryCellDrawSettings drawSettings = new InventoryCellDrawSettings(drawSettingsSO);

        content.Q<VisualElement>("inventory-preview").Add(InventoryUIToolkitGenerator.GenerateInventoryPreview(IInventory, in drawSettings));
        inventoryFoldout.Add(content);

        InitCallbacks(inventoryFoldout);

        return inventoryFoldout;
    }

    public static void InitCallbacks(VisualElement content)
    {
        VisualElement previewVisualElement = content.Q<VisualElement>("inventory-preview");
        content.Q<Button>("button-inventory-preview").RegisterCallback<ClickEvent>((e) => { previewVisualElement.style.display = (DisplayStyle)((int)previewVisualElement.style.display.value ^ 0b_1); });
    }

    public void OnInventoryChanged(ChangeEvent<Object> e)
    {
        
    }
}
