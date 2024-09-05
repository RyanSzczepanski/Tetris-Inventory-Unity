using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

[CustomEditor(typeof(ItemBasicSO))]
public class ItemBasicSOEditor : ItemBaseSOEditor
{
    public override VisualElement CreateInspectorGUI()
    {
        Init();

        root.Add(GenerateItemBaseInspector());

        var foldout = new Foldout() { viewDataKey = "ItemBasicSOInspectorFoldout", text = "Full Inspector" };
        InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
        root.Add(foldout);
        return root;
    }
}