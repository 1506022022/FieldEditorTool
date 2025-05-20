using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    [CustomEditor(typeof(DataComponent))]
    internal class DataEditor : Editor
    {
        FieldInfo[] areaFields;
        string[] areaTypeNames;
        Type[] areaTypes;
        DataComponent cellData;

        void OnEnable()
        {
            areaTypeNames = Types.GetDerivedTypeNames<AreaData>();
            cellData = (DataComponent)target;
            areaTypes = Types.GetDerivedTypes<AreaData>();
            areaFields = Types.FindTypeByName<AreaData>(cellData.HeaderType)?.GetFields(BindingFlags.Public | BindingFlags.Instance);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = false;
            EditorGUILayout.Vector2Field(nameof(cellData.Index), cellData.Index);
            GUI.enabled = true;

            EditorGUILayout.Space(25);

            int currentTypeIndex = Mathf.Max(Array.IndexOf(areaTypeNames, cellData.HeaderType), 0);
            currentTypeIndex = Mathf.Max(EditorGUILayout.Popup("Area Type", currentTypeIndex, areaTypeNames), 0);
            cellData.HeaderType = areaTypes[currentTypeIndex].Name;

            EditorGUILayout.Space(15);

            if (cellData.Area == null || cellData.Area.GetType().Name != cellData.HeaderType)
            {
                if (TryCreateAreaInstance(cellData.HeaderType, out object newAreaInstance))
                {
                    cellData.Area = (AreaData)newAreaInstance;
                    areaFields = Types.FindTypeByName<AreaData>(cellData.HeaderType).GetFields(BindingFlags.Public | BindingFlags.Instance);
                    EditorUtility.SetDirty(target);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Unable to instantiate class '{cellData.HeaderType}'", MessageType.Error);
                    return;
                }
            }

            EditorGUILayout.LabelField($"Custom Fields for {cellData.HeaderType}", EditorStyles.boldLabel);

            DrawCustomFields();
        }

        bool TryCreateAreaInstance(string typeName, out object instance)
        {
            instance = null;
            try
            {
                var type = Types.FindTypeByName<AreaData>(typeName);
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
            foreach (var field in areaFields)
            {
                if (field.GetCustomAttribute<HideInInspector>() != null)
                    continue;

                object value = field.GetValue(cellData.Area);
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
                    _ => DrawUnsupportedField(field.Name, fieldType)
                };

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(cellData, "Edit Area Class Field");
                    field.SetValue(cellData.Area, value);
                }
            }
        }

        object DrawUnsupportedField(string fieldName, Type fieldType)
        {
            EditorGUILayout.LabelField(fieldName, $"Unsupported type: {fieldType.Name}");
            return null;
        }
    }
}