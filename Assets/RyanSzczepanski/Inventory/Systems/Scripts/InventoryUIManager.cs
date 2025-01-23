using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager INSTANCE;
    public static InventoryCellDrawSettings DRAW_SETTINGS;

    [SerializeField] private InventoryCellDrawSettingsSO drawSettingsSO;

    private void Awake()
    {
        Application.targetFrameRate = 10000;
        if (INSTANCE == null) { INSTANCE = this; }
        else { Destroy(this); }

        DRAW_SETTINGS = new InventoryCellDrawSettings(drawSettingsSO);
        DragItemUI.Init(DRAW_SETTINGS);
    }
}
