using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ItemUI : MonoBehaviour
{
    private static GameObject ITEM_PREFAB;
    private static InventoryCellDrawSettingsSO DRAW_SETTINGS;

    public Item Item { get; private set; }
    private RectTransform spritesTransform;
    private Image sprite;
    private Image background;

    private void Init(Item item, InventoryCellDrawSettingsSO drawSettingsSO)
    {
        Item = item;
        DRAW_SETTINGS = drawSettingsSO;
        spritesTransform = transform.GetChild(0).GetComponent<RectTransform>();
        background = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        sprite = transform.GetChild(0).GetChild(1).GetComponent<Image>();
        Subscibe();
        GetComponent<RectTransform>().sizeDelta = Item.Size * DRAW_SETTINGS._cellSize;
        spritesTransform.eulerAngles = new Vector3(0, 0, 90 * Convert.ToInt32(Item.IsRotated));
        GetComponent<RectTransform>().anchorMin = Vector2.up;
        GetComponent<RectTransform>().anchorMax = Vector2.up;
    }

    public static ItemUI Init(Item item, SubInventoryUI subInventoryUI, Vector2Int targetCoord)
    {
        if(ITEM_PREFAB == null)
        {
            ITEM_PREFAB = Resources.Load<GameObject>("Prefab/ItemUI");
        }
        GameObject itemUIObject = Instantiate(ITEM_PREFAB, subInventoryUI.transform);
        ItemUI itemUI = itemUIObject.GetComponent<ItemUI>();
        itemUI.Init(item, subInventoryUI.drawSettingsSO);
        itemUI.MoveTo(subInventoryUI, targetCoord);
        return itemUI;
    }

    private void MoveTo(SubInventoryUI targetSubInventoryUI, Vector2Int targetCoords)
    {
        transform.SetParent(targetSubInventoryUI.transform);
        GetComponent<RectTransform>().anchoredPosition =
            new Vector2
                ((targetCoords.x * DRAW_SETTINGS._cellSize) + (DRAW_SETTINGS._outlineSize * 2),
                ((targetCoords.y * DRAW_SETTINGS._cellSize) + (DRAW_SETTINGS._outlineSize * 2)) * -1);
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscibe()
    {
        Item.ItemRemoved += OnItemRemoved;
    }
    private void Unsubscribe()
    {
        Item.ItemRemoved -= OnItemRemoved;
    }

    public void OnItemRemoved(object sender, ItemRemovedEventArgs args)
    {
        Destroy(this.gameObject);
    }
}
