using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class DragItemUI
{
    private static InventoryCellDrawSettings DRAW_SETTINGS;

    private static ItemBase ITEM;
    private static bool IS_ROTATED;
    private static Vector2Int ITEM_SIZE { get => IS_ROTATED ? new Vector2Int(ITEM.Data.size.y, ITEM.Data.size.x) : ITEM.Data.size; }

    private static GameObject DEBUG;

    private static GameObject BACKGROUND;
    private static GameObject ICON;

    private static Color VALID_PLACEMENT = new Color(0,1,0,.25f);
    private static Color INVALID_PLACEMENT = new Color(1, 0, 0, .25f);


    public static void Init(InventoryCellDrawSettings drawSettings)
    {
        DRAW_SETTINGS = drawSettings;

        if (BACKGROUND != null) { GameObject.Destroy(BACKGROUND); }
        if (ICON != null) { GameObject.Destroy(ICON); }

        BACKGROUND = new GameObject("DraggedItemBackground", typeof(Image));
        BACKGROUND.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
        BACKGROUND.GetComponent<RectTransform>().anchorMin = Vector2.up;
        BACKGROUND.GetComponent<RectTransform>().anchorMax = Vector2.up;
        BACKGROUND.SetActive(false);

        ICON = new GameObject("DraggedItemIcon", typeof(Image));
        ICON.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
        ICON.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
        ICON.GetComponent<Image>().preserveAspect = true;
        ICON.SetActive(false);

        DEBUG = new GameObject("DEBUG", typeof(Image));
        DEBUG.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
        DEBUG.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
        DEBUG.GetComponent<RectTransform>().sizeDelta = new Vector2(10,10);
        DEBUG.GetComponent<Image>().color = Color.red;

    }

    public static void OnDrag(SubInventoryUI hoveredSubInventory = null)
    {
        ICON.GetComponent<RectTransform>().position = Input.mousePosition;
        if (hoveredSubInventory != null)
        {
            DEBUG.GetComponent<RectTransform>().position = (Vector2)Input.mousePosition - SubInventoryUI.SlotAndItemCenteringOffset(ITEM_SIZE, DRAW_SETTINGS) * hoveredSubInventory.GetComponentInParent<Canvas>().scaleFactor;

            BACKGROUND.GetComponent<RectTransform>().position = hoveredSubInventory.ScreenPositionToGridSnapScreenPosition((Vector2)Input.mousePosition - SubInventoryUI.SlotAndItemCenteringOffset(ITEM_SIZE, DRAW_SETTINGS) * hoveredSubInventory.GetComponentInParent<Canvas>().scaleFactor, ITEM_SIZE);
            SetBackgroundColor(hoveredSubInventory);
            BACKGROUND.SetActive(true);
        }
        else
        {
            BACKGROUND.SetActive(false);
        }
    }

    public static void BeginDrag(ItemBase item)
    {
        ITEM = item;
        IS_ROTATED = ITEM.IsRotated;

        BACKGROUND.transform.SetAsLastSibling();
        BACKGROUND.transform.eulerAngles = new Vector3(0, 0, 0);
        BACKGROUND.GetComponent<RectTransform>().sizeDelta = ITEM_SIZE * DRAW_SETTINGS._cellSize;
        BACKGROUND.GetComponent<RectTransform>().pivot = Vector2.up;

        ICON.transform.SetAsLastSibling();
        ICON.transform.eulerAngles = IS_ROTATED ? new Vector3(0, 0, 90) : new Vector3(0, 0, 0);
        ICON.GetComponent<RectTransform>().sizeDelta = ITEM.Data.size * DRAW_SETTINGS._cellSize;
        ICON.GetComponent<Image>().sprite = ITEM.Data.icon;
        ICON.SetActive(true);

        DEBUG.transform.SetAsLastSibling();
    }
    public static void EndDrag()
    {
        BACKGROUND.SetActive(false);

        ICON.SetActive(false);
    }

    public static void Rotate()
    {
        BACKGROUND.transform.eulerAngles = BACKGROUND.transform.eulerAngles.z == 0 ? new Vector3(0, 0, 90) : new Vector3(0, 0, 0);
        BACKGROUND.GetComponent<RectTransform>().pivot = BACKGROUND.GetComponent<RectTransform>().pivot == Vector2.up ? Vector2.one : Vector2.up;

        ICON.transform.eulerAngles = ICON.transform.eulerAngles.z == 0 ? new Vector3(0, 0, 90) : new Vector3(0, 0, 0);

        IS_ROTATED = !IS_ROTATED;
        OnDrag();
    }

    private static void SetBackgroundColor(SubInventoryUI targetSubInventoryUI)
    {
        Vector2Int targetCoord = targetSubInventoryUI.GridCoordinateFromScreenPosition((Vector2)Input.mousePosition - SubInventoryUI.SlotAndItemCenteringOffset(ITEM_SIZE, DRAW_SETTINGS) * targetSubInventoryUI.GetComponentInParent<Canvas>().scaleFactor);
        Vector2Int maxClamp = new Vector2Int(targetSubInventoryUI.SubInventory.Size.x - ITEM_SIZE.x, targetSubInventoryUI.SubInventory.Size.y - ITEM_SIZE.y);
        targetCoord.Clamp(Vector2Int.zero, maxClamp);
        if (targetSubInventoryUI.SubInventory.CanMoveItem(ITEM, targetSubInventoryUI.SubInventory, targetCoord, IS_ROTATED))
        {
            BACKGROUND.GetComponent<Image>().color = VALID_PLACEMENT;
        }
        else
        {
            BACKGROUND.GetComponent<Image>().color = INVALID_PLACEMENT;
        }
    }
}