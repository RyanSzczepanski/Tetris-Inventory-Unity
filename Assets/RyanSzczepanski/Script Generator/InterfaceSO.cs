using System.Runtime.InteropServices.WindowsRuntime;
using Szczepanski.ScriptGenerator;
using UnityEngine;

namespace Szczepanski.ScriptGenerator
{
    [CreateAssetMenu(fileName = "New Script Generator Interface", menuName = "Script Generator/Interface SO")]
    public class InterfaceSO : ScriptableObject
    {
        public string name;
        public Property[] properties;
        public Function[] functions;

        public Interface AsInterface => new Interface() { name = name, properties = properties, functions = functions };
    }
}