using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    public class ScriptableObjectEditorWindow : EditorWindow
    {
        ScriptableObject targetObject;
        Editor objectEditor;

        public static EditorWindow ShowWindow(ScriptableObject obj)
        {
            var window = CreateInstance<ScriptableObjectEditorWindow>();
            window.titleContent = new GUIContent(obj.name);
            window.targetObject = obj;
            window.Show();

            return window;
        }

        void OnEnable()
        {
            if (targetObject != null)
            {
                objectEditor = Editor.CreateEditor(targetObject);
            }
        }

        void OnGUI()
        {
            if (targetObject == null)
            {
                EditorGUILayout.LabelField("No ScriptableObject assigned.");
                return;
            }

            if (objectEditor == null)
            {
                objectEditor = Editor.CreateEditor(targetObject);
            }

            EditorGUI.BeginChangeCheck();
            objectEditor.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(targetObject);
            }
        }
    }

}
