using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FieldEditorTool
{
    [CustomEditor(typeof(CustomNavigationAreas))]
    internal class CustomNavigationAreasEditor : Editor
    {
        string[] areaTypeNames;
        SerializedProperty areasProperty;
        ReorderableList areasList;
        Vector2 scrollPosition;

        const int DEFAULT = 0;
        const int NON_WALKABLE = 1;
        const int JUMP = 2;

        static readonly GUIContent nameLabel = EditorGUIUtility.TrTextContent("Name");
        static readonly GUIContent typeLabel = EditorGUIUtility.TrTextContent("Type");

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(13);
            EditorGUILayout.LabelField("Navigation Modifier", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            var so = FieldEditorUtility.GetCustomNavigationAreas().CreateSerializedObject();
            so.Update();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            areasList.DoLayoutList();
            EditorGUILayout.EndScrollView();
            so.ApplyModifiedProperties();
        }

        void OnEnable()
        {
            areaTypeNames = AreaType.GetDerivedTypeNames();
            InitializeAreasList();
        }

        void InitializeAreasList()
        {
            areasProperty = FieldEditorUtility.GetCustomNavigationAreas().GetProperty();
            areasList = new ReorderableList(FieldEditorUtility.GetCustomNavigationAreas().CreateSerializedObject(), areasProperty, false, true, false, false);

            areasList.drawElementCallback = DrawAreaListElement;
            areasList.drawHeaderCallback = DrawAreaListHeader;
            areasList.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        void DrawAreaListHeader(Rect rect)
        {
            GetAreaListRects(rect, out Rect nameRect, out Rect typeRect);
            GUI.Label(nameRect, nameLabel);
            GUI.Label(typeRect, typeLabel);
        }

        void DrawAreaListElement(Rect rect, int index, bool selected, bool focused)
        {
            rect.height -= 2;

            bool allowChangeName = index switch
            {
                DEFAULT => false,
                NON_WALKABLE => false,
                JUMP => false,
                _ => true,
            };

            bool allowChangeType = index switch
            {
                NON_WALKABLE => false,
                _ => true,
            };

            GetAreaListRects(rect, out Rect nameRect, out Rect typeRect);

            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            bool guiEnabledBackup = GUI.enabled;

            GUI.enabled = guiEnabledBackup && allowChangeName;
            EditorGUI.LabelField(nameRect, $"area {index}");

            GUI.enabled = guiEnabledBackup && allowChangeType;

            var navData = (CustomNavigationAreas)target;
            int currentIndex = Mathf.Max(0, Array.IndexOf(areaTypeNames, navData.AreaTypes[index]));
            int selectedIndex = EditorGUI.Popup(typeRect, currentIndex, areaTypeNames);

            if (index == NON_WALKABLE)
            {
                selectedIndex = 1;
            }

            if (selectedIndex != currentIndex)
            {
                Undo.RecordObject(navData, "Modify TypeField");
                navData.AreaTypes[index] = areaTypeNames[selectedIndex];
                EditorUtility.SetDirty(navData);
            }

            GUI.enabled = guiEnabledBackup;
            EditorGUI.indentLevel = oldIndent;
        }

        static void GetAreaListRects(Rect rect, out Rect nameRect, out Rect typeRect)
        {
            float typeWidth = EditorGUIUtility.singleLineHeight * 4;
            float nameWidth = rect.width - typeWidth;

            nameRect = new Rect(rect.x, rect.y, nameWidth - 4, rect.height);
            typeRect = new Rect(rect.x + nameWidth, rect.y, typeWidth, rect.height);
        }
    }
}
