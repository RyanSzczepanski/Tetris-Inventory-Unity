using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

public static class InventoryUIGenerator
{
    private static int subInventoryTracker;
    private static InventoryCellDrawSettings drawSettings;

    public static void GenerateUIObject(Transform transform, IInventorySO itemInventorySO, IInventory itemInventory, in InventoryCellDrawSettings drawSettings)
    {
        subInventoryTracker = 0;
        InventoryUIGenerator.drawSettings = drawSettings;
        GameObject Inventory = new GameObject("Inventory", typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        Inventory.transform.SetParent(transform, false);

        HorizontalOrVerticalLayoutGroup layoutGroup = Inventory.GetComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;

        ContentSizeFitter csf = Inventory.GetComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        csf.verticalFit = ContentSizeFitter.FitMode.MinSize;

        ArrangementTreeSearch(itemInventorySO.SubInventoryArrangements, Inventory.transform, in itemInventory);
    }
    private static void ArrangementTreeSearch(SubInventoryArrangement arrangement, Transform parent, in IInventory item)
    {
        Transform newParent;
        if (arrangement.HasSubInventory)
        {
            GameObject go = SubInventoryUIGenerator.GenerateSubInventoryObject(item.Inventory.SubInventories[subInventoryTracker], parent); ;
            newParent = go.transform;
            subInventoryTracker++;
        }
        else
        {
            newParent = GenerateArrangement(arrangement, parent).transform;
        }

        if (arrangement.IsLeaf) { return; }
        foreach (SubInventoryArrangement child in arrangement.childArrangements)
        {
            ArrangementTreeSearch(child, newParent, in item);
        }
    }
    private static GameObject GenerateArrangement(SubInventoryArrangement arrangement, Transform parent)
    {
        GameObject go = new GameObject("Arrangement");
        go.transform.SetParent(parent, false);
        HorizontalOrVerticalLayoutGroup layoutGroup;
        if (arrangement.direction == GridLayoutGroup.Axis.Vertical)
        {
            layoutGroup = go.AddComponent<VerticalLayoutGroup>();
        }
        else
        {
            layoutGroup = go.AddComponent<HorizontalLayoutGroup>();
        }
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childAlignment = arrangement.alignment;
        layoutGroup.spacing = 5;

        return go;
    }
}


public static class SubInventoryUIGenerator
{
    public static Dictionary<Vector2Int, Sprite> CAHCED_SPRITES = new Dictionary<Vector2Int, Sprite>();

    public static GameObject GenerateSubInventoryObject(SubInventory subInventory, Transform parent)
    {
        //Rename Object
        GameObject subInventoryObject = new GameObject($"{subInventory.Size.x}x{subInventory.Size.y} SubInventory");
        subInventoryObject.transform.SetParent(parent, false);
        SubInventoryUI subInventoryUI = subInventoryObject.AddComponent<SubInventoryUI>();
        subInventoryUI.Init(subInventory);
        //Generate Or Get Cached Sprite
        if (!CAHCED_SPRITES.TryGetValue(subInventory.Size, out Sprite sprite))
        {
            sprite = GenerateSprite(subInventory.Size, InventoryUIManager.DRAW_SETTINGS);
            CAHCED_SPRITES.Add(subInventory.Size, sprite);
        }

        //Set Component Values
        Image imageComponent = subInventoryObject.GetComponent<Image>();
        imageComponent.sprite = sprite;

        LayoutElement layoutElement = subInventoryObject.GetComponent<LayoutElement>();
        layoutElement.minWidth = sprite.texture.width;
        layoutElement.minHeight = sprite.texture.height;


        RectTransform rectTransform = subInventoryObject.GetComponent<RectTransform>(); 
        rectTransform.localScale = Vector3.one;
        rectTransform.pivot = Vector2.up;

        return subInventoryObject;
    }
    public static Sprite GenerateSprite(Vector2Int size, in InventoryCellDrawSettings drawSettings)
    {
        Texture2D texture2D = GenerateCellGridTexture(size, in drawSettings);
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        sprite.name = $"{size.x}x{size.y} SubInventory";
        return sprite;
    }
    private static Texture2D GenerateCellGridTexture(Vector2Int gridSize, in InventoryCellDrawSettings drawSettings)
    {
        Vector2Int textureSize = new(
        gridSize.x * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2,
        gridSize.y * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2
        );

        Texture2D texture = new(textureSize.x, textureSize.y);

        NativeArray<Color32> color = new NativeArray<Color32>(textureSize.x * textureSize.y, Allocator.TempJob);
        CalculatePixelColorsJob job = new CalculatePixelColorsJob() { color = color, textureSize = textureSize, drawSettings = drawSettings };


        JobHandle sheduleParralelJobHandle = job.ScheduleParallel(textureSize.x * textureSize.y, 64, new JobHandle());
        sheduleParralelJobHandle.Complete();

        texture.SetPixels32(color.ToArray());
        color.Dispose();
        texture.Apply();
        return texture;
    }
    public struct CalculatePixelColorsJob : IJobFor
    {
        public InventoryCellDrawSettings drawSettings;
        public Vector2Int textureSize;
        public NativeArray<Color32> color;

