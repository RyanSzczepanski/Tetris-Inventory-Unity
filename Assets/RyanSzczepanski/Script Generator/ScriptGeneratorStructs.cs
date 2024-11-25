using UnityEngine;

namespace Szczepanski.ScriptGenerator
{
    [System.Serializable]
    public struct Class
    {
        public string atributes;
        public string name;
        public string baseClass;
        public Interface[] interfaces;
        public Property[] properties;
        public Function[] functions;

        public static Class Empty => new Class() { atributes = string.Empty, name = string.Empty, baseClass = string.Empty, interfaces = new Interface[0], properties = new Property[0], functions = new Function[0] };
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

            foreach (Interface @interface in interfaces)
            {
                code.Append($"{@interface.name}, ");
            }
            code.Remove(code.lineLength - 2, 2);

            code.AppendLine();
            code.OpenBody();
            //Props
            code.AppendLine($"#region Properties");
            foreach (Interface @interface in interfaces)
            {
                if (@interface.properties.Length > 0) { code.AppendLine($"#region {@interface.name}"); }
                foreach (Property property in @interface.properties)
                {
                    code.AppendLine(property.ToString().Trim('\n'));
                }
                if (@interface.properties.Length > 0) { code.AppendLine($"#endregion"); }
            }
            if (functions.Length > 0) { code.AppendLine($"#region Base"); }
            foreach (Property property in properties)
            {
                code.AppendLine(property.ToString().Trim('\n'));
            }
            if (functions.Length > 0) { code.AppendLine($"#endregion"); }
            code.AppendLine($"#endregion");

            //Funcs
            code.AppendLine($"#region Functions");
            foreach (Interface @interface in interfaces)
            {
                if (@interface.functions.Length > 0) { code.AppendLine($"#region {@interface.name}"); }
                foreach (Function function in @interface.functions)
                {
                    code.AppendLine(function.ToString().Trim('\n'));
                }
                if (@interface.functions.Length > 0) { code.AppendLine($"#endregion"); }
            }
            if (functions.Length > 0) { code.AppendLine($"#region Base"); }
            foreach (Function function in functions)
            {
                code.AppendLine(function.ToString());
            }
            if (functions.Length > 0) { code.AppendLine($"#endregion"); }
            code.AppendLine($"#endregion");

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
        public bool isOverride;
        public bool hasBackingField;
        public bool isBackingFieldSerialized;
        public string getterBody;
        public Accessibility accessibility;

        public static Property Empty => new Property() { accessibility = Accessibility.Public };

        public override string ToString()
        {
            CodeBuilder code = new CodeBuilder();
            code.AppendLine($"{accessibility.ToString().ToLower()} {(isOverride ? "override " : string.Empty)}{type} {name} {{ {getter}{(setter != string.Empty ? " " : string.Empty)}{setter} }}");
            if (!hasBackingField) { return code.ToString().Trim('\n'); }
            code.AppendLine($"{(isBackingFieldSerialized ? "[SerializeField]" : string.Empty)} private {type} m_{name};");
            return code.ToString().Trim('\n');
        }

        private readonly string getter => $"get => {(hasBackingField ? $"m_{name};" : (getterBody == string.Empty ? "throw new System.NotImplementedException();" : getterBody))}";
        private readonly string setter => isOnlyGetter ? string.Empty : $"{(isPrivateSetter && accessibility == Accessibility.Public ? "private" : string.Empty)} set => m_{name} = value;";
    }

    [System.Serializable]
    public struct Function
    {
        public string signature;
        [Multiline]
        public string body;

        public string name;
        public string type;
        public Accessibility accessibility;
        public bool isOverride;

        public static Function Empty => new Function();

        public string GetSignature => signature == string.Empty ? $"{accessibility.ToString().ToLower()} {(isOverride ? "override " : string.Empty)}{type} {name}()" : signature;

        public override string ToString()
        {
            CodeBuilder code = new CodeBuilder();
            code.AppendLine($"{GetSignature}");
            code.OpenBody();
            code.AppendLine(body != string.Empty ? body : "throw new System.NotImplementedException();");
            return code.ToString().Trim('\n');
        }
    }

    [System.Serializable]
    public struct Interface
    {
        public string name;
        public Property[] properties;
        public Function[] functions;

        public Interface Empty => new Interface() { name = string.Empty, properties = new Property[0], functions = new Function[0] };
    }
}