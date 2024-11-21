using Sirenix.Utilities;
using System;
using System.Text;

namespace Szczepanski.ScriptGenerator
{
    public class ItemSOGenerator
    {
        public static ScriptGeneratorSettings ToSettingsStruct(ItemTags tags)
        {
            ScriptGeneratorSettings settings = ScriptGeneratorSettings.Empty;

            //Used for name generation
            Type[] interfacesTypes = ItemTagsUtils.TagsToTypes(tags);
            string[] interfacesNames = new string[interfacesTypes.Length];
            for (int i = 0; i < interfacesTypes.Length; i++)
            {
                interfacesNames[i] = interfacesTypes[i].Name;
            }
            interfacesNames.Sort();

            StringBuilder sb = new StringBuilder();
            sb.Append("Item ");
            if(interfacesNames.Length == 0 ) { sb.Append("Basic"); }
            for(int i = 0; i < interfacesTypes.Length; i++)
            {
                char[] copy = new char[interfacesNames[i].Length-3];
                interfacesNames[i].CopyTo(1, copy, 0, interfacesNames[i].Length-3);
                sb.Append(copy);
                sb.Append(" ");
            }
            sb.Append("SO");


            //Class
            settings.@class = new Class()
            {
                atributes = $"[CreateAssetMenu(fileName = \"New {sb}\", menuName = \"Item/{sb}\")]",
                name = sb.ToString().Replace(" ", ""),
                baseClass = "ItemBaseSO",
                interfaces = interfacesNames,
                functions = new Function[0],
                properties = new Property[0]
            };

            sb.Clear();

            //Property body builder
            if(interfacesNames.Length > 0)
            {
                foreach (var @interface in interfacesNames)
                {
                    sb.Append($"{@interface}.TAG | ");
                }
                sb.Remove(sb.Length - 3, 3);
            }
            
            //Properties
            settings.@class.properties = new Property[1]
            {
                new Property()
                {
                    name = "Tag",
                    type = "ItemTags",
                    accessibility = Accessibility.Public,
                    hasBackingField = false,
                    isOnlyGetter = true,
                    isOverride = true,
                    getterBody = sb.ToString(),
                }
            };

            //Function

            settings.@class.functions = new Function[1]
            {
                new Function()
                {
                    accessibility = Accessibility.Public,
                    name = "CreateItem",
                    isOverride = true,
                    type = "ItemBase",
                    body = $"return new {settings.@class.name.Remove(settings.@class.name.Length - 2, 2)}(this);"
                }
            };
            
            return settings;
        }
    }
}