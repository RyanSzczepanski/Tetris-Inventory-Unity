using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Szczepanski.UI
{
    public static class FloatingWindowFactory
    {
        public static GameObject CreateFloatingWindow(Transform parent, FloatingWindowSettings settings)
        {
            GameObject go = new GameObject("FloatingWindow", typeof(RectTransform), typeof(FloatingWindow), typeof(VerticalLayoutGroup));
            go.transform.SetParent(parent, false);
            go.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = false;
            go.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;

            if (!settings.isResizeable)
            {
                ContentSizeFitter csf = go.AddComponent<ContentSizeFitter>();
                csf.horizontalFit = ContentSizeFitter.FitMode.MinSize;
                csf.verticalFit = ContentSizeFitter.FitMode.MinSize;
            }

            FloatingWindow floatingWindow = go.GetComponent<FloatingWindow>();
            floatingWindow.GenerateUI(settings);
            return go;
        }
    }

    [System.Serializable]
    public struct FloatingWindowSettings
    {
        public bool isDraggable;
        public bool isResizeable;
        public Vector2 minWindowSize;
        public Vector2 windowSize;
        public string title;
    }

    public struct WindowElement
    {
        public WindowElement(GameObject gameObject)
        {
            this.gameObject = gameObject;
            rectTransform = gameObject.GetComponent<RectTransform>();
            layoutElement = gameObject.GetComponent<LayoutElement>();
        }

        public GameObject gameObject;
        public RectTransform rectTransform;
        public LayoutElement layoutElement;

        public T GetComponent<T>()
        {
            return gameObject.GetComponent<T>();
        }
    }

    public class FloatingWindow : MonoBehaviour
    {
        public WindowElement ResizeBarParent { get; private set; }
        public WindowElement TitleBar { get; private set; }
        public WindowElement Content { get; private set; }
        public bool IsResizeable { get; private set; }
        public bool IsDraggable { get; private set; }
        public string Title { get; private set; }
        public Vector2 MinWindowSize { get; private set; }

        public void GenerateUI(FloatingWindowSettings settings)
        {
            IsDraggable = settings.isDraggable;
            IsResizeable = settings.isResizeable;
            MinWindowSize = settings.minWindowSize;
            Title = settings.title;
            CreateObjectStructure();
            Resize(settings.windowSize);
        }

        public void SetPosition(Vector2 targetPosition)
        {
            GetComponent<RectTransform>().position = targetPosition;
        }
        public void Resize(Vector2 targetSize)
        {
            Vector2 newSize = new Vector2(
                Mathf.Max(targetSize.x, MinWindowSize.x),
                Mathf.Max(targetSize.y, MinWindowSize.y)
            );
            (transform as RectTransform).sizeDelta = newSize;
        }
        public void Resize(Vector2 targetSize, Vector2 direction)
        {
            Vector2 newSize = new Vector2(
                Mathf.Max(targetSize.x, MinWindowSize.x),
                Mathf.Max(targetSize.y, MinWindowSize.y)
            );
            Vector2 windowSizeDelta = (newSize - (transform as RectTransform).sizeDelta);
            (transform as RectTransform).sizeDelta = newSize;
            SetPosition((Vector2)GetComponent<RectTransform>().position + windowSizeDelta * GetComponentInParent<Canvas>().scaleFactor * direction / 2);

        }

        public void Close()
        {
            Destroy(gameObject);
        }

        private void CreateObjectStructure()
        {
            if (IsResizeable)
            {
                CreateResizeableStructure();
            }
            CreateTitleBarStructure();
            CreateContentStructure();
        }
        private void CreateTitleBarStructure()
        {
            TitleBar = new WindowElement(new GameObject("TitleBar", typeof(LayoutElement), typeof(HorizontalLayoutGroup), typeof(Image)));
            TitleBar.rectTransform.SetParent(transform, false);
            TitleBar.layoutElement.minHeight = 25;
            TitleBar.layoutElement.preferredWidth = 25;

            TitleBar.GetComponent<HorizontalLayoutGroup>().childForceExpandWidth = false;
            TitleBar.GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = false;
            TitleBar.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
            TitleBar.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset() { left = 2, right = 2 };

            if (IsDraggable)
            {
                DragBar dragBar = TitleBar.gameObject.AddComponent<DragBar>();
                dragBar.Init(this);
            }
            GameObject title = new GameObject("Title", typeof(TextMeshProUGUI));
            title.transform.SetParent(TitleBar.rectTransform, false);


            TextMeshProUGUI textMeshProUGUI = title.GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = Title;
            textMeshProUGUI.color = Color.black;
            textMeshProUGUI.fontSize = 18;
            textMeshProUGUI.font = (TMP_FontAsset)Resources.Load("Font/KodeMono-Bold SDF");
            textMeshProUGUI.textWrappingMode = TextWrappingModes.NoWrap;
            textMeshProUGUI.overflowMode = TextOverflowModes.Ellipsis;


            GameObject spacer = new GameObject("Spacer", typeof(LayoutElement));
            spacer.transform.SetParent(TitleBar.rectTransform, false);
            spacer.GetComponent<LayoutElement>().flexibleWidth = 1;

            GameObject closeButton = new GameObject("CloseButton", typeof(LayoutElement), typeof(Image), typeof(UnityEngine.UI.Button));
            closeButton.transform.SetParent(TitleBar.rectTransform, false);
            closeButton.GetComponent<LayoutElement>().minWidth = 21;
            closeButton.GetComponent<LayoutElement>().minHeight = 21;
            closeButton.GetComponent<LayoutElement>().preferredWidth = 21;
            closeButton.GetComponent<LayoutElement>().preferredHeight = 21;

            closeButton.GetComponent<UnityEngine.UI.Button>().targetGraphic = closeButton.GetComponent<Image>();
            closeButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Close);
            closeButton.GetComponent<UnityEngine.UI.Button>().colors = new ColorBlock()
            {
                normalColor = Color.red,
                highlightedColor = new Color(1 * .8f, 0, 0),
                selectedColor = Color.red,
                pressedColor = Color.red,
                disabledColor = Color.grey,
                colorMultiplier = 1,
                fadeDuration = .1f
            };

        }
        private void CreateResizeableStructure()
        {
            ResizeBarParent = new WindowElement(new GameObject("ResizeBars", typeof(Resizable), typeof(LayoutElement)));
            ResizeBarParent.rectTransform.SetParent(transform, false);
            ResizeBarParent.rectTransform.SetAsFirstSibling();
            ResizeBarParent.rectTransform.anchorMin = Vector2.zero;
            ResizeBarParent.rectTransform.anchorMax = Vector2.one;
            ResizeBarParent.layoutElement.ignoreLayout = true;
            Resizable resizable = ResizeBarParent.gameObject.GetComponent<Resizable>();
            resizable.Init(10f, this);
        }
        private void CreateContentStructure()
        {
            Content = new WindowElement(new GameObject("Content", typeof(LayoutElement), typeof(VerticalLayoutGroup)));
            Content.rectTransform.SetParent(transform, false);
            Content.layoutElement.flexibleWidth = 1;
            Content.layoutElement.flexibleHeight = 1;
        }
    }
}