using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Szczepanski
{
    namespace UI
    {
        public class ContextMenu : MonoBehaviour
        {
            public List<ContextMenuOption> options = new();

            private void Start()
            {
                GenerateContextMenu();
            }

            public void GenerateContextMenu()
            {
                foreach (ContextMenuOption option in options)
                {
                    option.GenerateContextMenuObject(transform);
                }
            }
        }
    }
}
