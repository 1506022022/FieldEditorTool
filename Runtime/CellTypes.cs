using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FieldEditorTool
{
    public class AreaType
    {
        [HideInInspector] public Vector2Int Index;
        [HideInInspector] public string HeaderType;

        public static string[] GetDerivedTypeNames()
        {
            return GetDerivedTypes()
                .Select(type => type.Name)
                .ToArray();
        }

        public static Type[] GetDerivedTypes()
        {
            var baseType = typeof(AreaType);
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

        public static Type FindTypeByName(string typeName)
        {
            return GetDerivedTypes()
                .FirstOrDefault(type => type.Name == typeName);
        }
    }

    [Serializable]
    public class Walkable : AreaType
    {
        public GameObject Who;
        public Vector3 Where;
        public float When;
    }

    [Serializable]
    public class NotWalkable : AreaType
    {
        public string Why;
    }
}