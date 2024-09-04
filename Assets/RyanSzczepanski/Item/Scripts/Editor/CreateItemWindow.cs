using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class CreateItemWindow : EditorWindow
{
    //Prefs
    string itemScriptableObjectClassPath;
    string itemObjectClassPath;
    string itemScriptableObjectPath;

    ItemTags itemTags;
    string newItemName;
    List<Type> itemTypes;

    [MenuItem("Window/Create Item GUI")]
    public static void ShowWindow()
    {
        CreateItemWindow window = EditorWindow.GetWindow<CreateItemWindow>("Item Tool");
        window.Init();
    }

    void SavePrefs()
    {
        EditorPrefs.SetString(nameof(itemScriptableObjectClassPath), itemScriptableObjectClassPath);
        EditorPrefs.SetString(nameof(itemObjectClassPath), itemObjectClassPath);
        EditorPrefs.SetString(nameof(itemScriptableObjectPath), itemScriptableObjectPath);
    }
    void LoadPrefs()
    {
        itemScriptableObjectClassPath = EditorPrefs.GetString(nameof(itemScriptableObjectClassPath));
        itemObjectClassPath = EditorPrefs.GetString(nameof(itemObjectClassPath));
        itemScriptableObjectPath = EditorPrefs.GetString(nameof(itemScriptableObjectPath));
    }

    void Init()
    {
        LoadPrefs();
        GetItemTypes(out itemTypes);
        if (itemScriptableObjectClassPath == string.Empty)
        {
            itemScriptableObjectClassPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(nameof(ItemBaseSO))[0]);
            itemScriptableObjectClassPath = itemScriptableObjectClassPath.Remove(itemScriptableObjectClassPath.Length - (nameof(ItemBaseSO).Length + 3));
        }
        if (itemObjectClassPath == string.Empty)
        {
            itemObjectClassPath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(nameof(ItemBase))[0]);
            itemObjectClassPath = itemObjectClassPath.Remove(itemObjectClassPath.Length - (nameof(ItemBase).Length + 3));
        }
    }

    void GetItemTypes(out List<Type> types)
    {
        types = new List<Type>();
        foreach (Type type in typeof(ItemBaseSO).Assembly.GetTypes())
        {
            if (type.BaseType == typeof(ItemBaseSO)) { types.Add(type); }
        }
    }

    string[] tabs = { "Settings", "Create Item", "Search For Item" };
    int tabSelected = 0;
    void OnGUI()
    {
        if (itemTypes == null) { GetItemTypes(out itemTypes); } 
        tabSelected = GUILayout.Toolbar(tabSelected, tabs);
        switch (tabSelected)
        {
            case 0:
                DrawSettings();
                break;
            case 1:
                DrawCreateItem();
                break;
            case 2:
                DrawSearchForItem();
                break;
        }
    }

    private void DrawSettings()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Item Scriptable Objects Class Path", "This is the file location for the SCRIPTABLE OBEJCT C# files"), GUILayout.Width(225));
        itemScriptableObjectClassPath = EditorGUILayout.TextField(itemScriptableObjectClassPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Item Objects Class Path", "This is the file location for the ITEM OBJECT C# files"), GUILayout.Width(225));
        itemObjectClassPath = EditorGUILayout.TextField(itemObjectClassPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Item Assets Path", "This is the file location for the ITEM ASSET files"), GUILayout.Width(225));
        itemScriptableObjectPath = EditorGUILayout.TextField(itemScriptableObjectPath);
        EditorGUILayout.EndHorizontal();
    }
    private bool TryGetTargetItemTypeFromTags(ItemTags tags, out Type targetType)
    {
        
        foreach (Type type in itemTypes.AsReadOnly())
        {
            if (ItemTagsUtils.TypesToTags(type) != itemTags) { continue; }
            targetType = type;
            return true;
        }
        targetType = null;
        return false;
    }
    private void DrawCreateItem()
    {
        UnityEngine.Object targetAssetObject = null;
        itemTags = (ItemTags)EditorGUILayout.EnumFlagsField("Item Tags", itemTags);
        if (!TryGetTargetItemTypeFromTags(itemTags, out Type targetType)) { return; }

        newItemName = EditorGUILayout.TextField("Item Name", newItemName);
        if (Regex.IsMatch(newItemName, "^(?<ItemName>(?>\\w|[- ])*)$"))
        {
            string[] assetGUIDS = AssetDatabase.FindAssets(newItemName);
            foreach(string assetGUID in assetGUIDS)
            {
                UnityEngine.Object assetObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(assetGUID));
                if(!itemTypes.Contains(assetObject.GetType()) || ((ItemBaseSO)assetObject).Tags != itemTags) { continue; }
                targetAssetObject ??= assetObject;
                GUILayout.Label(assetObject.name);
            }

            if (targetAssetObject != null)
            {
                if(GUILayout.Button($"Open Asset {targetAssetObject.name}"))
                {
                    PopUpAssetInspector.ShowWindow(targetAssetObject);
                }
            }
            else if (GUILayout.Button($"Create Asset"))
            {
                ScriptableObject so = ScriptableObject.CreateInstance(targetType.Name);
                AssetDatabase.CreateAsset(so, $"{itemScriptableObjectPath}{newItemName}.asset");
                PopUpAssetInspector.ShowWindow(so);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Asset name is invalid\nMust be Alphanumeric and can contain Spaces, Hyphens and Underscores", MessageType.Warning, false);
        }
        

        if (GUILayout.Button($"Open Class {targetType.Name}"))
        {
            foreach (var asset in AssetDatabase.FindAssets(targetType.Name))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(asset)));
            }
        }
    }
    private void DrawSearchForItem()
    {
        
    }


    private void OnDestroy()
    {
        Debug.Log("Saving Prefs");
        SavePrefs();
    }
}

public class PopUpAssetInspector : EditorWindow
{
    private UnityEngine.Object asset;
    private Editor assetEditor;

    public static PopUpAssetInspector ShowWindow(UnityEngine.Object asset)
    {
        var window = CreateWindow<PopUpAssetInspector>($"{asset.name} | {asset.GetType().Name}");
        window.asset = asset;
        window.assetEditor = Editor.CreateEditor(asset);
        return window;
    }

    private void OnGUI()
    {
        GUI.enabled = false;
        asset = EditorGUILayout.ObjectField("Asset", asset, asset.GetType(), false);
        GUI.enabled = true;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        assetEditor.OnInspectorGUI();
        EditorGUILayout.EndVertical();
    }
}
