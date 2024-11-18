using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Basic Item SO", menuName = "Items/Basic Item")]
public class ItemBasicSO : ItemBaseSO
{
    public override ItemTags Tags => ItemTags.Basic;

    public override ItemBase CreateItem()
    {
        return new ItemBasic(this);
    }
}