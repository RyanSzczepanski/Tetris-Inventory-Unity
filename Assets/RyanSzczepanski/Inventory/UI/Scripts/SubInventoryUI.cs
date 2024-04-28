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

    public void Init(SubInventory subInventory)
    {
        SubInventory = subInventory;
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
