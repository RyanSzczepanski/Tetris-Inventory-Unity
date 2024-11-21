using System.IO;
using Szczepanski.ScriptGenerator;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ItemEditorTool
{
    public class Window : EditorWindow
    {
        public VisualTreeAsset visualTree;

        //TODO: Move Tab Logic Into Seperate Classes

        //public CreateTab createTab;
        //public SearchTab searchTab;
        //public SettingsTab settingsTab;

        [SerializeField] VisualTreeAsset createTabVT;
        [SerializeField] VisualTreeAsset searchTabVT;
        [SerializeField] VisualTreeAsset settingsTabVT;
        
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
            Window window = GetWindow<Window>();
            window.titleContent = new GUIContent("Item Tool");

            Vector2 size = new Vector2(600, 475);
            window.minSize = size;
            window.maxSize = size;
        }

        bool outlineToggleLastState;
        public void OnGUI()
        {
            //Search Logic
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
            if (toolbarContentContainer != null)
            {
                rootFromUXML.Remove(toolbarContentContainer);
                toolbarContentContainer = null;
            }

            switch (newIndex)
            {
                case 0:
                    toolbarContentContainer = CreateTabGUI(createTabVT);
                    
                    rootFromUXML.Q<Button>("button-generate").clicked += () =>
                    {
                        ScriptGeneratorSettings settings = ItemSOGenerator.ToSettingsStruct((ItemTags)rootFromUXML.Q<EnumFlagsField>("item-tags").value);
                        //AssetDatabase.MakeEditable(EditorPrefs.GetString($"RyanSzczepanski_ItemTools_itemScriptableObjectClassPath"));
                        ScriptGenerator.GenerateCSFile(settings, EditorPrefs.GetString($"RyanSzczepanski_ItemTools_itemScriptableObjectClassPath"));
                        //AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        //Debug.Log(ScriptGenerator.GenerateCode(settings));
                    };
                    break;
                case 1:
                    toolbarContentContainer = CreateTabGUI(searchTabVT);
                    //TODO: Move into seacrh class
                    InventoryCellDrawSettings drawSettings = new InventoryCellDrawSettings(AssetDatabase.LoadAssetAtPath<InventoryCellDrawSettingsSO>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SubInventory UI Draw Settings")[0])));
                    ItemInventorySO itemInventorySO = AssetDatabase.LoadAssetAtPath<ItemInventorySO>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Test Rig 2")[0]));
                    toolbarContentContainer.Add(InventoryUIToolkitGenerator.GenerateInventoryPreview(itemInventorySO as IInventorySO, in drawSettings));
                    break;
                case 2:
                    toolbarContentContainer = CreateTabGUI(settingsTabVT);
                    //TODO: Move into settings class
                    ItemSettingsPrefs prefs = new ItemSettingsPrefs(rootFromUXML);

                    rootFromUXML.Q<Button>("save-button").clicked += () => prefs.SavePrefs();
                    rootFromUXML.Q<Button>("revert-button").clicked += () => prefs.LoadPrefs();
                    break;
            }
            rootVisualElement.styleSheets.Add(baseStyleSheet);
        }

        private TemplateContainer CreateTabGUI(VisualTreeAsset asset)
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
            for (int i = 0; i < toolbarButtonsQuery.Length; i++)
            {
                //Cant just pass i into func it gets wierd
                int index = i;
                rootFromUXML.Q<ToolbarButton>(toolbarButtonsQuery[i]).clicked += () => ToolBarClicked(index);
            }
        }
    }
    public abstract class Tab : ScriptableObject
    {
        public VisualTreeAsset VisualElement;
        public abstract void TabOpen();
        public abstract void TabClose();
    }

public struct ItemSettingsPrefs
    {
        private readonly TextField itemAssetPathTextField;
        private readonly TextField itemObjectClassPathTextField;
        private readonly TextField itemScriptableObjectClassPathTextField;

        public ItemSettingsPrefs(VisualElement root)
        {
            itemAssetPathTextField = root.Q<TextField>("item-assets-path");
            itemObjectClassPathTextField = root.Q<TextField>("item-object-class-path");
            itemScriptableObjectClassPathTextField = root.Q<TextField>("item-so-class-path");

            LoadPrefs();
        }

        public void SavePrefs()
        {
            string itemAssetsPath = itemAssetPathTextField.text;
            EditorPrefs.SetString($"RyanSzczepanski_ItemTools_{nameof(itemAssetsPath)}", itemAssetsPath);

            string itemObjectClassPath = itemObjectClassPathTextField.text;
            EditorPrefs.SetString($"RyanSzczepanski_ItemTools_{nameof(itemObjectClassPath)}", itemObjectClassPath);

            string itemScriptableObjectClassPath = itemScriptableObjectClassPathTextField.text;
            EditorPrefs.SetString($"RyanSzczepanski_ItemTools_{nameof(itemScriptableObjectClassPath)}", itemScriptableObjectClassPath);
        }

        public void LoadPrefs()
        {
            string itemAssetsPath = EditorPrefs.GetString($"RyanSzczepanski_ItemTools_{nameof(itemAssetsPath)}");
            itemAssetPathTextField.SetValueWithoutNotify(itemAssetsPath);

            string itemObjectClassPath = EditorPrefs.GetString($"RyanSzczepanski_ItemTools_{nameof(itemObjectClassPath)}");
            itemObjectClassPathTextField.SetValueWithoutNotify(itemObjectClassPath);

            string itemScriptableObjectClassPath = EditorPrefs.GetString($"RyanSzczepanski_ItemTools_{nameof(itemScriptableObjectClassPath)}");
            itemScriptableObjectClassPathTextField.SetValueWithoutNotify(itemScriptableObjectClassPath);
        }
    }
}
