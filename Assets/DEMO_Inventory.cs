using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DEMO_Inventory : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    List<RaycastResult> raycastResults = new();

    [SerializeField] ItemBaseSO newItem;

    private void Awake()
    {
        m_Raycaster = FindAnyObjectByType<GraphicRaycaster>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            raycastResults.Clear();
            PointerEventData m_PointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            m_Raycaster.Raycast(m_PointerData, raycastResults);

            if (raycastResults.Count <= 0) { return; }
            if (raycastResults[0].gameObject.TryGetComponent(out SubInventoryUI subInventoryUI))
            {
                var targetGridCoordinate = subInventoryUI.GridCoordinateFromScreenPosition(m_PointerData.position);

                newItem ??= ItemDB.GetObjectByName("Test Rig 1");
                bool isRotated = Input.GetKey(KeyCode.LeftShift);
                subInventoryUI.SubInventory.TryAddItem(newItem.CreateItem(), targetGridCoordinate, isRotated);
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            raycastResults.Clear();
            PointerEventData m_PointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            m_Raycaster.Raycast(m_PointerData, raycastResults);

            if (raycastResults.Count <= 0) { return; }
            if (raycastResults[0].gameObject.TryGetComponent(out SubInventoryUI subInventoryUI))
            {
                var targetGridCoordinate = subInventoryUI.GridCoordinateFromScreenPosition(m_PointerData.position);

                bool isRotated = Input.GetKey(KeyCode.LeftShift);
                
                SubInventory targetSubInv = subInventoryUI.SubInventory;
                ItemBase tempNewItem = null;
                for (int i = 0; i < 5; i++)
                {
                    tempNewItem = ItemDB.GetObjectByName("T20 Backpack").CreateItem();
                    targetSubInv.TryAddItem(tempNewItem, targetGridCoordinate, false);
                    targetGridCoordinate = new Vector2Int(0, 0);
                    targetSubInv = (tempNewItem as IInventory).Inventory.SubInventories[0];
                }
                IInventory.OpenUI(tempNewItem, InventoryUIManager.CANVAS.transform);
            }
        }
    }
}
