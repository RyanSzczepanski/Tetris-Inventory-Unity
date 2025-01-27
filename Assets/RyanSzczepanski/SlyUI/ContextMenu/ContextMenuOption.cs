using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Szczepanski.UI
{
    [System.Serializable]
    public struct ContextMenuOption
    {
        public string optionText;
        public delegate void OnSelectHandler();
        public OnSelectHandler OnSelected;

        public void ContextMenuClicked()
        {
            OnSelected?.Invoke();
        }

        public GameObject GenerateContextMenuObject(Transform parentTransform, TMP_FontAsset font)
        {
            GameObject contextMenuObject = new GameObject("ContextMenuOption", typeof(RectTransform), typeof(Image), typeof(UnityEngine.UI.Button), typeof(LayoutElement));
            contextMenuObject.transform.SetParent(parentTransform, false);
            contextMenuObject.GetComponent<LayoutElement>().minHeight = 20;

            GameObject contextMenuObjectText = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            contextMenuObjectText.transform.SetParent(contextMenuObject.transform, false);
            contextMenuObjectText.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            contextMenuObjectText.GetComponent<RectTransform>().anchorMax = Vector2.one;
            contextMenuObjectText.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            contextMenuObjectText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            contextMenuObjectText.GetComponent<TextMeshProUGUI>().fontSize = 18f;
            contextMenuObjectText.GetComponent<TextMeshProUGUI>().color = Color.black;
            contextMenuObjectText.GetComponent<TextMeshProUGUI>().text = optionText;
            contextMenuObjectText.GetComponent<TextMeshProUGUI>().font = font;
            return contextMenuObject;
        }
    }

    public class ContextMenuOptionMono : MonoBehaviour
    {
       
    }
}