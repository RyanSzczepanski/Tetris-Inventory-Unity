using System;
using UnityEngine;
using UnityEngine.UI;


namespace Szczepanski
{
    namespace UI
    {
        [RequireComponent(typeof(RectTransform))]
        public class Resizable : MonoBehaviour
        {
            [SerializeField] private float resizeBarSize;
            public void CreateResizeBars(Transform parent, FloatingWindow window)
            {
                foreach (var dir in (Int32[])Enum.GetValues(typeof(ResizeBarDirection)))
                {
                    GameObject go = new GameObject($"{(ResizeBarDirection)dir}");

                    ResizeBar bar = go.AddComponent<ResizeBar>();
                    bar.Init(window, (ResizeBarDirection)dir);

                    RectTransform rt = go.AddComponent<RectTransform>();
                    Image image = go.AddComponent<Image>();

                    rt.anchorMin = ResizeBarsEnumUtils.GetMin(dir);
                    rt.anchorMax = ResizeBarsEnumUtils.GetMax(dir);

                    if (ResizeBarsEnumUtils.IsCorner(dir))
                    {
                        image.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                    }
                    else
                    {
                        image.color = new Color(1.0f, 0.4f, 0.0f, 0.5f);
                    }


                    //I am so sorry for when I come back and look at this :(

                    rt.sizeDelta = new Vector2((
                            ((dir & 0b10_0000) >> 5)//Corner logic
                        + (((dir ^ 0b11_0000) & 0b11_0000) >> 5) * ((dir ^ 0b01_0000) >> 4)
                        - ((dir & 0b01_0000) >> 4)
                        ) * resizeBarSize,
                        (
                            ((dir & 0b10_0000) >> 5)//Corner logic
                        + ((dir & 0b01_0000) >> 4)
                        - (((dir ^ 0b11_0000) & 0b11_0000) >> 5) * ((dir ^ 0b01_0000) >> 4)
                        ) * resizeBarSize);


                    //Does the same shit but is actually readable

                    //Vector2 vect2 = new Vector2();
                    //if (ResizeBarsEnumUtils.IsCorner(dir))
                    //{
                    //    vect2.Set(5, 5);
                    //}
                    //else
                    //{
                    //    if (ResizeBarsEnumUtils.IsEdgeHorizontal(dir))
                    //    {
                    //        vect2.Set(-5, 5);
                    //    }
                    //    else
                    //    {
                    //        vect2.Set(5, -5);
                    //    }
                    //}
                    //rt.sizeDelta = vect2;

                    go.transform.SetParent(parent, false);

                }
            }
            public void Init(float resizeBarSize, FloatingWindow window)
            {
                this.resizeBarSize = resizeBarSize;
                GetComponent<RectTransform>().sizeDelta = Vector2.zero;//Vector2.one * resizeBarSize;
                CreateResizeBars(transform, window);
            }
        }


        public static class ResizeBarsEnumUtils
        {
            public static bool IsCorner(ResizeBarDirection dir)
            {
                return Convert.ToBoolean(((int)dir & 0b10_0000) >> 5);
            }
            public static bool IsCorner(int dir)
            {
                return Convert.ToBoolean((dir & 0b10_0000) >> 5);
            }
            public static bool IsEdgeHorizontal(ResizeBarDirection dir)
            {
                return Convert.ToBoolean(((int)dir & 0b01_0000) >> 4);
            }
            public static bool IsEdgeHorizontal(int dir)
            {
                return Convert.ToBoolean((dir & 0b01_0000) >> 4);
            }
            public static Vector2 GetMin(ResizeBarDirection dir)
            {
                return new Vector2(((int)dir & 0b00_1000) >> 3, ((int)dir & 0b00_0100) >> 2);
            }
            public static Vector2 GetMin(int dir)
            {
                return new Vector2((dir & 0b_1000) >> 3, (dir & 0b_0100) >> 2);

            }
            public static Vector2 GetMax(ResizeBarDirection dir)
            {
                return new Vector2(((int)dir & 0b_0010) >> 1, ((int)dir & 0b_0001) >> 0);
            }
            public static Vector2 GetMax(int dir)
            {
                return new Vector2((dir & 0b_0010) >> 1, (dir & 0b_0001) >> 0);
            }

            public static Vector2Int ExpandDirection(ResizeBarDirection dir)
            {
                return new Vector2Int(
                      ((((int)dir & ((int)dir << 2)) >> 3) & 0b0001)                        //Right
                    - (((((int)dir ^ 0b1000) & (((int)dir ^ 0b0010) << 2)) >> 3) & 0b0001), //Left
                      ((((int)dir & ((int)dir << 2)) >> 2) & 0b0001)                        //Up
                    - (((((int)dir ^ 0b0100) & (((int)dir ^ 0b0001) << 2)) >> 2) & 0b0001)  //Down
                    );
            }
        }


        public enum ResizeBarDirection
        {
            //[IsCorner][IsEdgeHorizontal]_[MinX][MinY][MaxX][MaxY]
            TopLeft = 0b10_0101,
            TopCenter = 0b01_0111,
            TopRight = 0b10_1111,
            MiddleLeft = 0b00_0001,
            MiddleRight = 0b00_1011,
            BottomLeft = 0b10_0000,
            BottomCenter = 0b01_0010,
            BottomRight = 0b10_1010,
        }
    }
}