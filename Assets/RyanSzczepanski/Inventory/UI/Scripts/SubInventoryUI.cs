using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SubInventoryUI : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public SubInventory SubInventory { get; private set; }
    public InventoryCellDrawSettingsSO drawSettingsSO;

    public void Init(SubInventory subInventory, InventoryCellDrawSettingsSO drawSettingsSO)
    {
        SubInventory = subInventory;
        this.drawSettingsSO = drawSettingsSO;
        Subscribe();
        foreach (Slot slot in  SubInventory.Slots)
        {
            if (slot.IsOccupied)
            {
                //ItemUI.InitUI(slot.ItemInSlot, this);
            }
        }
    }

    private void Update()
    {
        if (isDraggingItem)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                rotateItem = !rotateItem;
            }
        }
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
        ItemUI.Init(args.Item, this, args.TargetCellCoordinate);

    }
    public void OnItemMoved(object source, SubInventoryItemMovedEventArgs args)
    {
        
    }
    public void OnItemRemoved(object source, SubInventoryItemRemovedEventArgs args)
    {

    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.dragging) { return; }
        Vector2Int targetGridCoordinate = GridCoordinateFromScreenPosition(eventData.position);
        Slot slot = SubInventory.Slots[targetGridCoordinate.x, targetGridCoordinate.y];
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                Item item = new Item(new Vector2Int(1, 2));
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    item.Rotate();
                }
                SubInventory.TryAddItem(item, targetGridCoordinate);

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
                        Debug.Log($"IsRotated: {SubInventory.Slots[targetGridCoordinate.x, targetGridCoordinate.y].ItemInSlot.IsRotated}, Size: {SubInventory.Slots[targetGridCoordinate.x, targetGridCoordinate.y].ItemInSlot.Size}");
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
    Item draggedItem;
    Vector2Int originGridCoordinate;
    bool originRotatedStatus;
    bool rotateItem;
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggingItem) { return; }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originGridCoordinate = GridCoordinateFromScreenPosition(eventData.pressPosition);
        draggedItem = SubInventory.Slots[originGridCoordinate.x, originGridCoordinate.y].ItemInSlot;
        if (draggedItem == null) { return; }
        isDraggingItem = true;
        originRotatedStatus = draggedItem.IsRotated;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggingItem) { return; }

        //Raycast for TargetSubInventory
        SubInventoryUI targetSubInventoryUI = null;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        RaycastResult result = results.Find(r => r.gameObject.GetComponent<SubInventoryUI>());
        if (!result.isValid) { return; }
        SubInventoryUI targetSubInventoryUI = result.gameObject.GetComponent<SubInventoryUI>();


        Vector2Int targetCoordinate = targetSubInventoryUI.GridCoordinateFromScreenPosition(eventData.position);

        if (rotateItem) { draggedItem.Rotate(); }

        SubInventory.TryMoveItem(draggedItem, targetSubInventoryUI.SubInventory, targetCoordinate, originGridCoordinate, originRotatedStatus);

        rotateItem = false;
        isDraggingItem = false;
    }

    private Vector2 LocalPositionFromScreenPosition(Vector2 screenPosition)
    {
        Vector2 localPosition = new Vector2(screenPosition.x - transform.position.x, transform.position.y - screenPosition.y) / GetComponentInParent<Canvas>().transform.localScale;
        
        return localPosition;
    }
    private Vector2Int GridCoordinateFromLocalPosition(Vector2 localPosition)
    {
        //TODO: Remove search for drawsettings with DI
        InventoryCellDrawSettingsSO settings = Resources.Load<InventoryCellDrawSettingsSO>("SubInventory UI Draw Settings");

        return new Vector2Int(
            Mathf.Clamp(
                Mathf.FloorToInt((localPosition.x - settings._paddingSize - settings._outlineSize) / (settings._cellSize)),
                0,
                SubInventory.Size.x - 1
            ),
            Mathf.Clamp(
                Mathf.FloorToInt((localPosition.y - settings._paddingSize - settings._outlineSize) / (settings._cellSize)),
                0,
                SubInventory.Size.y - 1
            )
        );
    }
    private Vector2Int GridCoordinateFromScreenPosition(Vector2 screenPosition)
    {
        return GridCoordinateFromLocalPosition(LocalPositionFromScreenPosition(screenPosition));
    }
}
