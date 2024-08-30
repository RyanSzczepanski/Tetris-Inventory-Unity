using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Szczepanski;

namespace Szczepanski
{
    namespace UI
    {

        [System.Serializable]
        public class ContextMenuOption
        {
            public string optionText;

            public GameObject GenerateContextMenuObject(Transform parentTransform)
            {
                GameObject contextMenuObject = new GameObject("ContextMenuOption");
                contextMenuObject.transform.parent = parentTransform;
                contextMenuObject.AddComponent<RectTransform>();
                contextMenuObject.AddComponent<Button>();
                contextMenuObject.transform.localPosition = Vector3.zero;
                return null;
            }
        }
    }
}