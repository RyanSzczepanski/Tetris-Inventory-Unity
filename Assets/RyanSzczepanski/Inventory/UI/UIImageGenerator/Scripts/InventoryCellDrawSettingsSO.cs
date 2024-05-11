using UnityEngine;

[CreateAssetMenu(menuName = "InventoryUI/DrawSettings", fileName = "New SubInventory UI Draw Settings")]
public class InventoryCellDrawSettingsSO : ScriptableObject
{
    public int _cellSize = 50;
    public int _paddingSize = 1;
    public int _outlineSize = 1;

    public Color _cellColor = new Color(0.1f, 0.1f, 0.1f, 0.85f);
    public Color _cellAccentColor = new Color(0.15f, 0.15f, 0.15f, 0.85f);

    public Color _cellOutlineColor = new Color(0.3f, 0.3f, 0.3f, 0.75f);
    public Color _cellOutlineAccentColor = new Color(0.4f, 0.4f, 0.4f, 0.85f);

    public Color _outlineColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);
    public Color _outlineAccentColor = new Color(0.85f, 0.85f, 0.85f, 0.6f);
}
