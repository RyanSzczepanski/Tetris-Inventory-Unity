using Szczepanski.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragBar : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] FloatingWindow window;
    Vector2 offset;
    bool isEnabled;
    bool DoDrag = false;

    public void Init(FloatingWindow window)
    {
        this.window = window;
    }

    public void SetEnabled(bool enabled)
    {
        this.isEnabled = enabled;
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (!DoDrag) { return; }
        window.transform.position = eventData.position + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DoDrag = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!isEnabled) { return; }

        if(null != eventData.pointerCurrentRaycast.gameObject.GetComponent<UnityEngine.UI.Button>())
        {
            DoDrag = false;
            return;
        }
        DoDrag = true;
        eventData.dragging = true;
        offset = (Vector2)window.transform.position - eventData.pressPosition;
    }
}
