using System;
using System.Linq;
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
            return typeof(AreaType).Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(AreaType)))
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