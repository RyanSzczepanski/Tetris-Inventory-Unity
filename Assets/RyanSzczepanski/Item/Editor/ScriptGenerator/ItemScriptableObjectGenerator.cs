using Sirenix.Utilities;
using System;
using System.Text;

namespace Szczepanski.ScriptGenerator
{
    public class ItemScriptableObjectGenerator
    {
        public static ScriptGeneratorSettings ToSettingsStruct(ItemTags tags)
        {
            ScriptableObjectDatabase<InterfaceSO>.Init();
            ScriptGeneratorSettings settings = ScriptGeneratorSettings.Empty;
            settings.usings = new string[2]
            {
                "UnityEngine",
                "System.Collections.Generic"
            };

            //Used for name generation
            Type[] interfacesTypes = ItemTagsUtils.TagsToTypes(tags, true);
            string[] interfacesNames = new string[interfacesTypes.Length];
            Interface[] interfaces = new Interface[interfacesTypes.Length];
            for (int i = 0; i < interfacesTypes.Length; i++)
            {
                interfacesNames[i] = interfacesTypes[i].Name;
            }
            interfacesNames.Sort();

            StringBuilder tagsSpaceSeperated = new StringBuilder();
            tagsSpaceSeperated.Append("Item ");
            if(interfacesNames.Length == 0 ) { tagsSpaceSeperated.Append("Basic "); }
            for(int i = 0; i < interfacesTypes.Length; i++)
            {
                interfaces[i] = ScriptableObjectDatabase<InterfaceSO>.GetObjectByName(interfacesNames[i]).AsInterface;
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
                    getterBody = tagsSpaceSeperated.ToString() == string.Empty ? "ItemTags.Basic;" : tagsSpaceSeperated.ToString()
                }
            };

            //Function

            settings.@class.functions = new Function[1]
            {
                new Function()
                {
                    //accessibility = Accessibility.Public,
                    //name = "CreateItem",
                    signature = $"public override ItemBase CreateItem()",
                    isOverride = true,
                    //type = "ItemBase",
                    body = $"return new {settings.@class.name.Remove(settings.@class.name.Length - 2, 2)}(this);"
                }
            };
            
            return settings;
        }
    }

    public class ItemObjectGenerator
    {
        public static ScriptGeneratorSettings ToSettingsStruct(ItemTags tags)
        {
            ScriptableObjectDatabase<InterfaceSO>.Init();
            ScriptGeneratorSettings settings = ScriptGeneratorSettings.Empty;
            settings.usings = new string[]
            {
                "Szczepanski.UI"
            };

            //Used for name generation
            Type[] interfacesTypes = ItemTagsUtils.TagsToTypes(tags, false);
            string[] interfacesNames = new string[interfacesTypes.Length];
            Interface[] interfaces = new Interface[interfacesTypes.Length];
            for (int i = 0; i < interfacesTypes.Length; i++)
            {
                interfacesNames[i] = interfacesTypes[i].Name;
            }
            interfacesNames.Sort();

            StringBuilder tagsSpaceSeperated = new StringBuilder();
            tagsSpaceSeperated.Append("Item ");
            if (interfacesNames.Length == 0) { tagsSpaceSeperated.Append("Basic "); }
            for (int i = 0; i < interfacesTypes.Length; i++)
            {
                interfaces[i] = ScriptableObjectDatabase<InterfaceSO>.GetObjectByName(interfacesNames[i]).AsInterface;
                char[] shortenedInterfaceName = new char[interfacesNames[i].Length - 1];
                interfacesNames[i].CopyTo(1, shortenedInterfaceName, 0, interfacesNames[i].Length - 1);
                tagsSpaceSeperated.Append(shortenedInterfaceName);
                tagsSpaceSeperated.Append(" ");
            }


            //Class
            settings.@class = new Class()
            {
                name = tagsSpaceSeperated.ToString().Replace(" ", ""),
                baseClass = "ItemBase",
                interfaces = interfaces,
                functions = new Function[0],
                properties = new Property[0]
            };

            tagsSpaceSeperated.Clear();


            //Properties
            settings.@class.properties = new Property[]
            {
                new Property()
                {
                    name = "Data",
                    type = $"new {settings.@class.name}SO",
                    accessibility = Accessibility.Public,
                    hasBackingField = false,
                    isOnlyGetter = true,
                    isInit = true,
                },
                new Property()
                {
                    name = "ContextMenuOptions",
                    type = "ContextMenuOption[]",
                    accessibility = Accessibility.Public,
                    hasBackingField = false,
                    isOnlyGetter = true,
                    isOverride = true,
                    isMultiline = true,
                    //TODO: figure out how to generate context menu options
                    getterBody = "new ContextMenuOption[]\n{\n\tnew ContextMenuOption { optionText = \"Inspect\", OnSelected = () => throw new System.NotImplementedException(\"Inspect Item Not Implemented\")},\n\tnew ContextMenuOption { optionText = \"Discard\", OnSelected = () => ParentSubInventory.TryRemoveItem(this)}\n};"
                }
            };

            //Function

            settings.@class.functions = new Function[1]
            {
                new Function()
                {
                    signature = $"public {settings.@class.name}({settings.@class.name}SO data) : base(data)",
                    body = $"Data = data;"
                }
            };

            return settings;
        }
    }
}