using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SubInventoryArrangement
{
    [HideIf("IsLeaf")]
    public TextAnchor alignment;
    [HideIf("IsLeaf")]
    public GridLayoutGroup.Axis direction;
    [HideIf("HasSubInventory")]
    public SubInventoryArrangement[] childArrangements;
    [ShowIf("IsLeaf")]
    public Vector2Int subInventorySize;

    public bool IsLeaf { get => childArrangements.Length == 0; }
    public bool HasSubInventory { get => subInventorySize != Vector2Int.zero; }
}