using System.Runtime.InteropServices.WindowsRuntime;
using Szczepanski.ScriptGenerator;
using UnityEngine;

[CreateAssetMenu(fileName = "New Script Generator Interface", menuName = "Scriptable Objects/Script Generator Interface")]
public class ScriptGeneratorPropertySO : ScriptableObject
{
    public string name;
    public Property[] properties;
    public Function[] functions;

    public Interface AsInterface => new Interface() {name = name, properties = properties, functions = functions};
    public string ToJSON(ScriptGeneratorPropertySO scriptGeneratorPropertySO)
    {
        return JsonUtility.ToJson(scriptGeneratorPropertySO);
    }
}
