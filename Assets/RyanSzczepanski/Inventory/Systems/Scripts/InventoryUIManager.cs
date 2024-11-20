using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager INSTANCE;
    public static InventoryCellDrawSettings DRAW_SETTINGS;
    public static Canvas CANVAS;

    [SerializeField] private InventoryCellDrawSettingsSO drawSettingsSO;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        if (INSTANCE == null) { INSTANCE = this; }
        else { Destroy(this); }

        DRAW_SETTINGS = new InventoryCellDrawSettings(drawSettingsSO);
        CANVAS = canvas;
        DragItemUI.Init(DRAW_SETTINGS);
    }
}
