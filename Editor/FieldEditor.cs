using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal class FieldEditor : EditorWindow
    {
        static FieldEditorSettings settings => FieldEditorSettings.Instance;
        static HashSet<IFieldEditorInitialize> initializes = new();
        static HashSet<IFieldEditorUI> uis = new();
        static HashSet<IFieldEditorDispose> disposers = new();
        static HashSet<IFieldEditorElement> elements = new();
        static HashSet<IFieldEditorFile> files = new();

        [MenuItem("Tools/FieldEditor")]
        static void ShowWindow()
        {
            var window = FieldEditorStorage.GetWindow(settings);
            window.ShowWindow();
        }

        void OnGUI()
        {
            foreach (var ui in uis)
            {
                ui.OnGUI();
                GUILayout.Space(20);
            }
        }

        void OnEnable()
        {
            UpdateWindow();
            Initialize();
            var fileManager = FieldEditorStorage.GetFileManager();

            fileManager.OnClickSaveButton -= Save;
            fileManager.OnClickSaveButton += Save;

            fileManager.OnClickReadButton -= OnLoadFile;
            fileManager.OnClickReadButton += OnLoadFile;

            settings.OnValidateEvent -= OnValidateSettings;
            settings.OnValidateEvent += OnValidateSettings;
        }

        void Save()
        {
            var json = elements.Select(x => x.GetJson()).ToArray();
            FieldEditorStorage.GetFileManager().WriteJson(string.Join('\n', json), settings.SavePath);
        }

        void OnValidateSettings()
        {
            UpdateWindow();
            Initialize();
        }

        void OnLoadFile(string path)
        {
            var data = File.ReadAllLines(path);
            var list = new List<AreaType>();

            for (int i = 0; i < data.Length; i++)
            {
                var type = JsonUtility.FromJson<AreaType>(data[i]).HeaderType;
                list.Add((AreaType)JsonUtility.FromJson(data[i], AreaType.FindTypeByName(type)));
            }

            foreach (var item in files)
            {
                item.OnReadFile(list);
            }
        }

        void UpdateWindow()
        {
            uis = FieldEditorStorage.GetUIs(settings);
            disposers = FieldEditorStorage.GetDisposers(settings);
            elements = FieldEditorStorage.GetElements(settings);
            files = FieldEditorStorage.GetFieldEditorFiles(settings);
            initializes = FieldEditorStorage.GetInitializes(settings);
        }

        void Initialize()
        {
            foreach (var element in initializes)
            {
                element.Initialize();
            }
        }

        void OnDisable()
        {
            Dispose();
            settings.OnValidateEvent -= OnValidateSettings;
        }

        void Dispose()
        {
            foreach (var element in disposers)
            {
                element.Dispose();
            }
        }
    }
}