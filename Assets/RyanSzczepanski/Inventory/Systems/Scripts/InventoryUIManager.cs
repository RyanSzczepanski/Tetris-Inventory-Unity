using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Szczepanski.UI;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager INSTANCE;
    public static InventoryCellDrawSettings DRAW_SETTINGS;
    public static Canvas CANVAS;

    [SerializeField] private InventoryCellDrawSettingsSO drawSettingsSO;
    [SerializeField] private Canvas canvas;
    [SerializeField] private ComputeShader computeShader;

    private void Awake()
    {
        if (INSTANCE == null) { INSTANCE = this; }
        else { Destroy(this); }

        DRAW_SETTINGS = new InventoryCellDrawSettings(drawSettingsSO);
        CANVAS = canvas;
        InventoryUIGenerator.PreLoadPrefabs();
        FloatingWindowFactory.PreLoadPrefabs();
        SubInventoryUIGenerator.PreLoadPrefabs();
        SubInventoryUIGenerator.SetShader(computeShader);
        InventoryUIGenerator.PreJIT(DRAW_SETTINGS);
        DragItemUI.Init(DRAW_SETTINGS);
    }
}
