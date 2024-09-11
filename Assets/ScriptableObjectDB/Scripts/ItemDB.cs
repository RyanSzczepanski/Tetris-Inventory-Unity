using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB : ScriptableObjectDatabase<ItemBaseSO>
{
    private void Awake()
    {
        Init();
    }
}
