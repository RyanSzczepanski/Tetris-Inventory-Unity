using Sirenix.Utilities;
using System;
using System.Data.Common;
using System.Text;
using UnityEngine;

namespace Szczepanski.ScriptGenerator
{
    public class ItemSOGenerator
    {
        public static ScriptGeneratorSettings ToSettingsStruct(ItemTags tags)
        {
            ScriptGeneratorSettings settings = ScriptGeneratorSettings.Empty;
            ScriptGeneratorDB.Init();

            settings.usings = new string[1]
            {
                "UnityEngine"
            };

            //Used for name generation
            Type[] interfacesTypes = ItemTagsUtils.TagsToTypes(tags);
            string[] interfacesNames = new string[interfacesTypes.Length];
            Interface[] interfaces = new Interface[interfacesTypes.Length];
            for (int i = 0; i < interfacesTypes.Length; i++)
            {
                interfacesNames[i] = interfacesTypes[i].Name;
            }
            interfacesNames.Sort();

            StringBuilder tagsSpaceSeperated = new StringBuilder();
            tagsSpaceSeperated.Append("Item ");
            if(interfacesNames.Length == 0 ) { tagsSpaceSeperated.Append("Basic"); }
            for(int i = 0; i < interfacesTypes.Length; i++)
            {
                interfaces[i] = ScriptGeneratorDB.GetObjectByName(interfacesNames[i]).AsInterface;
                char[] shortenedInterfaceName = new char[interfacesNames[i].Length-3];
                interfacesNames[i].CopyTo(1, shortenedInterfaceName, 0, interfacesNames[i].Length-3);
                tagsSpaceSeperated.Append(shortenedInterfaceName);
                tagsSpaceSeperated.Append(" ");
            }
            tagsSpaceSeperated.Append("SO");


            //Class
            settings.@class = new Class()
            {
                atributes = $"[CreateAssetMenu(fileName = \"New {tagsSpaceSeperated}\", menuName = \"Item/{tagsSpaceSeperated}\")]",
                name = tagsSpaceSeperated.ToString().Replace(" ", ""),
                baseClass = "ItemBaseSO",
                interfaces = interfaces,
                functions = new Function[0],
                properties = new Property[0]
            };

            tagsSpaceSeperated.Clear();

            //Property body builder
            if(interfacesNames.Length > 0)
            {
                foreach (var @interface in interfacesNames)
                {
                    tagsSpaceSeperated.Append($"{@interface}.TAG | ");
                }
                tagsSpaceSeperated.Remove(tagsSpaceSeperated.Length - 3, 3);
                tagsSpaceSeperated.Append(';');
            }
            
            //Properties
            settings.@class.properties = new Property[1]
            {
                new Property()
                {
                    name = "Tags",
                    type = "ItemTags",
                    accessibility = Accessibility.Public,
                    hasBackingField = false,
                    isOnlyGetter = true,
                    isOverride = true,
                    getterBody = tagsSpaceSeperated.ToString(),
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