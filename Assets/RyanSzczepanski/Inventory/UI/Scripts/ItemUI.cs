using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ItemUI : MonoBehaviour
{
    private static GameObject ITEM_PREFAB;
    private static InventoryCellDrawSettings DRAW_SETTINGS;

    public ItemBase Item { get; private set; }
    [SerializeField] private RectTransform icon;
    [SerializeField] private RectTransform background;


    public static ItemUI Generate(ItemBase item, SubInventoryUI subInventoryUI, Vector2Int targetCoord)
    {
        ITEM_PREFAB ??= Resources.Load<GameObject>("Prefab/ItemUI");
        GameObject itemUIObject = Instantiate(ITEM_PREFAB, subInventoryUI.transform);
        ItemUI itemUI = itemUIObject.GetComponent<ItemUI>();
        itemUI.Init(item, subInventoryUI.drawSettings);
        itemUI.MoveTo(subInventoryUI, targetCoord);
        return itemUI;
    }

    private void Init(ItemBase item, InventoryCellDrawSettings drawSettings)
    {
        Item = item;
        DRAW_SETTINGS = drawSettings;
        icon.GetComponent<Image>().sprite = Item.Data.Icon;

        Subscibe();

        transform.GetComponent<RectTransform>().sizeDelta = Item.Data.Size * DRAW_SETTINGS._cellSize;
        GetComponent<RectTransform>().anchorMin = Vector2.up;
        GetComponent<RectTransform>().anchorMax = Vector2.up;

        transform.eulerAngles = new Vector3(0, 0, 90 * Convert.ToInt32(Item.IsRotated));
    }
    public void MoveTo(SubInventoryUI targetSubInventoryUI, Vector2Int targetCoords)
    {
        transform.SetParent(targetSubInventoryUI.transform);
        GetComponent<RectTransform>().pivot = Item.IsRotated ? Vector2.one : Vector2.up;
        GetComponent<RectTransform>().anchoredPosition =
            new Vector2
                ((targetCoords.x * DRAW_SETTINGS._cellSize) + (DRAW_SETTINGS._outlineSize * 2),
                ((targetCoords.y * DRAW_SETTINGS._cellSize) + (DRAW_SETTINGS._outlineSize * 2)) * -1);
    }
    public void SetBackgroundActive(bool isActive)
    {
        background.gameObject.SetActive(isActive);
    }


    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscibe()
    {
        Item.ItemRemoved += OnItemRemoved;
        Item.ItemRotated += OnItemRotated;
    }
    private void Unsubscribe()
    {
        Item.ItemRemoved -= OnItemRemoved;
        Item.ItemRotated -= OnItemRotated;

    }

    public void OnItemRemoved(object sender, ItemRemovedEventArgs args)
    {
        Destroy(this.gameObject);
    }
    public void OnItemRotated(object sender, bool IsRotated)
    {
        GetComponent<RectTransform>().pivot = IsRotated ? Vector2.one : Vector2.up;
    }
}
