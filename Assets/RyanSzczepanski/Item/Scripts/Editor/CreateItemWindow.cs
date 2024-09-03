using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

public class CreateItemWindow : EditorWindow
{
    //Prefs
    string itemScriptableObjectClassPath;
    string itemObjectClassPath;
    string itemScriptableObjectPath;

    ItemTags itemTags;
    string newItemName;
    List<Type> itemTypes = new List<Type>();

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
        foreach (Type type in typeof(ItemBaseSO).Assembly.GetTypes())
        {
            if (type.BaseType == typeof(ItemBaseSO)) { itemTypes.Add(type); }
        }
    }

    string[] tabs = { "Settings", "Create Item", "Search For Item" };
    int tabSelected = -1;
    void OnGUI()
    {
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
        EditorGUILayout.LabelField("Item Scriptable Object Class Path", GUILayout.Width(225));
        itemScriptableObjectClassPath = EditorGUILayout.TextField(itemScriptableObjectClassPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Item Object Class Path", GUILayout.Width(225));
        itemObjectClassPath = EditorGUILayout.TextField(itemObjectClassPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Item Scriptable Object Path", GUILayout.Width(225));
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
        itemTags = (ItemTags)EditorGUILayout.EnumFlagsField("Item Tags", itemTags);
        if (!TryGetTargetItemTypeFromTags(itemTags, out Type targetType)) { return; }


        newItemName = EditorGUILayout.TextField("Item Name", newItemName);

        if (GUILayout.Button($"Create Asset"))
        {
            ScriptableObject so = ScriptableObject.CreateInstance(targetType.Name);
            AssetDatabase.CreateAsset(so, $"{itemScriptableObjectPath}{newItemName}.asset");
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
