using UnityEngine;
using UnityEngine.UI;

public static class DragItemUI
{
    private static InventoryCellDrawSettings DRAW_SETTINGS;

    private static ItemBase ITEM;
    private static bool IS_ROTATED;
    private static Vector2Int ITEM_SIZE { get => IS_ROTATED ? new Vector2Int(ITEM.Data.Size.y, ITEM.Data.Size.x) : ITEM.Data.Size; }

    private static GameObject DRAGGED_BACKGROUND;
    private static GameObject DRAGGED_ICON;

    private static Color VALID_PLACEMENT = new Color(0,1,0,.25f);
    private static Color INVALID_PLACEMENT = new Color(1, 0, 0, .25f);


    public static void Init(InventoryCellDrawSettings drawSettings)
    {
        DRAW_SETTINGS = drawSettings;

        if (DRAGGED_BACKGROUND != null) { GameObject.Destroy(DRAGGED_BACKGROUND); }
        if (DRAGGED_ICON != null) { GameObject.Destroy(DRAGGED_ICON); }

        DRAGGED_BACKGROUND = new GameObject("DraggedItemBackground", typeof(Image));
        DRAGGED_BACKGROUND.transform.SetParent(GameObject.FindAnyObjectByType<Canvas>().transform);
        DRAGGED_BACKGROUND.GetComponent<RectTransform>().anchorMin = Vector2.up;
        DRAGGED_BACKGROUND.GetComponent<RectTransform>().anchorMax = Vector2.up;
        DRAGGED_BACKGROUND.SetActive(false);

        DRAGGED_ICON = new GameObject("DraggedItemIcon", typeof(Image));
        DRAGGED_ICON.transform.SetParent(GameObject.FindAnyObjectByType<Canvas>().transform);
        DRAGGED_ICON.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
        DRAGGED_ICON.GetComponent<Image>().preserveAspect = true;
        DRAGGED_ICON.SetActive(false);
    }

    public static void OnDrag(SubInventoryUI hoveredSubInventory = null)
    {
        DRAGGED_ICON.GetComponent<RectTransform>().position = Input.mousePosition;
        if (hoveredSubInventory != null)
        {
            DRAGGED_BACKGROUND.GetComponent<RectTransform>().position = hoveredSubInventory.ScreenPositionToGridSnapScreenPosition((Vector2)Input.mousePosition - SubInventoryUI.SlotAndItemCenteringOffset(ITEM_SIZE, DRAW_SETTINGS) * hoveredSubInventory.GetComponentInParent<Canvas>().scaleFactor, ITEM_SIZE);
            SetBackgroundColor(hoveredSubInventory);
            DRAGGED_BACKGROUND.SetActive(true);
        }
        else
        {
            DRAGGED_BACKGROUND.SetActive(false);
        }
    }

    public static void BeginDrag(ItemBase item)
    {
        ITEM = item;
        IS_ROTATED = ITEM.IsRotated;

        DRAGGED_BACKGROUND.transform.SetAsLastSibling();
        DRAGGED_BACKGROUND.transform.eulerAngles = new Vector3(0, 0, 0);
        DRAGGED_BACKGROUND.GetComponent<RectTransform>().sizeDelta = ITEM_SIZE * DRAW_SETTINGS._cellSize;
        DRAGGED_BACKGROUND.GetComponent<RectTransform>().pivot = Vector2.up;

        DRAGGED_ICON.transform.SetAsLastSibling();
        DRAGGED_ICON.transform.eulerAngles = IS_ROTATED ? new Vector3(0, 0, 90) : new Vector3(0, 0, 0);
        DRAGGED_ICON.GetComponent<RectTransform>().sizeDelta = ITEM.Data.Size * DRAW_SETTINGS._cellSize;
        DRAGGED_ICON.GetComponent<Image>().sprite = ITEM.Data.Icon;
        DRAGGED_ICON.SetActive(true);
    }
    public static void EndDrag()
    {
        DRAGGED_BACKGROUND.SetActive(false);
        DRAGGED_ICON.SetActive(false);
    }

    public static void Rotate()
    {
        DRAGGED_BACKGROUND.transform.eulerAngles = DRAGGED_BACKGROUND.transform.eulerAngles.z == 0 ? new Vector3(0, 0, 90) : new Vector3(0, 0, 0);
        DRAGGED_BACKGROUND.GetComponent<RectTransform>().pivot = DRAGGED_BACKGROUND.GetComponent<RectTransform>().pivot == Vector2.up ? Vector2.one : Vector2.up;

        DRAGGED_ICON.transform.eulerAngles = DRAGGED_ICON.transform.eulerAngles.z == 0 ? new Vector3(0, 0, 90) : new Vector3(0, 0, 0);

        IS_ROTATED = !IS_ROTATED;
        OnDrag();
    }

    private static void SetBackgroundColor(SubInventoryUI targetSubInventoryUI)
    {
        Vector2Int targetCoord = targetSubInventoryUI.GridCoordinateFromScreenPosition((Vector2)Input.mousePosition - SubInventoryUI.SlotAndItemCenteringOffset(ITEM_SIZE, DRAW_SETTINGS) * targetSubInventoryUI.GetComponentInParent<Canvas>().scaleFactor);
        Vector2Int maxClamp = new Vector2Int(targetSubInventoryUI.SubInventory.Size.x - ITEM_SIZE.x, targetSubInventoryUI.SubInventory.Size.y - ITEM_SIZE.y);
        targetCoord.Clamp(Vector2Int.zero, maxClamp);
        DRAGGED_BACKGROUND.GetComponent<Image>().color = targetSubInventoryUI.SubInventory.CanAddItem(ITEM, targetCoord, IS_ROTATED) ? VALID_PLACEMENT : INVALID_PLACEMENT;
    }
}