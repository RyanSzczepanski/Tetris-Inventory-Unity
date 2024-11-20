using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Szczepanski.UI
{
    [RequireComponent(typeof(MaskableGraphic))]
    public class Button : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [field: SerializeField] public bool IsActive { get; private set; } = true;

        public Color normalColor = Color.white;
        public Color highlightColor = Color.red;
        public Color PressedColor = Color.red;
        public Color SelectedColor = Color.white;
        public Color DisabledColor = Color.white;

        public float fadeDuration = 0.1f;

        public delegate void OnClick();
        public event OnClick OnButtonClicked;

        private Color startColor;
        private Color targetColor;
        private bool doLerp;
        private float startTime;
        private float lerpValue;

        private void Awake()
        {
            startColor = normalColor;
        }

        private void StartLerp(Color targetColor)
        {
            startColor = GetComponent<Image>().color;
            this.targetColor = targetColor;
            startTime = Time.realtimeSinceStartup;
            doLerp = true;
        }
        private void EndLerp()
        {
            doLerp = false;
        }
        private void Update()
        {
            if (doLerp)
            {
                lerpValue = (Time.realtimeSinceStartup - startTime) / fadeDuration;
                GetComponent<Image>().color = Color.Lerp(startColor, targetColor, lerpValue);
                if (lerpValue > 1)
                {
                    EndLerp();
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnButtonClicked?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            StartLerp(normalColor);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            StartLerp(PressedColor);
        }
    }
}