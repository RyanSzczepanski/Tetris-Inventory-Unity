using System;
using UnityEngine;
using UnityEngine.UI;

public static class ItemUIFactory
{
    private static GameObject ITEM_PREFAB;

    public static GameObject GenerateUI(ItemBase item, SubInventoryUI subInventoryUI, Vector2Int targetCoordinate)
    {
        ITEM_PREFAB ??= Resources.Load<GameObject>("Prefab/ItemUI");
        GameObject go = GameObject.Instantiate(ITEM_PREFAB);
        ItemUI itemUI = go.GetComponent<ItemUI>();

        go.transform.GetComponent<RectTransform>().sizeDelta = item.Data.Size * InventoryUIManager.DRAW_SETTINGS._cellSize;
        go.GetComponent<RectTransform>().anchorMin = Vector2.up;
        go.GetComponent<RectTransform>().anchorMax = Vector2.up;

        go.transform.eulerAngles = new Vector3(0, 0, 90 * Convert.ToInt32(item.IsRotated));
        itemUI.Init(item);
        itemUI.MoveTo(subInventoryUI, targetCoordinate);
        return go;
    }
}

public class ItemUI : MonoBehaviour
{
    public ItemBase Item { get; private set; }
    [SerializeField] private RectTransform icon;
    [SerializeField] private RectTransform background;

    public void Init(ItemBase item)
    {
        Item = item;
        Subscibe();
        icon.GetComponent<Image>().sprite = item.Data.Icon;
    }
    public void MoveTo(SubInventoryUI targetSubInventoryUI, Vector2Int targetCoords)
    {
        transform.SetParent(targetSubInventoryUI.transform);
        GetComponent<RectTransform>().pivot = Item.IsRotated ? Vector2.one : Vector2.up;
        transform.eulerAngles = new Vector3(0, 0, 90 * Convert.ToInt32(Item.IsRotated));

        GetComponent<RectTransform>().anchoredPosition =
            new Vector2
                ((targetCoords.x * InventoryUIManager.DRAW_SETTINGS._cellSize) + (InventoryUIManager.DRAW_SETTINGS._outlineSize * 2),
                ((targetCoords.y * InventoryUIManager.DRAW_SETTINGS._cellSize) + (InventoryUIManager.DRAW_SETTINGS._outlineSize * 2)) * -1);
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
