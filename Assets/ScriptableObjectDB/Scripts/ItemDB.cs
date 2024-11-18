using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : ScriptableObjectDatabase<ItemBaseSO>
{
    public static T[] GetItems<T>() 
        where T : ItemBaseSO
    {
        List<T> items = new List<T>();
        foreach (ItemBaseSO itemBaseSO in GetObjectArray())
        {
            if(itemBaseSO.GetType() == typeof(T))
            {
                items.Add(itemBaseSO as T);
            }
        }
        return items.ToArray();
    }
    public static ItemBaseSO[] GetItems(ItemTags itemTags)
    {
        List<ItemBaseSO> items = new List<ItemBaseSO>();
        foreach (ItemBaseSO itemBaseSO in GetObjectArray())
        {
            if (itemBaseSO.Tags == itemTags)
            {
                items.Add(itemBaseSO);
            }
        }
        return items.ToArray();
    }
}
