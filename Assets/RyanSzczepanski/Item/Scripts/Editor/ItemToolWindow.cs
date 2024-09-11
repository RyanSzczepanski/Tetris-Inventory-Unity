using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.ReorderableList;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemToolWindow : EditorWindow
{
    public VisualTreeAsset visualTree;
    public VisualTreeAsset createItemVisualTree;
    public VisualTreeAsset testTreeAsset;
    public StyleSheet baseStyleSheet;
    public StyleSheet inventoryPreviewOutline;
    public StyleSheet inventoryPreviewNoOutline;

    VisualElement rootFromUXML;


    string[] toolbarButtonsQuery;
    int selectedToolbarIndex = -1;
    TemplateContainer toolbarContentContainer;

    private record ToolbarButtonRecord
    {
        public readonly ToolbarButton toolbarButton;
        public readonly int index;
        public ToolbarButtonRecord(ToolbarButton toolbarButton, int index)
        {
            this.toolbarButton = toolbarButton;
            this.index = index;
        }
        
    }

    [MenuItem("WUG/Item Database")]
    public static void ShowWindow()
    {
        ItemToolWindow window = GetWindow<ItemToolWindow>();
        window.titleContent = new GUIContent("Item Tool");
        

        Vector2 size = new Vector2(600, 475);
        window.minSize = size;
        window.maxSize = size;
    }

    bool outlineToggleLastState;
    public void OnGUI()
    {
        Toggle OutlineToggle = toolbarContentContainer?.Query<Toggle>("outline-toggle");
        if (OutlineToggle != null && OutlineToggle.value != outlineToggleLastState)
        {
            if (OutlineToggle.value)
            {
                toolbarContentContainer.styleSheets.Add(inventoryPreviewOutline);
                toolbarContentContainer.styleSheets.Remove(inventoryPreviewNoOutline);
            }
            else
            {
                toolbarContentContainer.styleSheets.Add(inventoryPreviewNoOutline);
                toolbarContentContainer.styleSheets.Remove(inventoryPreviewOutline);
            }
            outlineToggleLastState = OutlineToggle.value;
        }
    }

    private void ToolBarClicked(int newIndex)
    {
        if (selectedToolbarIndex == newIndex) { return; }
        selectedToolbarIndex = newIndex;
        if(toolbarContentContainer != null) { Debug.Log("Removing Temp Container"); rootFromUXML.Remove(toolbarContentContainer); toolbarContentContainer = null; }
        
        switch (newIndex)
        {
            case 0:
                toolbarContentContainer = CreateTempGUI(createItemVisualTree);
                break;
            case 1:
                toolbarContentContainer = CreateTempGUI(testTreeAsset);
                InventoryCellDrawSettings drawSettings = new InventoryCellDrawSettings(AssetDatabase.LoadAssetAtPath<InventoryCellDrawSettingsSO>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SubInventory UI Draw Settings")[0])));
                ItemInventorySO itemInventorySO = AssetDatabase.LoadAssetAtPath<ItemInventorySO>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Test Rig 2")[0]));
                toolbarContentContainer.Add(InventoryUIToolkitGenerator.GenerateInventoryPreview(itemInventorySO as IItemInventorySO, in drawSettings));
                break;
        }
        rootVisualElement.styleSheets.Add(baseStyleSheet);
        
    }

    private TemplateContainer CreateTempGUI(VisualTreeAsset asset)
    {
        toolbarContentContainer = asset.Instantiate();
        rootFromUXML.Add(toolbarContentContainer);
        return toolbarContentContainer;
    }

    public void CreateGUI()
    {
        rootFromUXML = visualTree.Instantiate();
        rootVisualElement.Add(rootFromUXML);
        toolbarButtonsQuery = new string[]
        {
            "toolbar-create",
            "toolbar-search",
            "toolbar-settings",
        };
        for(int i = 0; i < toolbarButtonsQuery.Length; i++)
        {
            //Cant just pass i into func it gets wierd
            int index = i;
            rootFromUXML.Q<ToolbarButton>(toolbarButtonsQuery[i]).clicked += () => ToolBarClicked(index);
        }

    }
}
