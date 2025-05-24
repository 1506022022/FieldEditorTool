using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal class FieldEditorFileManager : IFieldEditorUI
    {
        static FieldEditorFileManager instance;
        internal static FieldEditorFileManager Instance
        {
            get
            {
                if (instance == null) instance = new FieldEditorFileManager();
                return instance;
            }
        }
        internal event Action OnClickSaveButton;
        internal event Action<string> OnClickReadButton;
        FieldEditorFileManager() { }
        internal void WriteJson(string json, string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Debug.LogWarning($"forget path : {Path.GetDirectoryName(path)}");
                return;
            }
            File.WriteAllText(path, json);
        }
        void ReadFile()
        {
            FieldEditorSettings.Instance.SaveFolder = EditorUtility.OpenFilePanel("Read Json", FieldEditorSettings.Instance.SaveFolder, "json");
            if (string.IsNullOrEmpty(FieldEditorSettings.Instance.SaveFolder)) return;
            FieldEditorSettings.Instance.FileName = Path.GetFileNameWithoutExtension(FieldEditorSettings.Instance.SaveFolder);
            FieldEditorSettings.Instance.SaveFolder = Path.GetDirectoryName(FieldEditorSettings.Instance.SaveFolder);
            OnClickReadButton?.Invoke(FieldEditorSettings.Instance.SavePath);
        }
        void SaveFile()
        {
            FieldEditorSettings.Instance.SaveFolder = EditorUtility.SaveFilePanel("Save Json", FieldEditorSettings.Instance.SaveFolder, FieldEditorSettings.Instance.FileName, FieldEditorSettings.Instance.Extension.ToString());
            if (string.IsNullOrEmpty(FieldEditorSettings.Instance.SaveFolder)) return;
            FieldEditorSettings.Instance.FileName = Path.GetFileNameWithoutExtension(FieldEditorSettings.Instance.SaveFolder);
            FieldEditorSettings.Instance.SaveFolder = Path.GetDirectoryName(FieldEditorSettings.Instance.SaveFolder);
            OnClickSaveButton?.Invoke();
        }
        void IFieldEditorUI.OnGUI()
        {
            GUILayout.Label("ÆÄÀÏ", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (Style.Button(nameof(ReadFile)))
            {
                ReadFile();
            }
            if (Style.Button(nameof(SaveFile)))
            {
                SaveFile();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}