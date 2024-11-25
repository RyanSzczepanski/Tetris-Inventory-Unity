using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Szczepanski.ScriptGenerator
{
    public class ScriptGenerator
    {
        public const string AUTO_GEN_LABLE = "\n///------------------------------------///\n///    This File Was Auto Generated    ///\n/// Using Szczepanski Script Generator ///\n///------------------------------------///\n";

        public static void GenerateCSFile(ScriptGeneratorSettings settings, string filepath)
        {
            File.WriteAllText($"{filepath}\\{settings.@class.name}.cs", GenerateCode(settings));
            AssetDatabase.Refresh();
        }
        public static string GenerateCode(ScriptGeneratorSettings settings)
        {
            CodeBuilder code = new CodeBuilder();
            code.AppendLine(AUTO_GEN_LABLE);

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

    public enum Accessibility
    {
        Public = 0,
        Private = 1,
        Protected = 2,

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
            code.Append($"{line}\n");
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
