using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Szczepanski
{
    namespace UI
    {
        //TODO: Resize bar should not directly change the content size and window position but ask the window to move
        public class ResizeBar : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler/*, IPointerUpHandler, IPointerMoveHandler*/
        {
            [SerializeField] private FloatingWindow window;

            [SerializeField] private ResizeBarDirection direction;

            [SerializeField] private Texture2D tex;

            public void SetWindow(FloatingWindow window) => this.window = window;

            public void Init(FloatingWindow window, ResizeBarDirection direction)
            {
                SetWindow(window);
                this.direction = direction;
                string path;
                switch (ResizeBarsEnumUtils.ExpandDirection(this.direction))
                {
                    case Vector2Int v when v.Equals(Vector2Int.up):
                        path = "Cursors/Vertical_Resize";
                        break;
                    case Vector2Int v when v.Equals(Vector2Int.down):
                        path = "Cursors/Vertical_Resize";
                        break;
                    case Vector2Int v when v.Equals(Vector2Int.left):
                        path = "Cursors/Horizontal_Resize";
                        break;
                    case Vector2Int v when v.Equals(Vector2Int.right):
                        path = "Cursors/Horizontal_Resize";
                        break;
                    case Vector2Int v when v.Equals(new Vector2Int(1, 1)):
                        path = "Cursors/Up_Right_Resize";
                        break;
                    case Vector2Int v when v.Equals(new Vector2Int(-1, -1)):
                        path = "Cursors/Up_Right_Resize";
                        break;
                    case Vector2Int v when v.Equals(new Vector2Int(1, -1)):
                        path = "Cursors/Up_Left_Resize";
                        break;
                    case Vector2Int v when v.Equals(new Vector2Int(-1, 1)):
                        path = "Cursors/Up_Left_Resize";
                        break;
                    default:
                        path = string.Empty;
                        break;
                }
                tex = Resources.Load<Texture2D>(path);
            }

            Vector2 startPreferredSize;
            public void OnPointerDown(PointerEventData eventData)
            {
                eventData.dragging = true;
                startPreferredSize = (window.transform as RectTransform).sizeDelta;
            }

            public void OnDrag(PointerEventData eventData)
            {
                Vector2 mouseDelta = (eventData.position - eventData.pressPosition) * ResizeBarsEnumUtils.ExpandDirection(direction);
                mouseDelta /= GetComponentInParent<Canvas>().scaleFactor;
                Vector2 preferredSize = startPreferredSize + mouseDelta;
                window.Resize(preferredSize, ResizeBarsEnumUtils.ExpandDirection(direction));
            }
            public void OnEndDrag(PointerEventData eventData)
            {
                foreach (GameObject go in eventData.hovered)
                {
                    if (go == gameObject)
                    {
                        return;
                    }
                }
                Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                if (!eventData.dragging)
                {
                    Cursor.SetCursor(tex, new Vector2(16, 16), CursorMode.Auto);

                }
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                if (!eventData.dragging)
                {
                    Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto);
                }
            }
        }
    }
}