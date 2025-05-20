using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal class FieldEditWindow : IFieldEditorWindow, IFieldEditorUI, IFieldEditorDispose
    {
        EditorWindow fieldEditor;
        EditorWindow navigation;
        EditorWindow subWindow;
        void IFieldEditorWindow.ShowWindow()
        {
            fieldEditor = EditorWindow.GetWindow<FieldEditor>(nameof(FieldEditor));
            fieldEditor.minSize = new(300, 400);
        }

        public void Dispose()
        {
            CloseSide();
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (Style.Button(nameof(OpenSide)))
            {
                OpenSide();
                fieldEditor.Focus();
            }
            if (Style.Button(nameof(CloseSide)))
            {
                CloseSide();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void CloseSide()
        {
            navigation?.Close();
            subWindow?.Close();
            navigation = null;
            subWindow = null;
        }
        void OpenSide()
        {
            CloseSide();
            EditorApplication.ExecuteMenuItem("Window/AI/Navigation");
            Rect nextPosition = new Rect();
            if (EditorWindow.focusedWindow.titleContent.text == "Navigation")
            {
                navigation = EditorWindow.focusedWindow;
                var navigationRect = navigation.position;
                navigationRect.position = Vector3.zero;
                navigation.position = navigationRect;

                nextPosition = new Rect(
                    navigationRect.xMax + 10,
                    navigationRect.y,
                    navigationRect.width,
                    navigationRect.height
                );
            }

            subWindow = ScriptableObjectEditorWindow.ShowWindow(FieldEditorUtility.GetCustomNavigationAreas());
            var expansionPosition = subWindow.position;
            nextPosition.width = expansionPosition.width;
            subWindow.position = nextPosition;
            subWindow.name = nameof(NavigationAreasCustomData);
        }

        Rect GetSidePosition()
        {
            Rect sideRect = new Rect();
            var navigationRect = navigation.position;
            var subWindowRect = subWindow.position;

            sideRect = new Rect(
                navigationRect.xMax + 10,
                navigationRect.y,
                navigationRect.width,
                navigationRect.height
            );

            sideRect = new Rect(

                subWindowRect.xMin - 10,
                navigationRect.y,
                navigationRect.width,
                navigationRect.height
            );
            return sideRect;
        }
    }
}
