using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public static class InventoryUIGenerator
{
    private static int subInventoryTracker;
    private static InventoryCellDrawSettings drawSettings;

    private static GameObject INVENTORY_PREFAB;
    private static GameObject HORIZONTAL_ARRANGEMENT_PREFAB;
    private static GameObject VERTICAL_ARRANGEMENT_PREFAB;

    public static void PreLoadPrefabs()
    {
        INVENTORY_PREFAB ??= new GameObject("Inventory", typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        HORIZONTAL_ARRANGEMENT_PREFAB ??= new GameObject("Arrangement", typeof(HorizontalLayoutGroup));
        VERTICAL_ARRANGEMENT_PREFAB ??= new GameObject("Arrangement", typeof(VerticalLayoutGroup));
    }

    public static void GenerateUIObject(Transform transform, IInventorySO itemInventorySO, IInventory itemInventory, in InventoryCellDrawSettings drawSettings)
    {
        subInventoryTracker = 0;
        InventoryUIGenerator.drawSettings = drawSettings;
        GameObject Inventory = GameObject.Instantiate(INVENTORY_PREFAB, transform);

        HorizontalOrVerticalLayoutGroup layoutGroup = Inventory.GetComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;

        ContentSizeFitter csf = Inventory.GetComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        csf.verticalFit = ContentSizeFitter.FitMode.MinSize;

        ArrangementTreeSearch(itemInventorySO.SubInventoryArrangements, Inventory.transform, itemInventory);
    }
    private static void ArrangementTreeSearch(SubInventoryArrangement arrangement, Transform parent, IInventory item)
    {
        Transform newParent;
        if (arrangement.HasSubInventory)
        {
            GameObject go = SubInventoryUIGenerator.GenerateSubInventoryObject(item.Inventory.SubInventories[subInventoryTracker], parent, drawSettings);
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
            ArrangementTreeSearch(child, newParent, item);
        }
    }
    private static GameObject GenerateArrangement(SubInventoryArrangement arrangement, Transform parent)
    {
        GameObject go;
        HorizontalOrVerticalLayoutGroup layoutGroup;
        if (arrangement.direction == GridLayoutGroup.Axis.Vertical)
        {
            go = GameObject.Instantiate(VERTICAL_ARRANGEMENT_PREFAB, parent);
            layoutGroup = go.GetComponent<VerticalLayoutGroup>();
        }
        else
        {
            go = GameObject.Instantiate(HORIZONTAL_ARRANGEMENT_PREFAB, parent);
            layoutGroup = go.GetComponent<HorizontalLayoutGroup>();
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
    private static Dictionary<Vector2Int, Texture2D> CAHCED_SPRITES = new Dictionary<Vector2Int, Texture2D>();
    private static ComputeShader computeShader;

    private static GameObject SUB_INVENTORY;

    public static void SetShader(ComputeShader shader)
    {
        computeShader = shader;
    }
    public static void PreLoadPrefabs()
    {
        SUB_INVENTORY ??= new GameObject($"SubInventory Prefab", typeof(SubInventoryUI));
    }

    public static GameObject GenerateSubInventoryObject(SubInventory subInventory, Transform parent, InventoryCellDrawSettings drawSettings)
    {
        //Rename Object
        GameObject subInventoryObject = GameObject.Instantiate(SUB_INVENTORY, parent);
        subInventoryObject.name = $"{subInventory.Size.x}x{subInventory.Size.y} SubInventory";

        SubInventoryUI subInventoryUI = subInventoryObject.GetComponent<SubInventoryUI>();
        subInventoryUI.Init(subInventory, drawSettings);
        //Generate Or Get Cached Sprite
        Texture2D texture2D;
        if(!CAHCED_SPRITES.TryGetValue(subInventory.Size, out texture2D))
        {
            Profiler.BeginSample("Generate Sprite From Shader");
            texture2D = GenerateCellGridTextureShader(subInventory.Size, drawSettings);
            Profiler.EndSample();
        }
        //Set Component Values
        RawImage imageComponent = subInventoryObject.GetComponent<RawImage>();
        imageComponent.texture = texture2D;

        LayoutElement layoutElement = subInventoryObject.GetComponent<LayoutElement>();
        layoutElement.minWidth = texture2D.width;
        layoutElement.minHeight = texture2D.height;


        RectTransform rectTransform = subInventoryObject.GetComponent<RectTransform>(); 
        rectTransform.localScale = Vector3.one;
        rectTransform.pivot = Vector2.up;

        return subInventoryObject;
    }

    private static Texture2D GenerateCellGridTextureShader(Vector2Int gridSize, InventoryCellDrawSettings drawSettings)
    {
        Vector2Int textureSize = new(
        gridSize.x * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2,
        gridSize.y * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2
        );

        RenderTexture renderTexture = new RenderTexture(textureSize.x, textureSize.y, 32);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        if (computeShader == null) { throw new System.NullReferenceException("computeShader is null"); }

        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetInts("textureSize", new int[] { renderTexture.width, renderTexture.height });
        computeShader.Dispatch(0, renderTexture.width, renderTexture.height, 1);

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        Graphics.CopyTexture(renderTexture, 0, 0, 0, 0, renderTexture.width, renderTexture.height, texture, 0, 0, 0, 0);
        texture.filterMode = FilterMode.Trilinear;
        //Profiler.BeginSample("Create Sprite");
        //Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        //sprite.name = $"{gridSize.x}x{gridSize.y} SubInventory";
        //Profiler.EndSample();

        CAHCED_SPRITES.Add(gridSize, texture);

        return texture;
    }

    private static Texture2D GenerateCellGridTexture(Vector2Int gridSize, in InventoryCellDrawSettings drawSettings)
    {
        Vector2Int textureSize = new(
        gridSize.x * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2,
        gridSize.y * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2
        );

        Texture2D texture = new Texture2D(textureSize.x, textureSize.y);

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