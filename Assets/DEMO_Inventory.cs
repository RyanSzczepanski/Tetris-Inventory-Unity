using System.Collections;
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
            PointerEventData m_PointerData = new PointerEventData(EventSystem.current);

            m_PointerData.position = Input.mousePosition;

            m_Raycaster.Raycast(m_PointerData, raycastResults);
            foreach (var raycast in raycastResults)
            {
                //if(raycast.gameObject == null) { continue; }
                if (raycast.gameObject.TryGetComponent<SubInventoryUI>(out SubInventoryUI subInventoryUI))
                {
                    var targetGridCoordinate = subInventoryUI.GridCoordinateFromScreenPosition(m_PointerData.position);

                    ItemBaseSO itemSO = ItemDB.GetObjectByName("Basic Item 2x2");
                    bool isRotated = Input.GetKey(KeyCode.LeftShift);
                    subInventoryUI.SubInventory.TryAddItem(itemSO.CreateItem(), targetGridCoordinate, isRotated);
                    break;
                }
            }
        }
    }
}
