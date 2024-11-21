using Sirenix.OdinInspector;
using Szczepanski.ScriptGenerator;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct ScriptGeneratorSettings
{
    public string[] usings;
    public string nameSpace;
    public Class @class;

    public static ScriptGeneratorSettings Empty => new ScriptGeneratorSettings() { usings = new string[0], nameSpace = string.Empty, @class = Class.Empty };
    public ScriptGeneratorSettings(string[] usings, string nameSpace, Class @class)
    {
        this.usings = usings;
        this.nameSpace = nameSpace;
        this.@class = @class;
    }
}
