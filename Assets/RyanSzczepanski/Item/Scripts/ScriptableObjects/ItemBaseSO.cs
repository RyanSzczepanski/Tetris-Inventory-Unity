using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ItemBaseSO : ScriptableObject
{
    public string fullName;
    public string shortName;
    public string description;
    public Vector2Int size;
    public float weight;
    public Sprite icon;

    public ItemBase CreateItem()
    {
        return new ItemBase(this);
    }
}
