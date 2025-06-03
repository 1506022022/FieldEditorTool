using UnityEditor;

namespace FieldEditorTool
{
    internal static class FieldEditorWindow
    {
        static FieldEditor fieldEditor;
        static readonly NavigationWindowGUI navigationSection = new();
        static readonly AreaWindowGUI areaSection = new();
        static readonly SceneWindowGUI sceneSection = new();
        static void ShowWindow()
        {
            fieldEditor = EditorWindow.GetWindow<FieldEditor>(nameof(FieldEditor));
            fieldEditor.minSize = new(300, 400);
        }
        static void CloseWindow()
        {
            if (!fieldEditor) return;
            fieldEditor.Close();
            fieldEditor.OnDisable();
        }

        [MenuItem("FieldEditor/AreaEditor")]
        static void ShowAreaEditor()
        {
            CloseWindow();

            FieldEditor.Initializes.Clear();
            FieldEditor.Initializes.Add(areaSection);

            FieldEditor.UIes.Clear();
            FieldEditor.UIes.Add(areaSection);
            FieldEditor.UIes.Add(FieldEditorFileManager.Instance);

            FieldEditor.Disposers.Clear();
            FieldEditor.Disposers.Add(areaSection);

            FieldEditor.Elements.Clear();
            FieldEditor.Elements.Add(areaSection);

            FieldEditor.Files.Clear();
            FieldEditor.Files.Add(areaSection);

            ShowWindow();
        }


        [MenuItem("FieldEditor/CellEditor")]
        static void ShowCellEditor()
        {
            CloseWindow();

            FieldEditor.Initializes.Clear();
            FieldEditor.Initializes.Add(navigationSection);

            FieldEditor.UIes.Clear();
            FieldEditor.UIes.Add(navigationSection);
            FieldEditor.UIes.Add(FieldEditorFileManager.Instance);

            FieldEditor.Disposers.Clear();
            FieldEditor.Disposers.Add(navigationSection);

            FieldEditor.Elements.Clear();
            FieldEditor.Elements.Add(navigationSection);

            FieldEditor.Files.Clear();
            FieldEditor.Files.Add(navigationSection);

            ShowWindow();
        }

        [MenuItem("FieldEditor/SceneEditor")]
        static void ShowSceneEditor()
        {
            CloseWindow();

            FieldEditor.Initializes.Clear();

            FieldEditor.UIes.Clear();
            FieldEditor.UIes.Add(sceneSection);
            FieldEditor.UIes.Add(FieldEditorFileManager.Instance);

            FieldEditor.Disposers.Clear();

            FieldEditor.Elements.Clear();
            FieldEditor.Elements.Add(sceneSection);

            FieldEditor.Files.Clear();

            ShowWindow();
        }
    }
}
