using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal class FieldEditor : EditorWindow
    {
        public static HashSet<IFieldEditorInitialize> Initializes { get; private set; } = new();
        public static HashSet<IFieldEditorUI> UIes { get; private set; } = new();
        public static HashSet<IFieldEditorDispose> Disposers { get; private set; } = new();
        public static HashSet<IFieldEditorElement> Elements { get; private set; } = new();
        public static HashSet<IFieldEditorFile> Files { get; private set; } = new();

        void OnGUI()
        {
            foreach (var ui in UIes)
            {
                ui.OnGUI();
                GUILayout.Space(50);
            }
        }

        public void OnEnable()
        {
            Initialize();
            var fileManager = FieldEditorFileManager.Instance;

            fileManager.OnClickSaveButton -= Save;
            fileManager.OnClickSaveButton += Save;

            fileManager.OnClickReadButton -= OnLoadFile;
            fileManager.OnClickReadButton += OnLoadFile;
        }

        void Save()
        {
            var json = Elements.Select(x => x.GetJson()).ToArray();
            FieldEditorFileManager.Instance.WriteJson(string.Join('\n', json), FieldEditorSettings.Instance.SavePath);
        }

        void OnLoadFile(string path)
        {
            string[] data = { };
            try
            {
                data = File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }

            var list = new List<EntityData>();

            for (int i = 0; i < data.Length; i++)
            {
                var headerType = JsonUtility.FromJson<EntityData>(data[i]).HeaderType;
                var castType = Types.FindTypeByName<EntityData>(headerType);
                list.Add((EntityData)JsonUtility.FromJson(data[i], castType));
            }

            foreach (var item in Files)
            {
                item.OnReadFile(list);
            }
        }

        void Initialize()
        {
            foreach (var element in Initializes)
            {
                element.Initialize();
            }
        }

        public void OnDisable()
        {
            Dispose();
        }

        void Dispose()
        {
            foreach (var element in Disposers)
            {
                element.Dispose();
            }
        }
    }
}