        [BurstCompile]
        public void Execute(int index)
        {
            int x = index % textureSize.x;
            int y = Mathf.FloorToInt((float)index / textureSize.x);
            color[index] = CalculatePixelColor(x, y, textureSize, drawSettings);
        }

        [BurstCompile]
        private Color CalculatePixelColor(int x, int y, Vector2Int textureSize, in InventoryCellDrawSettings drawSettings)
        {
            Color pixelColor;
            //Outline of entire sub inventory
            if (x < drawSettings._outlineSize ||
                y < drawSettings._outlineSize ||
                x > textureSize.x - drawSettings._outlineSize - 1 ||
                y > textureSize.y - drawSettings._outlineSize - 1)
            {
                //Breaks up solid colors
                if ((x + y % 2) % 2 == 0)
                    pixelColor = drawSettings._outlineAccentColor;
                else
                    pixelColor = drawSettings._outlineColor;
            }
            //CellOutlinePadding (Keeps an even outline width on outside cell blocks)
            else if (
                x < drawSettings._paddingSize + drawSettings._outlineSize ||
                y < drawSettings._paddingSize + drawSettings._outlineSize ||
                x > textureSize.x - (drawSettings._paddingSize + drawSettings._outlineSize) ||
                y > textureSize.y - (drawSettings._paddingSize + drawSettings._outlineSize))
            {
                //Breaks up solid colors
                if ((x + y % 2) % 2 == 0)
                    pixelColor = drawSettings._cellOutlineAccentColor;
                else
                    pixelColor = drawSettings._cellOutlineColor;
            }
            //CellOutline
            else if ((x - (drawSettings._paddingSize + drawSettings._outlineSize)) % drawSettings._cellSize == 0 ||
            (y - (drawSettings._paddingSize + drawSettings._outlineSize)) % drawSettings._cellSize == 0 ||
            (x - (drawSettings._paddingSize + drawSettings._outlineSize)) % drawSettings._cellSize == drawSettings._cellSize - 1 ||
                (y - (drawSettings._paddingSize + drawSettings._outlineSize)) % drawSettings._cellSize == drawSettings._cellSize - 1)
            {
                //Breaks up solid colors
                if ((x + y % 2) % 2 == 0)
                    pixelColor = drawSettings._cellOutlineAccentColor;
                else
                    pixelColor = drawSettings._cellOutlineColor;
            }
            //Fill
            else
            {
                //Breaks up solid colors
                if ((x + y % 2) % 2 == 0)
                    pixelColor = drawSettings._cellAccentColor;
                else
                    pixelColor = drawSettings._cellColor;
            }
            return pixelColor;
        }
    }
}