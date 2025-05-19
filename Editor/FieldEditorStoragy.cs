using PlasticGui.WorkspaceWindow;
using System.Collections.Generic;

namespace FieldEditorTool
{
    internal static class FieldEditorStorage
    {
        static readonly FieldEditWindow editWindow = new();
        static readonly FieldEditorSettingsSection settingsSection = new();
        static readonly NavigationSection navigationSection = new();
        static readonly FieldEditorFileManager fileManager = new();

        internal static IFieldEditorWindow GetWindow(FieldEditorSettings settings)
        {
            return editWindow;
        }

        internal static HashSet<IFieldEditorUI> GetUIs(FieldEditorSettings settings)
        {
            var elements = new HashSet<IFieldEditorUI>();
            elements.Add(settingsSection);
            AddNavigationIfNeeded(elements, settings);
            elements.Add(fileManager);
            return elements;
        }

        internal static HashSet<IFieldEditorInitialize> GetInitializes(FieldEditorSettings settings)
        {
            var elements = new HashSet<IFieldEditorInitialize>();
            AddNavigationIfNeeded(elements, settings);
            return elements;
        }

        internal static HashSet<IFieldEditorDispose> GetDisposers(FieldEditorSettings settings)
        {
            var elements = new HashSet<IFieldEditorDispose>();
            AddNavigationIfNeeded(elements, settings);
            return elements;
        }

        internal static HashSet<IFieldEditorElement> GetElements(FieldEditorSettings settings)
        {
            var elements = new HashSet<IFieldEditorElement>();
            AddNavigationIfNeeded(elements, settings);
            return elements;
        }

        internal static HashSet<IFieldEditorFile> GetFieldEditorFiles(FieldEditorSettings settings)
        {
            var elements = new HashSet<IFieldEditorFile>();
            AddNavigationIfNeeded(elements, settings);
            return elements;
        }

        internal static FieldEditorFileManager GetFileManager()
        {
            return fileManager;
        }

        static void AddNavigationIfNeeded<T>(HashSet<T> elements, FieldEditorSettings settings) where T : class
        {
            if (settings != null && settings.UseNavigation && navigationSection is T nav)
            {
                elements.Add(nav);
            }
        }
    }
}
