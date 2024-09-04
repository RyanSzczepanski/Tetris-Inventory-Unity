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
    TemplateContainer tempContainer;

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
        Toggle OutlineToggle = tempContainer?.Query<Toggle>("outline-toggle");
        if (OutlineToggle != null && OutlineToggle.value != outlineToggleLastState)
        {
            if (OutlineToggle.value)
            {
                tempContainer.styleSheets.Add(inventoryPreviewOutline);
                tempContainer.styleSheets.Remove(inventoryPreviewNoOutline);
            }
            else
            {
                tempContainer.styleSheets.Add(inventoryPreviewNoOutline);
                tempContainer.styleSheets.Remove(inventoryPreviewOutline);
            }
            outlineToggleLastState = OutlineToggle.value;
        }
    }

    private void ToolBarClicked(int newIndex)
    {
        if (selectedToolbarIndex == newIndex) { return; }
        selectedToolbarIndex = newIndex;
        if(tempContainer != null) { Debug.Log("Removing Temp Container"); rootFromUXML.Remove(tempContainer); tempContainer = null; }
        
        switch (newIndex)
        {
            case 0:
                tempContainer = CreateTempGUI(createItemVisualTree);
                break;
            case 1:
                tempContainer = CreateTempGUI(testTreeAsset);
                InventoryCellDrawSettings drawSettings = new InventoryCellDrawSettings(AssetDatabase.LoadAssetAtPath<InventoryCellDrawSettingsSO>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SubInventory UI Draw Settings")[0])));
                ItemInventorySO itemInventorySO = AssetDatabase.LoadAssetAtPath<ItemInventorySO>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Test Rig 2")[0]));
                tempContainer.Add(InventoryUIToolkitGenerator.GenerateInventoryPreview(itemInventorySO as IInventorySO, in drawSettings));
                break;
        }
        rootVisualElement.styleSheets.Add(baseStyleSheet);
        
    }

    private TemplateContainer CreateTempGUI(VisualTreeAsset asset)
    {
        tempContainer = asset.Instantiate();
        rootFromUXML.Add(tempContainer);
        return tempContainer;
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
