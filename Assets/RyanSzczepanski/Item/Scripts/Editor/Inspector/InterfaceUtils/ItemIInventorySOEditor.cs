using UnityEngine.UIElements;

public static class IItemInventoryEditor
{
    public static VisualElement GenerateInspector(VisualTreeAsset asset)
    {
        VisualElement content = asset.Instantiate();
        return content;
    }
}
