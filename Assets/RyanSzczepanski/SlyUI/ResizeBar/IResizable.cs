using System.Diagnostics;
using UnityEngine;

namespace Szczepanski
{
    namespace UI
    {
        public interface IResizable
        {
            public bool IsResizeable { get; }
            public Vector2 MinWindowSize { get; }

            public WindowElement ResizeBarParent { get; }
            public WindowElement Content { get; }
            public void Resize(Vector2 targetSize, Vector2 direction);
        }
    }
}