using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(ItemInventorySO))]
public class ItemInventorySOEditor : ItemBaseSOEditor, IInventorySOEditor
{
    public override VisualElement CreateInspectorGUI()
    {
        Init();
        root.Add(GenerateItemBaseInspector());
        root.Add(IInventorySOEditor.GenerateInspector(inspectorAssets.ItemIInventoryInterface, serializedObject.targetObject as IInventorySO));
        var foldout = new Foldout() { viewDataKey = "ItemBasicSOInspectorFoldout", text = "Full Inspector" };
        InspectorElement.FillDefaultInspector(foldout, serializedObject, this);
        root.Add(foldout);
        
        return root;
    }
}
