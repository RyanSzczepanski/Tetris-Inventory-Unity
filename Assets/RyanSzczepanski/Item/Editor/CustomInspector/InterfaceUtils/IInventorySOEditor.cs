using UnityEditor;
using UnityEditor.UIElements;
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

        content.Q<VisualElement>("inventory-preview").Add(InventoryUIToolkitGenerator.GenerateInventoryPreview(IInventory, drawSettings));


        //Callbacks
        VisualElement previewVisualElement = content.Q<VisualElement>("inventory-preview");
        content.Q<Button>("button-inventory-preview").RegisterCallback<ClickEvent>((e) => {
            previewVisualElement.style.display = (DisplayStyle)((int)previewVisualElement.style.display.value ^ 0b_1);
            OnInventoryChanged(content.Q<VisualElement>("inventory-preview"), IInventory, drawSettings);
        });
        //Defer Callbacks to let sub properties generate
        content.schedule.Execute(() => { DeferedCallbackSetting(content, IInventory, drawSettings); }).StartingIn(0);

        inventoryFoldout.Add(content);
        return inventoryFoldout;
    }

    public static void DeferedCallbackSetting(VisualElement content, IInventorySO IInventory,  InventoryCellDrawSettings drawSettings)
    {
        content.Q<PropertyField>("subinventory-arrangements").Query<PropertyField>()
        .ForEach((element) =>
        {
            element.RegisterCallback<SerializedPropertyChangeEvent>((e) =>
            {
                OnInventoryChanged(content.Q<VisualElement>("inventory-preview"), IInventory, drawSettings);
            });
        });
    }

    public static void OnInventoryChanged(VisualElement parent, in IInventorySO IInventory, in InventoryCellDrawSettings drawSettings)
    {
        parent.RemoveAt(0);
        parent.Add(InventoryUIToolkitGenerator.GenerateInventoryPreview(IInventory, in drawSettings));
    }
}
