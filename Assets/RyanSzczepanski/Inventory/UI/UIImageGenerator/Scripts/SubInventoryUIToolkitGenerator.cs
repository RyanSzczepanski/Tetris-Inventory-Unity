using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine;

public static class InventoryUIToolkitGenerator
{
    private static int subInventoryTracker;
    private static InventoryCellDrawSettings drawSettings;

    public static VisualElement GenerateInventoryPreview(IInventorySO itemInventory, in InventoryCellDrawSettings drawSettings)
    {
        subInventoryTracker = 0;
        InventoryUIToolkitGenerator.drawSettings = drawSettings;
        VisualElement inventory = new VisualElement();
        inventory.style.flexGrow = 0;
        inventory.style.alignSelf = Align.FlexStart;

        ArrangementTreeSearch(itemInventory.SubInventoryArrangements, inventory, in itemInventory);
        return inventory;
    }
    private static void ArrangementTreeSearch(SubInventoryArrangement arrangement, VisualElement parent, in IInventorySO item)
    {
        VisualElement newParent;
        if (arrangement.HasSubInventory)
        {
            //TODO:
            newParent = SubInventoryUIToolkitGenerator.GenerateSubInventoryObject(item.SubInventorySizes[subInventoryTracker], parent, in drawSettings); ;
            subInventoryTracker++;
        }
        else
        {
            newParent = GenerateArrangement(arrangement, parent);
        }

        if (arrangement.IsLeaf) { return; }
        foreach (SubInventoryArrangement child in arrangement.childArrangements)
        {
            //TODO
            ArrangementTreeSearch(child, newParent, in item);
        }
    }
    private static VisualElement GenerateArrangement(SubInventoryArrangement arrangement, VisualElement parent)
    {
        VisualElement ve = new VisualElement();
        //TODO: Some bug with Upper Right and Parent Alignment
        if (arrangement.direction == GridLayoutGroup.Axis.Horizontal)
        {
            ve.style.flexDirection = FlexDirection.Row;

            if (arrangement.alignment == TextAnchor.UpperLeft || arrangement.alignment == TextAnchor.UpperCenter || arrangement.alignment == TextAnchor.UpperRight)
            {
                ve.style.alignItems = Align.FlexStart;
            }
            else if(arrangement.alignment == TextAnchor.MiddleLeft || arrangement.alignment == TextAnchor.MiddleCenter || arrangement.alignment == TextAnchor.MiddleRight)
            {
                ve.style.alignItems = Align.Center;
            }
            else
            {
                ve.style.alignItems = Align.FlexEnd;
            }

            if (arrangement.alignment == TextAnchor.UpperLeft || arrangement.alignment == TextAnchor.MiddleLeft || arrangement.alignment == TextAnchor.LowerLeft)
            {
                ve.style.alignSelf = Align.FlexStart;
            }
            else if (arrangement.alignment == TextAnchor.UpperCenter || arrangement.alignment == TextAnchor.MiddleCenter || arrangement.alignment == TextAnchor.LowerCenter)
            {
                ve.style.alignSelf = Align.Center;
            }
            else
            {
                ve.style.alignSelf = Align.FlexEnd;
            }
        }
        else
        {
            if (arrangement.alignment == TextAnchor.UpperLeft || arrangement.alignment == TextAnchor.UpperCenter || arrangement.alignment == TextAnchor.UpperRight)
            {
                ve.style.alignSelf = Align.FlexStart;
            }
            else if (arrangement.alignment == TextAnchor.MiddleLeft || arrangement.alignment == TextAnchor.MiddleCenter || arrangement.alignment == TextAnchor.MiddleRight)
            {
                ve.style.alignSelf = Align.Center;
            }
            else
            {
                ve.style.alignSelf = Align.FlexEnd;
            }

            if (arrangement.alignment == TextAnchor.UpperLeft || arrangement.alignment == TextAnchor.MiddleLeft || arrangement.alignment == TextAnchor.LowerLeft)
            {
                ve.style.alignItems = Align.FlexStart;
            }
            else if (arrangement.alignment == TextAnchor.UpperCenter || arrangement.alignment == TextAnchor.MiddleCenter || arrangement.alignment == TextAnchor.LowerCenter)
            {
                ve.style.alignItems = Align.Center;
            }
            else
            {
                ve.style.alignItems = Align.FlexEnd;
            }
        }
        ve.style.flexGrow = 0;

        Color color = Random.ColorHSV(0,1,.5f,1,1,1,1,1);
        color.a = .25f;
        ve.AddToClassList("inventory-preview-arrangement");

        ve.style.borderBottomColor = color;
        ve.style.borderTopColor = color;
        ve.style.borderLeftColor = color;
        ve.style.borderRightColor = color;

        parent.Add(ve);

        return ve;
    }
}

public static class SubInventoryUIToolkitGenerator
{
    public static VisualElement GenerateSubInventoryObject(Vector2Int subInventory, VisualElement parent, in InventoryCellDrawSettings drawSettings)
    {
        //Rename Object
        VisualElement ve = new VisualElement();

        Sprite sprite = SubInventoryUIGenerator.GenerateSprite(subInventory, drawSettings);

        //Set Component Values
        ve.style.backgroundImage = sprite.texture;
        ve.style.width = sprite.texture.width;
        ve.style.height = sprite.texture.height;

        ve.style.marginBottom = 2.5f;
        ve.style.marginTop = 2.5f;
        ve.style.marginLeft = 2.5f;
        ve.style.marginRight = 2.5f;

        parent.Add(ve);
        return ve;
    }
}