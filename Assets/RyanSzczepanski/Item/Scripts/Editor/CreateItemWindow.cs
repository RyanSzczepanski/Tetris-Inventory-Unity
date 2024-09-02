using Sirenix.Utilities;
using System;
using UnityEditor;
using UnityEngine;

public class CreateItemWindow : EditorWindow
{
    ItemTags itemTags;
    string itemScriptableObjectClassPath;
    string itemObjectClassPath;
    string itemScriptableObjectPath;
    string newItemName;
    [MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        CreateItemWindow window = EditorWindow.GetWindow<CreateItemWindow>();
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
        
    }

    void OnGUI()
    {
        itemScriptableObjectClassPath = EditorGUILayout.TextField("Item Scriptable Object Class Path", itemScriptableObjectClassPath);
        itemObjectClassPath = EditorGUILayout.TextField("Item Object Class Path", itemObjectClassPath);
        itemScriptableObjectPath = EditorGUILayout.TextField("Item Scriptable Object Path", itemScriptableObjectPath);

        Type targetType = null;
        GUILayout.Label("Item Tags", EditorStyles.boldLabel);
        itemTags = (ItemTags)EditorGUILayout.EnumFlagsField("Item Tags", itemTags);

        foreach (Type type in typeof(ItemBaseSO).Assembly.GetTypes())
        {
            if(type.BaseType != typeof(ItemBaseSO)) { continue; }
            if(ItemTagsUtils.TypesToTags(type) != itemTags) { continue; }
            targetType = type;
            //Debug.Log(ItemTagsUtils.TypesToTags(type));
        }
        
        if(targetType == null) { return; }
        if (GUILayout.Button($"Open {targetType.Name}"))
        {
            foreach (var asset in AssetDatabase.FindAssets(targetType.Name))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(asset)));
            }
        }

        newItemName = EditorGUILayout.TextField("Item Name", newItemName);

        if (GUILayout.Button($"Create Scriptable Object {targetType.Name}"))
        {
            ScriptableObject so = ScriptableObject.CreateInstance(targetType.Name);
            AssetDatabase.CreateAsset(so, $"{itemScriptableObjectPath}{newItemName}.asset");
            //Debug.Log(ScriptableObject);
            foreach (var member in targetType.GetBaseClasses())
            {
                foreach (var Method in typeof(ScriptableObject).GetMethods())
                {
                    //Debug.Log(Method.Name);
                }
            }
        }
    }


    private void OnDestroy()
    {
        Debug.Log("Saving Prefs");
        SavePrefs();
    }
}
