using UnityEngine;

[CreateAssetMenu(menuName = "InventoryUI/DrawSettings", fileName = "New SubInventory UI Draw Settings")]
public class InventoryCellDrawSettingsSO : ScriptableObject
{
    public int _cellSize = 50;
    public int _paddingSize = 1;
    public int _outlineSize = 1;

    public Color _cellColor = new(0.1f, 0.1f, 0.1f, 0.85f);
    public Color _cellAccentColor = new(0.15f, 0.15f, 0.15f, 0.85f);

    public Color _cellOutlineColor = new(0.3f, 0.3f, 0.3f, 0.75f);
    public Color _cellOutlineAccentColor = new(0.4f, 0.4f, 0.4f, 0.85f);

    public Color _outlineColor = new(0.9f, 0.9f, 0.9f, 0.5f);
    public Color _outlineAccentColor = new(0.85f, 0.85f, 0.85f, 0.6f);
}

public struct InventoryCellDrawSettings
{
    public int   _cellSize;
    public int   _paddingSize;
    public int   _outlineSize;

    public Color _cellColor;
    public Color _cellAccentColor;

    public Color _cellOutlineColor;
    public Color _cellOutlineAccentColor;

    public Color _outlineColor;
    public Color _outlineAccentColor;

    public InventoryCellDrawSettings(InventoryCellDrawSettingsSO drawSettings)
    {
        _cellSize = drawSettings._cellSize;
        _paddingSize = drawSettings._paddingSize;
        _outlineSize = drawSettings._outlineSize;

        _cellColor = drawSettings._cellColor;
        _cellAccentColor = drawSettings._cellAccentColor;

        _cellOutlineColor = drawSettings._cellOutlineColor;
        _cellOutlineAccentColor = drawSettings._cellOutlineAccentColor;

        _outlineColor = drawSettings._outlineColor;
        _outlineAccentColor = drawSettings._outlineAccentColor;
    }
}