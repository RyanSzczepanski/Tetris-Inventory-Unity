using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace Szczepanski.UI
{
    public static class FloatingWindowFactory
    {
        private static GameObject _PREFAB;

        public static void PreLoadPrefab()
        {
            if (_PREFAB != null) { return; }
            _PREFAB = BuildFloatingWindow(null, new FloatingWindowSettings() {
                isDraggable = true,
                isResizeable = true,
                title = "PREFAB",
                minWindowSize = new Vector2(600, 400),
                windowSize = new Vector2(600, 400)
            });
        }

        public static GameObject CreateFloatingWindow(Transform parent, FloatingWindowSettings settings)
        {
            if(_PREFAB == null) { PreLoadPrefab(); }
            GameObject go = GameObject.Instantiate(_PREFAB, parent);
            ContentSizeFitter csf = go.GetComponent<ContentSizeFitter>();
            if (settings.isResizeable) { csf.enabled = false; }
            else { csf.enabled = true; }
            FloatingWindow floatingWindow = go.GetComponent<FloatingWindow>();
            floatingWindow.SetUI(settings);
            return go;
        }

        private static GameObject BuildFloatingWindow(Transform parent, FloatingWindowSettings settings)
        {
            GameObject go = new GameObject("FloatingWindow", typeof(RectTransform), typeof(FloatingWindow), typeof(VerticalLayoutGroup));
            go.transform.SetParent(parent, false);
            go.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = false;
            go.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;

            ContentSizeFitter csf = go.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.MinSize;
            csf.verticalFit = ContentSizeFitter.FitMode.MinSize;
            if (settings.isResizeable) { csf.enabled = false; }

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
            Profiler.BeginSample("CreateWindowStructure");
            CreateWindowStructure();
            Profiler.EndSample();
            Profiler.BeginSample("SetWindowData");
            SetWindowData();
            Profiler.EndSample();
            Resize(settings.windowSize);
        }
        public void SetUI(FloatingWindowSettings settings)
        {
            IsDraggable = settings.isDraggable;
            IsResizeable = settings.isResizeable;
            MinWindowSize = settings.minWindowSize;
            Title = settings.title;
            Profiler.BeginSample("SetWindowData");
            SetWindowData();
            Profiler.EndSample();
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

        private void CreateWindowStructure()
        {
            if (IsResizeable)
            {
                Profiler.BeginSample("CreateResizeableStructure");
                CreateResizeableStructure();
                Profiler.EndSample();

            }
            Profiler.BeginSample("CreateTitleBarStructure");
            CreateTitleBarStructure();
            Profiler.EndSample();

            Profiler.BeginSample("CreateContentStructure");
            CreateContentStructure();
            Profiler.EndSample();
        }
        private void CreateTitleBarStructure()
        {
            TitleBar = new WindowElement(new GameObject("TitleBar", typeof(LayoutElement), typeof(HorizontalLayoutGroup), typeof(Image)));
            new GameObject("Title", typeof(TextMeshProUGUI)).transform.SetParent(TitleBar.rectTransform, false);
            new GameObject("Spacer", typeof(LayoutElement)).transform.SetParent(TitleBar.rectTransform, false);
            GameObject closeButton = new GameObject("CloseButton", typeof(LayoutElement), typeof(Image), typeof(UnityEngine.UI.Button));
            closeButton.transform.SetParent(TitleBar.rectTransform, false);
            TitleBar.rectTransform.SetParent(transform, false);

            if (IsDraggable)
            {
                DragBar dragBar = TitleBar.gameObject.AddComponent<DragBar>();
                dragBar.Init(this);
            }
        }
        private void CreateResizeableStructure()
        {
            ResizeBarParent = new WindowElement(new GameObject("ResizeBars", typeof(Resizable), typeof(LayoutElement)));
            ResizeBarParent.rectTransform.SetParent(transform, false);
            ResizeBarParent.rectTransform.SetAsFirstSibling();
        }
        private void CreateContentStructure()
        {
            Content = new WindowElement(new GameObject("Content", typeof(LayoutElement), typeof(VerticalLayoutGroup)));
            Content.rectTransform.SetParent(transform, false);
        }

        private void SetWindowData()
        {
            SetResizeableData();
            SetTitleBarData();
            SetContentData();
        }
        private void SetTitleBarData()
        {
            TitleBar = new WindowElement(transform.GetChild(1).gameObject);

            GameObject title = TitleBar.rectTransform.GetChild(0).gameObject;
            GameObject spacer = TitleBar.rectTransform.GetChild(1).gameObject;
            GameObject closeButton = TitleBar.rectTransform.GetChild(2).gameObject;

            if (!IsDraggable)
            {
                TitleBar.GetComponent<DragBar>().SetEnabled(false);
            }
            else
            {
                TitleBar.GetComponent<DragBar>().SetEnabled(true);
            }

            TitleBar.layoutElement.minHeight = 25;
            TitleBar.layoutElement.preferredWidth = 25;

            HorizontalLayoutGroup horizontalLayoutGroup = TitleBar.GetComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childForceExpandWidth = false;
            horizontalLayoutGroup.childForceExpandHeight = false;
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
            horizontalLayoutGroup.padding = new RectOffset() { left = 2, right = 2 };

            title.transform.SetParent(TitleBar.rectTransform, false);


            TextMeshProUGUI textMeshProUGUI = title.GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = Title;
            textMeshProUGUI.color = Color.black;
            textMeshProUGUI.fontSize = 18;
            textMeshProUGUI.font = (TMP_FontAsset)Resources.Load("Font/KodeMono-Bold SDF");
            textMeshProUGUI.textWrappingMode = TextWrappingModes.NoWrap;
            textMeshProUGUI.overflowMode = TextOverflowModes.Ellipsis;


            spacer.transform.SetParent(TitleBar.rectTransform, false);
            spacer.GetComponent<LayoutElement>().flexibleWidth = 1;

            closeButton.transform.SetParent(TitleBar.rectTransform, false);

            LayoutElement layoutElement = closeButton.GetComponent<LayoutElement>();
            layoutElement.minWidth = 21;
            layoutElement.minHeight = 21;
            layoutElement.preferredWidth = 21;
            layoutElement.preferredHeight = 21;


            UnityEngine.UI.Button button = closeButton.GetComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(Close);
            button.targetGraphic = closeButton.GetComponent<Image>();
            button.colors = new ColorBlock()
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
        private void SetResizeableData()
        {
            ResizeBarParent = new WindowElement(transform.GetChild(0).gameObject);
            if (!IsResizeable) { ResizeBarParent.gameObject.SetActive(false); return; }
            ResizeBarParent.rectTransform.anchorMin = Vector2.zero;
            ResizeBarParent.rectTransform.anchorMax = Vector2.one;
            ResizeBarParent.layoutElement.ignoreLayout = true;
            Resizable resizable = ResizeBarParent.gameObject.GetComponent<Resizable>();
            resizable.Init(10f, this);
        }
        private void SetContentData()
        {
            Content = new WindowElement(transform.GetChild(2).gameObject);

            Content.layoutElement.flexibleWidth = 1;
            Content.layoutElement.flexibleHeight = 1;
        }
    }
}