using UnityEditor;

namespace FieldEditorTool
{
    internal static class FieldEditorWindow
    {
        static FieldEditor fieldEditor;
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
