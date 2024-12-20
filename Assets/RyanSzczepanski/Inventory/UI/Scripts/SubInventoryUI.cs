using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(LayoutElement))]
public class SubInventoryUI : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public SubInventory SubInventory { get; private set; }
    public InventoryCellDrawSettings drawSettings;

    public delegate void OnDragBeginHandler(object sender);
    public delegate void OnDragEndHandler(object sender);
    public event OnDragBeginHandler DragBegin;
    public event OnDragEndHandler DragEnd;


    public void Init(SubInventory subInventory, InventoryCellDrawSettings drawSettings)
    {
        SubInventory = subInventory;
        this.drawSettings = drawSettings;
        Subscribe();

        List<ItemBase> items = new List<ItemBase>();
        for (int y = 0; y < subInventory.Slots.GetLength(1); y++)
        {
            for (int x = 0; x < subInventory.Slots.GetLength(0); x++)
            {
                if (!subInventory.Slots[x, y].IsOccupied) { continue; }
                if (items.Contains(subInventory.Slots[x, y].ItemInSlot)) { continue; }
                items.Add(subInventory.Slots[x, y].ItemInSlot);
                ItemUI.Generate(subInventory.Slots[x, y].ItemInSlot, this, new Vector2Int(x, y));
            }
        }
    }

    private void Update()
    {
        if (isDraggingItem)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isRotated = !isRotated;
                DragItemUI.Rotate();
            }
        }
    }
    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        SubInventory.ItemAdded += OnItemAdded;
        SubInventory.ItemMoved += OnItemMoved;
        SubInventory.ItemRemoved += OnItemRemoved;
    }
    private void Unsubscribe()
    {
        SubInventory.ItemAdded -= OnItemAdded;
        SubInventory.ItemMoved -= OnItemMoved;
        SubInventory.ItemRemoved -= OnItemRemoved;
    }

    public void OnItemAdded(object source, SubInventoryItemAddedEventArgs args)
    {
        ItemUI.Generate(args.Item, this, args.TargetCellCoordinate);

    }
    public void OnItemMoved(object source, SubInventoryItemMovedEventArgs args)
    {
        
    }
    public void OnItemRemoved(object source, SubInventoryItemRemovedEventArgs args)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging) { return; }
        Vector2Int targetGridCoordinate = GridCoordinateFromScreenPosition(eventData.position);
        Slot slot = SubInventory.Slots[targetGridCoordinate.x, targetGridCoordinate.y];
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:

                break;
            case PointerEventData.InputButton.Middle:
                if (slot.IsOccupied)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        Debug.Log($"{slot.ItemInSlot.GetHashCode()}");
                    }
                    else
                    {
                        ItemBase itemInSlot = SubInventory.Slots[targetGridCoordinate.x, targetGridCoordinate.y].ItemInSlot;
                        string log = $"{itemInSlot}\n   Origin Coordinate: {SubInventory.GetItemOriginSlot(itemInSlot)}";
                        Debug.Log(log);
                    }
                }
                else
                {
                    Debug.Log($"Empty");
                }

                break;
            case PointerEventData.InputButton.Right:
                if (slot.IsOccupied)
                {
                    SubInventory.TryRemoveItem(slot.ItemInSlot);
                }
                break;
        }
    }

    bool isDraggingItem;

    Vector2Int originGridCoordinate;
    bool originRotatedStatus;
    bool isRotated;

    ItemBase targetItem;
    ItemUI targetItemUI;


    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingItem) { return; }
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        RaycastResult result = results.Find(r => r.gameObject.GetComponent<SubInventoryUI>());
        if (!result.isValid) { DragItemUI.OnDrag(); return; }
        DragItemUI.OnDrag(result.gameObject.GetComponent<SubInventoryUI>());
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }
        Vector2Int clickGridCoordinate = GridCoordinateFromScreenPosition(eventData.pressPosition);
        targetItem = SubInventory.Slots[clickGridCoordinate.x, clickGridCoordinate.y].ItemInSlot;

        if (targetItem == null) { return; }

        //TODO: Better way of getting origin grid coordinate?
        isRotated = targetItem.IsRotated;
        originGridCoordinate = SubInventory.GetItemOriginSlot(targetItem);

        DragItemUI.BeginDrag(targetItem);

        targetItemUI = eventData.pointerPressRaycast.gameObject.GetComponentInParent<ItemUI>();
        targetItemUI?.SetBackgroundActive(false);

        isDraggingItem = true;
        originRotatedStatus = targetItem.IsRotated;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingItem) { return; } 

        //Raycast for TargetSubInventory
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        RaycastResult result = results.Find(r => r.gameObject.GetComponent<SubInventoryUI>());
        if (!result.isValid)
        {
            DragItemUI.EndDrag();
            return;
        }
        SubInventoryUI targetSubInventoryUI = result.gameObject.GetComponent<SubInventoryUI>();

        Vector2Int itemSize = isRotated ? new Vector2Int(targetItem.Data.Size.y, targetItem.Data.Size.x) : targetItem.Data.Size;

        Vector2Int targetCoordinate = targetSubInventoryUI.GridCoordinateFromScreenPosition(eventData.position - SlotAndItemCenteringOffset(itemSize, drawSettings) * GetComponentInParent<Canvas>().scaleFactor);
        targetCoordinate.Clamp(Vector2Int.zero, new Vector2Int(SubInventory.Size.x - itemSize.x, SubInventory.Size.y - itemSize.y));

        SubInventory.TryMoveItem(targetItem, targetSubInventoryUI.SubInventory, targetCoordinate, isRotated, originGridCoordinate, originRotatedStatus);

        DragItemUI.EndDrag();

        targetItemUI?.SetBackgroundActive(false);

        targetItem = null;
        isRotated = false;
        isDraggingItem = false;
    }

    public Vector2 LocalPositionFromScreenPosition(Vector2 screenPosition)
    {
        return new Vector2(screenPosition.x - transform.position.x, transform.position.y - screenPosition.y) / GetComponentInParent<Canvas>().transform.localScale;
    }
    public Vector2 ScreenPositionFromLocalPosition(Vector2 localPosition)
    {
        localPosition *= GetComponentInParent<Canvas>().transform.localScale;
        return new Vector2(localPosition.x + transform.position.x, -(localPosition.y - transform.position.y));
    }

    public Vector2Int GridCoordinateFromLocalPosition(Vector2 localPosition)
    {
        return new Vector2Int(
            Mathf.Clamp(
                Mathf.FloorToInt((localPosition.x - drawSettings._paddingSize - drawSettings._outlineSize) / (drawSettings._cellSize)),
                0,
                SubInventory.Size.x - 1
            ),
            Mathf.Clamp(
                Mathf.FloorToInt((localPosition.y - drawSettings._paddingSize - drawSettings._outlineSize) / (drawSettings._cellSize)),
                0,
                SubInventory.Size.y - 1
            )
        );
    }
    public Vector2Int GridCoordinateFromScreenPosition(Vector2 screenPosition)
    {
        return GridCoordinateFromLocalPosition(LocalPositionFromScreenPosition(screenPosition));
    }

    public Vector2 LocalPositionFromGridCoordinate(Vector2Int gridCoordinate)
    {
        return new Vector2(gridCoordinate.x * drawSettings._cellSize + drawSettings._paddingSize + drawSettings._outlineSize,
            gridCoordinate.y * drawSettings._cellSize + drawSettings._paddingSize + drawSettings._outlineSize);
    }
    public Vector2 ScreenPositionFromGridCoordinate(Vector2Int gridCoordinate)
    {
        return ScreenPositionFromLocalPosition(LocalPositionFromGridCoordinate(gridCoordinate));
    }

    public Vector2 ScreenPositionToGridSnapScreenPosition(Vector2 screenPosition, Vector2Int itemSize)
    {
        Vector2Int gridCoord = GridCoordinateFromScreenPosition(screenPosition);
        gridCoord.Clamp(Vector2Int.zero, new Vector2Int(SubInventory.Size.x - itemSize.x, SubInventory.Size.y - itemSize.y));
        return ScreenPositionFromGridCoordinate(gridCoord);
    }

    public static Vector2 ItemCenteringOffset(Vector2Int itemSize, InventoryCellDrawSettings drawSettings)
    {
        return
            (drawSettings._cellSize * 0.5f * new Vector2(itemSize.x, -itemSize.y));
    }
    public static Vector2 SlotCenteringOffset(Vector2 originPosition, InventoryCellDrawSettings drawSettings)
    {
        return originPosition - new Vector2(drawSettings._cellSize / 2, -drawSettings._cellSize / 2);
    }
    public static Vector2 SlotAndItemCenteringOffset(Vector2Int itemSize, InventoryCellDrawSettings drawSettings)
    {
        return
            ((drawSettings._cellSize * 0.5f * new Vector2(itemSize.x, -itemSize.y)) -
            new Vector2(drawSettings._cellSize / 2, -drawSettings._cellSize / 2));
    }
}
