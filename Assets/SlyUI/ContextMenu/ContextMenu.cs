using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Szczepanski.UI
{
    public static class ContextMenuFactory
    {
        public static GameObject CreateContextMenu(Transform parent, ContextMenuSettings settings)
        {
            GameObject go = new GameObject("ContextMenu", typeof(RectTransform), typeof(ContextMenu), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            go.transform.SetParent(parent, false);
            go.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
            go.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;

            ContextMenu contextMenu = go.GetComponent<ContextMenu>();
            contextMenu.GenerateUI(settings);
            return go;
        }
    }

    public class ContextMenu : MonoBehaviour
    {
        public ContextMenuOption[] options;
        public ContextMenuLifeSpan lifeSpan;
        public ContextMenuSizeFit sizeFit;
        public void GenerateUI(ContextMenuSettings settings)
        {
            options = settings.options;
            lifeSpan = settings.lifeSpan;
            sizeFit = settings.sizeFit;

            GenerateContextMenu();
        }

        private void GenerateContextMenu()
        {
            float maxSize = 0;
            foreach (ContextMenuOption option in options)
            {
                GameObject cmObject = option.GenerateContextMenuObject(transform);
                cmObject.GetComponentInChildren<Button>().OnButtonClicked += option.ContextMenuClicked;
                if (sizeFit == ContextMenuSizeFit.ToLargestElement)
                {
                    maxSize = Mathf.Max(cmObject.GetComponentInChildren<TextMeshProUGUI>().preferredWidth, maxSize);
                }
                if (lifeSpan == ContextMenuLifeSpan.OnOptionSelect)
                {
                    cmObject.GetComponentInChildren<Button>().OnButtonClicked += Close;
                }
            }
            if (sizeFit == ContextMenuSizeFit.ToLargestElement)
            {
                GetComponent<RectTransform>().sizeDelta = new Vector2(maxSize, 0);
            }
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public struct ContextMenuSettings
    {
        public float width;
        public ContextMenuLifeSpan lifeSpan;
        public ContextMenuSizeFit sizeFit;

        public ContextMenuOption[] options;
    }

    public enum ContextMenuLifeSpan
    {
        Manual,
        Time,
        OnOptionSelect,
    }
    public enum ContextMenuSizeFit
    {
        Manual,
        ToLargestElement,
    }
}
