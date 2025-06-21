using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    [CustomEditor(typeof(DataComponent))]
    internal class DataEditor : Editor
    {
        FieldInfo[] fields;
        string[] entityNames;
        Type[] entityTypes;
        DataComponent data;

        void OnEnable()
        {
            data = (DataComponent)target;
            entityNames = Types.GetDerivedTypeNames<EntityData>();
            entityTypes = Types.GetDerivedTypes<EntityData>();

            var type = Types.FindTypeByName<EntityData>(data.HeaderType);
            fields = type != null ? GetFieldsRootFirst(type) : Array.Empty<FieldInfo>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(25);

            int currentTypeIndex = Mathf.Max(Array.IndexOf(entityNames, data.HeaderType), 0);
            currentTypeIndex = Mathf.Max(EditorGUILayout.Popup("Entity Type", currentTypeIndex, entityNames), 0);
            data.HeaderType = entityTypes[currentTypeIndex].Name;

            EditorGUILayout.Space(15);

            if (data.Data == null || data.Data.GetType().Name != data.HeaderType)
            {
                if (TryCreateAreaInstance(data.HeaderType, out object newInstance))
                {
                    data.Data = (EntityData)newInstance;

                    var type = Types.FindTypeByName<EntityData>(data.HeaderType);
                    fields = type != null ? GetFieldsRootFirst(type) : Array.Empty<FieldInfo>();

                    EditorUtility.SetDirty(target);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Unable to instantiate class '{data.HeaderType}'", MessageType.Error);
                    return;
                }
            }

            EditorGUILayout.LabelField($"Custom Fields for {data.HeaderType}", EditorStyles.boldLabel);

            DrawCustomFields();
        }

        bool TryCreateAreaInstance(string typeName, out object instance)
        {
            instance = null;
            try
            {
                var type = Types.FindTypeByName<EntityData>(typeName);
                instance = Activator.CreateInstance(type);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void DrawCustomFields()
        {
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<HideInInspector>() != null)
                    continue;

                object value = field.GetValue(data.Data);
                Type fieldType = field.FieldType;

                EditorGUI.BeginChangeCheck();

                value = fieldType switch
                {
                    Type t when t == typeof(int) => EditorGUILayout.IntField(field.Name, (int)value),
                    Type t when t == typeof(float) => EditorGUILayout.FloatField(field.Name, (float)value),
                    Type t when t == typeof(string) => EditorGUILayout.TextField(field.Name, (string)value),
                    Type t when t == typeof(bool) => EditorGUILayout.Toggle(field.Name, (bool)value),
                    Type t when t.IsEnum => EditorGUILayout.EnumPopup(field.Name, (Enum)value),
                    Type t when t == typeof(GameObject) => EditorGUILayout.ObjectField(field.Name, (GameObject)value, typeof(GameObject), true),
                    Type t when t == typeof(Vector3) => EditorGUILayout.Vector3Field(field.Name, (Vector3)value),
                    Type t when t == typeof(Vector3Int) => EditorGUILayout.Vector3IntField(field.Name, (Vector3Int)value),
                    Type t when t == typeof(LayerMask) => (LayerMask)(EditorGUILayout.LayerField(field.Name, ((LayerMask)value).value)),
                    Type t when t == typeof(Quaternion) => EditorGUILayout.Vector4Field(field.Name, ((Quaternion)value).eulerAngles),
                    _ => DrawUnsupportedField(field.Name, fieldType)
                };

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(data, "Edit EntityData Field");

                    // 타입 호환성 검사
                    if (field.FieldType.IsAssignableFrom(value?.GetType()))
                        field.SetValue(data.Data, value);
                }
            }
        }

        object DrawUnsupportedField(string fieldName, Type fieldType)
        {
            EditorGUILayout.LabelField(fieldName, $"Unsupported type: {fieldType.Name}");
            return null;
        }

        /// <summary>
        /// 루트 클래스부터 자식 클래스 순서대로 필드를 반환
        /// </summary>
        static FieldInfo[] GetFieldsRootFirst(Type type)
        {
            var result = new List<FieldInfo>();
            var stack = new Stack<Type>();

            while (type != null && type != typeof(object))
            {
                stack.Push(type);
                type = type.BaseType;
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                var fields = current.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                result.AddRange(fields);
            }

            return result.ToArray();
        }
    }
}
