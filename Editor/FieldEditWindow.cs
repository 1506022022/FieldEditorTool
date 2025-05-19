using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal class FieldEditWindow : IFieldEditorWindow
    {
        void IFieldEditorWindow.ShowWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/AI/Navigation");

            var navigationWindow = EditorWindow.focusedWindow;
            var navigationRect = navigationWindow.position;
            navigationRect.position = Vector3.zero;
            navigationWindow.position = navigationRect;

            Rect nextPosition = new Rect(
                navigationRect.xMax + 10,
                navigationRect.y,
                navigationRect.width,
                navigationRect.height
            );

            var navigationExpansionWindow = ScriptableObjectEditorWindow.ShowWindow(FieldEditorUtility.GetCustomNavigationAreas());
            var expansionPosition = navigationExpansionWindow.position;
            nextPosition.width = expansionPosition.width;
            navigationExpansionWindow.position = nextPosition;

            EditorWindow.GetWindow<FieldEditor>(nameof(FieldEditor));
        }
    }
}
