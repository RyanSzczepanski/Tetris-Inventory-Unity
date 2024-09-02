using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemBaseSO : ScriptableObject
{
    [field: SerializeField] public string FullName { get; private set; }
    [field: SerializeField] public string ShortName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; }
    [field: SerializeField] public float Weight { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public ItemTags Tags { get; private set; }

    public virtual ItemBase CreateItem()
    {
        return new ItemBase(this);
    }
}
