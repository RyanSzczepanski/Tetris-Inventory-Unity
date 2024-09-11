using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ItemBaseSO))]
public class ItemBaseSOEditor : Editor
{
    protected ItemInspectorUXMLAssetsSO inspectorAssets;

    protected VisualElement root;

    public override VisualElement CreateInspectorGUI()
    {
        Init();
        return base.CreateInspectorGUI();
    }

    protected void Init()
    {
        Debug.Log("Init");
        root = new VisualElement();
        inspectorAssets = (ItemInspectorUXMLAssetsSO)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:ItemInspectorUXMLAssetsSO")[0]));

    }

    public void OnIconChanged(ChangeEvent<Object> e)
    {
        if(e.newValue == null)
        {
            root.Q<VisualElement>("icon-preview").style.opacity = 0;
            root.Q<VisualElement>("no-icon-preview").style.opacity = 100;
        }
        else
        {
            root.Q<VisualElement>("icon-preview").style.opacity = 100;
            root.Q<VisualElement>("no-icon-preview").style.opacity = 0;
        }
        root.Q<VisualElement>("icon-preview").style.backgroundImage = (e.newValue as Sprite)?.texture;
    }

    public VisualElement GenerateItemBaseInspector()
    {
        VisualElement content = inspectorAssets.ItemBaseSO.Instantiate();
        content.Q<ObjectField>("sprite-field").RegisterCallback<ChangeEvent<Object>>(OnIconChanged);
        return content;
    }
}


