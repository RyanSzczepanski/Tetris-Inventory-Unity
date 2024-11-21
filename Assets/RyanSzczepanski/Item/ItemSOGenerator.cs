using Sirenix.OdinInspector;
using Szczepanski.ScriptGenerator;
using UnityEngine;

public class ItemSOGenerator : MonoBehaviour
{
    public ScriptGeneratorSettigns settings;
    ScriptGenerator scriptGenerator;



    [Button]
    public void Generate()
    {
        scriptGenerator = new ScriptGenerator()
        {
            settings = settings,
        };


        Debug.Log(scriptGenerator.GenerateCode());
    }
}

[System.Serializable]
public struct ScriptGeneratorSettigns
{
    public string[] usings;
    public string nameSpace;
    public Class newClass;
}
