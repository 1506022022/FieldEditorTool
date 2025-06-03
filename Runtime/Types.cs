using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FieldEditorTool
{
    public static class Types
    {
        public static string[] GetDerivedTypeNames<T>() where T : class
        {
            return GetDerivedTypes<T>()
                .Select(type => type.Name)
                .ToArray();
        }

        public static Type[] GetDerivedTypes<T>() where T : class
        {
            var baseType = typeof(T);
            var baseAssembly = baseType.Assembly;

            var assemblies = new List<Assembly> { baseAssembly };
            assemblies.AddRange(
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a != baseAssembly)
            );

            return assemblies
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t != null);
                    }
                })
                .Where(type => type.IsClass
                            && !type.IsAbstract
                            && type.IsSubclassOf(baseType))
                .ToArray();

        }

        public static Type FindTypeByName<T>(string typeName) where T : class
        {
            return GetDerivedTypes<T>()
                .FirstOrDefault(type => type.Name == typeName);
        }
    }

    [Serializable]
    public class EntityData
    {
        [HideInInspector] public string HeaderType;
    }
    
}