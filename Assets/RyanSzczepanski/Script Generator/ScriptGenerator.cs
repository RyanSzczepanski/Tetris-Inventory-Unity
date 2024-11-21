using JetBrains.Annotations;
using Sirenix.OdinInspector;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace Szczepanski.ScriptGenerator
{
    public class ScriptGenerator : MonoBehaviour
    {
        public ScriptGeneratorSettigns settings;
        //public string nameSpace;
        //public string className;
        //public string baseClass;
        //public string[] interfaces;
        //public string[] usings;
        private CodeBuilder code = new CodeBuilder();

        [Button]
        public void Test()
        {
            Debug.Log(GenerateCode());
        }

        public string GenerateCode()
        {
            code.Clear();
            //Usings
            foreach (string s in settings.usings)
            {
                code.AppendLine($"using {s};");
            }
            if (settings.usings.Length != 0)
            {
                code.AppendLine();
            }
            //NameSpace
            if (settings.nameSpace != string.Empty)
            {
                code.AppendLine($"namespace {settings.nameSpace}");
                code.OpenBody();
            }
            //Class Name
            code.AppendLine(settings.newClass.ToString());
            //code.Append($"public class {className}");
            //if (baseClass != string.Empty)
            //{
            //    code.Append($" : {baseClass}");
            //}
            //code.AppendLine();
            //code.OpenBody();


            //Property property = new Property()
            //{
            //    accessibility = Accessibility.Public,
            //    type = "string",
            //    isOnlyGetter = true,
            //    getterBody = "ItemTags.Basic;",
            //    isPrivateSetter = true,
            //    name = "StringProperty"
            //};

            //code.AppendLine(property.ToString());

            return code.ToString();
        }

        public string IndentString(int indentAmount)
        {
            string indent = string.Empty;
            for (int i = 0; i < indentAmount; i++)
            {
                indent += "\t";
            }
            return indent;
        }
    }

    [System.Serializable]
    public struct Class
    {
        public string atributes;
        public string name;
        public string baseClass;
        public string[] interfaces;
        public Property[] properties;
        public string functions;

        public CodeBuilder code;

        public override string ToString()
        {
            code = new CodeBuilder();
            code.AppendLine(atributes);
            code.Append($"public class {name}");
            if (baseClass != string.Empty)
            {
                code.Append($" : {baseClass}");
            }
            if (baseClass != string.Empty)
            {
                code.Append($", {interfaces.ToCommaSeparatedString()}");
            }
            else
            {
                code.Append($" : {interfaces.ToCommaSeparatedString()}");
            }
            code.AppendLine();
            code.OpenBody();

            foreach(Property property in properties)
            {
                code.AppendLine(property.ToString());
            }

            return code.ToString();
        }
    }

    [System.Serializable]
    public struct Property
    {
        public string name;
        public string type;
        public bool isPrivateSetter;
        public bool isOnlyGetter;
        public bool hasBackingField;
        public string getterBody;
        public Accessibility accessibility;


        public override string ToString()
        {
            CodeBuilder code = new CodeBuilder();
            code.AppendLine($"{accessibility.ToString().ToLower()} {type} {name} {{ {GenerateGetter()} {GenerateSetter()} }}");
            if (!hasBackingField) { return code.ToString().Trim('\n'); }
            code.AppendLine($"private {type} m_{name}");
            return code.ToString().Trim('\n');
        }


        private string GenerateGetter()
        {
            if (hasBackingField)
            {
                return $"get =>  m_{name};";
            }
            else
            {
                return "get => " + (getterBody == string.Empty ? "throw new System.NotImplementedException();" : getterBody);
            }
        }
        private string GenerateSetter()
        {
            if (isOnlyGetter) { return string.Empty; }
            else
            {
                string setterAccessor = (isPrivateSetter && accessibility == Accessibility.Public) ? "private" : string.Empty;
                return $"{setterAccessor} set => m_{name} = value;";
            }
        }
    }

    public struct Function
    {
        public string name;
        public string body;
        public Accessibility accessibility;

    }

    public enum Accessibility
    {
        Private = 0,
        Protected = 1,
        Public = 2,

    }

    public class CodeBuilder
    {
        private StringBuilder code = new StringBuilder();
        private StringBuilder line = new StringBuilder();
        private int indent;
        private string IndentString
        {
            get
            {
                string str = string.Empty;
                for (int i = 0; i < indent; i++)
                {
                    str += "\t";
                }
                return str;
            }
        }

        public void Append(string str)
        {
            if (line.Length == 0) { line.Append(IndentString); }
            line.Append($"{str}");
        }

        public void AppendLine()
        {
            code.AppendLine(line.ToString());
            line.Clear();
        }

        public void EndLine()
        {
            code.Append(line);
        }

        public void OpenBody()
        {
            AppendLine("{");
            indent++;
        }
        public void CloseBody()
        {
            indent--;
            AppendLine("}");
        }

        public void AppendLine(string str)
        {
            if (line.Length == 0) { line.Append(IndentString); }
            line.Append($"{str}");
            line.Replace("\n", $"\n{IndentString}");
            AppendLine();
        }

        public void AppendLine(string[] strArr)
        {
            foreach(string strItem in strArr)
            {
                if (line.Length == 0) { line.Append(IndentString); }
                line.Append($"{strItem}");
                AppendLine();
            }   
        }

        public void Clear()
        {
            code.Clear();
            line.Clear();
            indent = 0;
        }

        public override string ToString()
        {
            for (int i = 0; i < indent;)
            {
                CloseBody();
            }
            return code.ToString();
        }
    }
}
