using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace Szczepanski.UI
{
    public static class FloatingWindowFactory
    {
        private static GameObject FLOATING_WINDOW_PREFAB;
        private static GameObject FLOATING_WINDOW_RESIZEABLE_PREFAB;

        public static void PreLoadPrefabs()
        {
            FLOATING_WINDOW_PREFAB = BuildFloatingWindow(null, new FloatingWindowSettings() {
                isDraggable = true,
                isResizeable = false,
                title = "PREFAB",
                minWindowSize = new Vector2(600, 400),
                windowSize = new Vector2(600, 400),
                font = (TMP_FontAsset)Resources.Load("Font/KodeMono-Bold SDF.asset"),
            });
            FLOATING_WINDOW_PREFAB.hideFlags = HideFlags.HideAndDontSave;

            FLOATING_WINDOW_RESIZEABLE_PREFAB = BuildFloatingWindow(null, new FloatingWindowSettings()
            {
                isDraggable = true,
                isResizeable = true,
                title = "PREFAB",
                minWindowSize = new Vector2(600, 400),
                windowSize = new Vector2(600, 400),
                font = (TMP_FontAsset)Resources.Load("Font/KodeMono-Bold SDF.asset"),
            });
            FLOATING_WINDOW_RESIZEABLE_PREFAB.hideFlags = HideFlags.HideAndDontSave;
        }

        public static GameObject CreateFloatingWindow(Transform parent, FloatingWindowSettings settings)
        {
            GameObject prefab = settings.isResizeable ? FLOATING_WINDOW_RESIZEABLE_PREFAB : FLOATING_WINDOW_PREFAB;
            if(prefab == null) { PreLoadPrefabs(); }
            GameObject floatingWindowObject = GameObject.Instantiate(prefab, parent);
            floatingWindowObject.hideFlags = HideFlags.None;
            FloatingWindow floatingWindow = floatingWindowObject.GetComponent<FloatingWindow>();
            floatingWindow.SetUI(settings);
            return floatingWindowObject;
        }

        private static GameObject BuildFloatingWindow(Transform parent, FloatingWindowSettings settings)
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
        public TMP_FontAsset font;
    }

    [System.Serializable]
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
         [field: SerializeField, HideInInspector] public WindowElement ResizeBarParent { get; private set; }
        [field: SerializeField, HideInInspector] public WindowElement TitleBar { get; private set; }
        [field: SerializeField, HideInInspector] public WindowElement Content { get; private set; }
        public bool IsResizeable { get; private set; }
        public bool IsDraggable { get; private set; }
        public string Title { get; private set; }
        public Vector2 MinWindowSize { get; private set; }
        public TMP_FontAsset Font { get; private set; }

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
            
            if(IsResizeable)
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
            TitleBar = new WindowElement(new GameObject("TitleBar", typeof(LayoutElement), typeof(Image)));
            TitleBar.rectTransform.SetParent(transform, false);
            new GameObject("Title", typeof(TextMeshProUGUI)).transform.SetParent(TitleBar.rectTransform, false);
            new GameObject("CloseButton", typeof(Image), typeof(UnityEngine.UI.Button)).transform.SetParent(TitleBar.rectTransform, false);

            if (IsDraggable)
            {
                DragBar dragBar = TitleBar.gameObject.AddComponent<DragBar>();
                dragBar.Init(this);
            }
        }
        private void CreateResizeableStructure()
        {
            ResizeBarParent = new WindowElement(new GameObject("ResizeBars", typeof(LayoutElement)));
            ResizeBarParent.rectTransform.SetParent(transform, false);
            ResizeBarParent.rectTransform.SetAsFirstSibling();
            ResizeBarParent.gameObject.AddComponent<Resizable>().Init(10f, this);
        }
        private void CreateContentStructure()
        {
            Content = new WindowElement(new GameObject("Content", typeof(LayoutElement), typeof(VerticalLayoutGroup)));
            Content.rectTransform.SetParent(transform, false);
        }

        private void SetWindowData()
        {
            if(IsResizeable)
            {
                SetResizeableData();
            }
            SetTitleBarData();
            SetContentData();
        }
        private void SetTitleBarData()
        {
            GameObject title = TitleBar.rectTransform.GetChild(0).gameObject;
            GameObject closeButton = TitleBar.rectTransform.GetChild(1).gameObject;

            if (!IsDraggable)
            {
                TitleBar.GetComponent<DragBar>().SetEnabled(false);
            }
            else
            {
                TitleBar.GetComponent<DragBar>().SetEnabled(true);
            }

            TitleBar.layoutElement.minHeight = 26;
            TitleBar.layoutElement.flexibleWidth = 1;

            title.transform.SetParent(TitleBar.rectTransform, false);

            RectTransform rectTransform = title.GetComponent<RectTransform>();
            rectTransform.pivot     = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
            rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            rectTransform.offsetMin = new Vector2(3.0f, 0.0f);
            rectTransform.offsetMax = new Vector2(-27.0f, 0.0f);

            TextMeshProUGUI textMeshProUGUI = title.GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = Title;
            textMeshProUGUI.color = Color.black;
            textMeshProUGUI.fontSize = 18;
            textMeshProUGUI.font = Font;
            textMeshProUGUI.alignment = TextAlignmentOptions.MidlineLeft;
            textMeshProUGUI.textWrappingMode = TextWrappingModes.NoWrap;
            textMeshProUGUI.overflowMode = TextOverflowModes.Ellipsis;


            closeButton.transform.SetParent(TitleBar.rectTransform, false);

            rectTransform = closeButton.GetComponent<RectTransform>();
            rectTransform.pivot     = new Vector2(1.0f, 0.5f);
            rectTransform.anchorMin = new Vector2(1.0f, 0.5f);
            rectTransform.anchorMax = new Vector2(1.0f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(-3, 0);
            rectTransform.sizeDelta = new Vector2(20, 20);


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
            if (!IsResizeable) { ResizeBarParent.gameObject.SetActive(false); return; }
            ResizeBarParent.rectTransform.anchorMin = Vector2.zero;
            ResizeBarParent.rectTransform.anchorMax = Vector2.one;
            ResizeBarParent.layoutElement.ignoreLayout = true;
        }
        private void SetContentData()
        {
            Content.layoutElement.flexibleWidth = 1;
            Content.layoutElement.flexibleHeight = 1;
        }
    }
}