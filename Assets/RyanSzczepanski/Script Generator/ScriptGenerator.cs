using System.IO;
using System.Text;
using UnityEngine;

namespace Szczepanski.ScriptGenerator
{
    public class ScriptGenerator
    {
        public static void GenerateCSFile(ScriptGeneratorSettings settings, string filepath)
        {
            if (File.Exists($"{filepath}\\{settings.@class.name}_AutoGen.cs")) { return; }
            File.WriteAllText($"{filepath}\\{settings.@class.name}_AutoGen.cs", GenerateCode(settings));
        }
        public static string GenerateCode(ScriptGeneratorSettings settings)
        {
            CodeBuilder code = new CodeBuilder();

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
            //Class
            code.AppendLine(settings.@class.ToString());

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

    [System.Serializable]
    public struct Class
    {
        public string atributes;
        public string name;
        public string baseClass;
        public string[] interfaces;
        public Property[] properties;
        public Function[] functions;

        public static Class Empty => new Class() { atributes = string.Empty, name = string.Empty, baseClass = string.Empty, interfaces = new string[0], properties = new Property[0], functions = new Function[0] };
        public override string ToString()
        {
            CodeBuilder code = new CodeBuilder();
            code.AppendLine(atributes);
            code.Append($"public class {name}");
            if (baseClass != string.Empty)
            {
                code.Append($" : {baseClass}");
            }
            if (baseClass != string.Empty)
            {
                code.Append($", ");
            }
            else
            {
                code.Append($" : ");
            }

            foreach (var item in interfaces)
            {
                code.Append($"{item}, ");
            }
            code.Remove(code.lineLength - 2,2);

            code.AppendLine();
            code.OpenBody();

            foreach(Property property in properties)
            {
                code.AppendLine(property.ToString());
            }
            foreach (Function function in functions)
            {
                code.AppendLine(function.ToString());
            }

            return code.ToString().Trim('\n');
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
        public bool isOverride;
        public string getterBody;
        public Accessibility accessibility;

        public static Property Empty => new Property() { accessibility = Accessibility.Public }; 

        public override string ToString()
        {
            CodeBuilder code = new CodeBuilder();
            code.AppendLine($"{accessibility.ToString().ToLower()} {(isOverride ? "override" : string.Empty)} {type} {name} {{ {getter} {setter} }}");
            if (!hasBackingField) { return code.ToString().Trim('\n'); }
            code.AppendLine($"private {type} m_{name}");
            return code.ToString().Trim('\n');
        }


        private readonly string getter => $"get =>  {(hasBackingField ? $"m_{name}" : (getterBody == string.Empty ? "throw new System.NotImplementedException();" : getterBody))}";
        private readonly string setter => isOnlyGetter ? string.Empty : $"{(isPrivateSetter && accessibility == Accessibility.Public ? "private" : string.Empty)} set => m_{name} = value;";
    }

    [System.Serializable]
    public struct Function
    {
        public string name;
        [Multiline]
        public string body;
        public string type;
        public Accessibility accessibility;
        public bool isOverride;

        public static Function Empty => new Function();

        public override string ToString()
        {
            CodeBuilder code = new CodeBuilder();
            code.AppendLine($"{accessibility.ToString().ToLower()} {(isOverride ? "override" : string.Empty)} {type} {name}()");
            code.OpenBody();
            code.AppendLine(body != string.Empty ? body : "throw new System.NotImplementedException();");
            return code.ToString().Trim('\n');
        }
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
        public int lineLength => line.Length;
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

        public void Remove(int startIndex, int length)
        { 
            line.Remove(startIndex, length);
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
            return code.ToString().Trim('\n');
        }
    }
}
