using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SubInventoryUIGenerator
{
    public static Dictionary<Vector2Int, Sprite> CAHCED_SPRITES = new Dictionary<Vector2Int, Sprite>();

    private readonly SubInventoryUITextureGenerator textureGenerator;
    private readonly SubInventory subInventory;

    public SubInventoryUIGenerator(InventoryCellDrawSettingsSO drawSettings, SubInventory subInventory)
    {
        this.subInventory = subInventory;
        textureGenerator = new(drawSettings);
    }

    public GameObject GenerateSubInventoryObject(Transform parent)
    {
        //Rename Object
        GameObject subInventoryObject = new GameObject($"{subInventory.Size.x}x{subInventory.Size.y} SubInventory");
        subInventoryObject.transform.SetParent(parent, false);
        //TODO: Remove this and replace with required components in SubInventoryUI
        subInventoryObject.AddComponent<Image>();
        //subInventoryObject.AddComponent<SubInventoryUI>().Init(subInventory);

        Sprite sprite = LookUpOrGenerateSprite(subInventory.Size);

        //Set Component Values
        Image imageComponent = subInventoryObject.GetComponent<Image>();
        imageComponent.sprite = sprite;

        RectTransform rectTransform = subInventoryObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = sprite.texture.Size();  
        rectTransform.localScale = Vector3.one;
        rectTransform.pivot = Vector2.up;

        return subInventoryObject;
    }
    private Sprite LookUpOrGenerateSprite(Vector2Int key)
    {
        Sprite sprite;
        if (!CAHCED_SPRITES.TryGetValue(key, out sprite))
        {
            sprite = GenerateSprite(key);
            CAHCED_SPRITES.Add(subInventory.Size, sprite);
        }
        return sprite;
    }
    private Sprite GenerateSprite(Vector2Int size)
    {
        Texture2D texture2D = textureGenerator.GenerateCellGridTexture(size);
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
        sprite.name = $"{subInventory.Size.x}x{subInventory.Size.y} SubInventory";
        return sprite;
    }
}


public class SubInventoryUITextureGenerator
{
    private readonly InventoryCellDrawSettingsSO drawSettings;
    public SubInventoryUITextureGenerator(InventoryCellDrawSettingsSO drawSettings)
    {
        this.drawSettings = drawSettings;
    }

    public Texture2D GenerateCellGridTexture(Vector2Int gridSize)
    {
        Vector2Int textureSize = new(
        gridSize.x * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2,
        gridSize.y * drawSettings._cellSize + drawSettings._paddingSize * 2 + drawSettings._outlineSize * 2
        );
        Color32[] color = new Color32[textureSize.x * textureSize.y];
        Texture2D texture = new(textureSize.x, textureSize.y);

        for (int y = 0; y < textureSize.y; y++)
        {
            for (int x = 0; x < textureSize.x; x++)
            {
                color[y * textureSize.x + x] = CalculatePixelColor(x, y, textureSize);
            }
        }

        texture.SetPixels32(color);
        texture.Apply();
        return texture;
    }
    private Color CalculatePixelColor(int x, int y, Vector2Int textureSize)
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