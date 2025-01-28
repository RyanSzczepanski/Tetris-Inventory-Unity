using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Timeline;
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

        INVENTORY_PREFAB.hideFlags = HideFlags.HideAndDontSave;
        HORIZONTAL_ARRANGEMENT_PREFAB.hideFlags = HideFlags.HideAndDontSave;
        VERTICAL_ARRANGEMENT_PREFAB.hideFlags = HideFlags.HideAndDontSave;
    }

    public static void PreJIT(InventoryCellDrawSettings DRAW_SETTINGS)
    {
        ItemInventorySO itemInventorySO = ScriptableObject.CreateInstance<ItemInventorySO>();
        var field = typeof(ItemInventorySO).GetField("m_SubInventoryArrangements", BindingFlags.Instance | BindingFlags.NonPublic);
        field.SetValue(itemInventorySO, new SubInventoryArrangement() { subInventorySize = new Vector2Int(1, 1), childArrangements = new SubInventoryArrangement[0] });
        ItemInventory item = itemInventorySO.CreateItem() as ItemInventory;
        GameObject.Destroy(GenerateUIObject(null, item.Data, item, DRAW_SETTINGS));

        foreach (var method in typeof(InventoryUIGenerator).GetMethods(
            BindingFlags.DeclaredOnly |
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static))
        {
            Debug.Log($"JIT Compile InventoryUIGenerator.{method.Name}");
            RuntimeHelpers.PrepareMethod(method.MethodHandle);
        }
        foreach (var method in typeof(SubInventoryUIGenerator).GetMethods(
            BindingFlags.DeclaredOnly |
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.Static))
        {
            Debug.Log($"JIT Compile SubInventoryUIGenerator.{method.Name}");
            RuntimeHelpers.PrepareMethod(method.MethodHandle);
        }
    }

    public static GameObject GenerateUIObject(Transform transform, IInventorySO itemInventorySO, IInventory itemInventory, in InventoryCellDrawSettings drawSettings)
    {
        Profiler.BeginSample("GenerateUIObject");
        subInventoryTracker = 0;
        InventoryUIGenerator.drawSettings = drawSettings;
        GameObject inventoryObject = GameObject.Instantiate(INVENTORY_PREFAB, transform);
        inventoryObject.hideFlags = HideFlags.None;

        HorizontalOrVerticalLayoutGroup layoutGroup = inventoryObject.GetComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;

        ContentSizeFitter csf = inventoryObject.GetComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        csf.verticalFit = ContentSizeFitter.FitMode.MinSize;
        
        ArrangementTreeSearch(itemInventorySO.SubInventoryArrangements, inventoryObject.transform, itemInventory);
        Profiler.EndSample();
        return inventoryObject;
    }
    private static void ArrangementTreeSearch(SubInventoryArrangement arrangement, Transform parent, IInventory item)
    {
        //Profiler.BeginSample("ArrangementTreeSearch");
        Transform newParent;
        if (arrangement.HasSubInventory)
        {
            GameObject subInventoryUIObject = SubInventoryUIGenerator.GenerateSubInventoryObject(item.Inventory.SubInventories[subInventoryTracker], parent, in drawSettings);
            newParent = subInventoryUIObject.transform;
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
        //Profiler.EndSample();
    }
    private static GameObject GenerateArrangement(SubInventoryArrangement arrangement, Transform parent)
    {
        GameObject arrangementObject;
        HorizontalOrVerticalLayoutGroup layoutGroup;
        if (arrangement.direction == GridLayoutGroup.Axis.Vertical)
        {
            arrangementObject = GameObject.Instantiate(VERTICAL_ARRANGEMENT_PREFAB, parent);
            layoutGroup = arrangementObject.GetComponent<VerticalLayoutGroup>();
        }
        else
        {
            arrangementObject = GameObject.Instantiate(HORIZONTAL_ARRANGEMENT_PREFAB, parent);
            layoutGroup = arrangementObject.GetComponent<HorizontalLayoutGroup>();
        }
        arrangementObject.hideFlags = HideFlags.None;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childAlignment = arrangement.alignment;
        layoutGroup.spacing = 5;

        return arrangementObject;
    }
}


public static class SubInventoryUIGenerator
{
    private static Dictionary<Vector2Int, RenderTexture> CAHCED_TEXTURES = new Dictionary<Vector2Int, RenderTexture>();
    private static ComputeShader computeShader;

    private static GameObject SUB_INVENTORY;

    public static void SetShader(ComputeShader shader)
    {
        computeShader = shader;
    }
    public static void PreLoadPrefabs()
    {
        SUB_INVENTORY ??= new GameObject($"SubInventory Prefab", typeof(SubInventoryUI));
        SUB_INVENTORY.hideFlags = HideFlags.HideAndDontSave;
    }

    public static GameObject GenerateSubInventoryObject(SubInventory subInventory, Transform parent, in InventoryCellDrawSettings drawSettings)
    {
        //Rename Object
        GameObject subInventoryObject = GameObject.Instantiate(SUB_INVENTORY, parent);
        subInventoryObject.hideFlags = HideFlags.None;
        SubInventoryUI subInventoryUI = subInventoryObject.GetComponent<SubInventoryUI>();
        subInventoryUI.Init(subInventory, drawSettings);
        //Generate Or Get Cached Texture
        RenderTexture texture2D;
        if(!CAHCED_TEXTURES.TryGetValue(subInventory.Size, out texture2D))
        {
            texture2D = GenerateCellGridTextureShader(subInventory.Size, in drawSettings);
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

    public static RenderTexture GenerateCellGridTextureShader(Vector2Int gridSize, in InventoryCellDrawSettings drawSettings)
    {
        Profiler.BeginSample("GenerateCellGridTextureShader");
        Vector2Int textureSize = new(
        gridSize.x * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2,
        gridSize.y * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2
        );

        RenderTexture renderTexture = new RenderTexture(textureSize.x, textureSize.y, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        if (computeShader == null) { throw new System.NullReferenceException("computeShader is null"); }

        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetInts("textureSize", new int[] { renderTexture.width, renderTexture.height });
        computeShader.Dispatch(0, renderTexture.width, renderTexture.height, 1);

        CAHCED_TEXTURES.Add(gridSize, renderTexture);
        Profiler.EndSample();
        return renderTexture;
    }

    public static Texture2D GenerateCellGridTextureCPU(Vector2Int gridSize, in InventoryCellDrawSettings drawSettings)
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