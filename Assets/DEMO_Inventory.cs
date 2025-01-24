using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DEMO_Inventory : MonoBehaviour
{
    GraphicRaycaster m_Raycaster;
    List<RaycastResult> raycastResults = new();

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

                ItemBaseSO itemSO = ItemDB.GetObjectByName("Test Rig 1");
                bool isRotated = Input.GetKey(KeyCode.LeftShift);
                subInventoryUI.SubInventory.TryAddItem(itemSO.CreateItem(), targetGridCoordinate, isRotated);
            }
        }
    }
}
