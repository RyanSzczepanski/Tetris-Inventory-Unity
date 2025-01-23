using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Szczepanski.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragBar : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] FloatingWindow window;
    Vector2 offset;
    bool DoDrag = false;

    public void Init(FloatingWindow window)
    {
        this.window = window;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!DoDrag) { return; }
        window.SetPosition(eventData.position + offset);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DoDrag = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.pointerCurrentRaycast.gameObject.GetComponent<Button>() != null)
        {
            DoDrag = false;
            return;
        }
        DoDrag = true;
        eventData.dragging = true;
        offset = (Vector2)window.transform.position - eventData.pressPosition;
    }
}
