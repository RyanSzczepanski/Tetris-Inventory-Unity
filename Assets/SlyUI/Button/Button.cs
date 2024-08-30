using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Szczepanski
{
    [RequireComponent(typeof(Image))]
    public class Button : MonoBehaviour
    {
        [field: SerializeField] public bool IsActive { get; private set; } = true;

        public Color normalColor = Color.white;
        public Color highlightColor = Color.red;
        public Color PressedColor = Color.white;
        public Color SelectedColor = Color.white;
        public Color DisabledColor = Color.white;

        public float fadeDuration = 0.1f;


        private Color startColor;
        private Color targetColor;
        private bool doLerp;
        private float startTime;
        private float lerpValue;

        private void StartLerp()
        {
            startTime = Time.realtimeSinceStartup;
            doLerp = true;
        }
        private void EndLerp()
        {
            doLerp = false;
        }
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                StartLerp();
            }
            if (doLerp)
            {
                lerpValue = (Time.realtimeSinceStartup - startTime) / fadeDuration;
                if (lerpValue > 1)
                {
                    EndLerp();
                }
            }
            GetComponent<Image>().color = Color.Lerp(normalColor, highlightColor, lerpValue);
        }
    }
}