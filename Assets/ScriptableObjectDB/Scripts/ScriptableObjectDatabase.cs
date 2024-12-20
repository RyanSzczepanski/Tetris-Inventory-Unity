using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectDatabase<T> : MonoBehaviour where T : ScriptableObject
{
    static Dictionary<string, T> objects;

    private void Awake() => Init();

    public static void Init()
    {
        objects = new Dictionary<string, T>();
        var objectArray = Resources.LoadAll<T>("");
        foreach (var obj in objectArray)
        {
            if (objects.ContainsKey(obj.name))
            {
                Debug.LogError($"Duplicate Entry \'{obj.name}\'");
                return;
            }
            objects[obj.name] = obj;
        }
    }

    public static T GetObjectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.LogError($"Object with name {name} not found in database");
            return null;
        }
        return objects[name];
    }

    public static Dictionary<string, T>.ValueCollection GetValues() => objects.Values;
    public static T[] GetObjectArray() => objects.Values.ToArray<T>();
    public static int GetLength() => objects.Count;
}
