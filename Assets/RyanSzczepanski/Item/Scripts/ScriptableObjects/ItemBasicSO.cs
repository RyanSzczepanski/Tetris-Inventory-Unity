using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Basic Item SO", menuName = "Items/Basic Item")]
public class ItemBasicSO : ItemBaseSO
{
    public override ItemTags Tags => ItemTags.Basic;

    public override ItemBase CreateItem()
    {
        return new ItemBasic(this);
    }
}
