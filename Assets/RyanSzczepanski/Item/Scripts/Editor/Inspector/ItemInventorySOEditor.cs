using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(ItemInventorySO))]
public class ItemInventorySOEditor : ItemBaseSOEditor
{
    public override VisualElement CreateInspectorGUI()
    {
        Init();
        root.Add(GenerateItemBaseInspector());
        root.Add(IItemInventoryEditor.GenerateInspector(inspectorAssets.ItemIInventoryInterface));

        var foldout = new Foldout() { viewDataKey = "ItemBasicSOInspectorFoldout", text = "Full Inspector" };
        InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
        root.Add(foldout);
        return root;
    }
}
