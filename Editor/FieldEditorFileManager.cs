using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal class FieldEditorFileManager : IFieldEditorUI
    {
        internal event Action OnClickSaveButton;
        internal event Action<string> OnClickReadButton;
        internal event Action OnClickNewButton;
        internal void WriteJson(string json, string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            File.WriteAllText(path, json);
        }
        void ReadFile()
        {
            FieldEditorSettings.Instance.SavePath = EditorUtility.OpenFilePanel("Open Json", FieldEditorSettings.Instance.SavePath, "txt,json");
            if (!File.Exists(FieldEditorSettings.Instance.SavePath)) return;
            OnClickReadButton?.Invoke(FieldEditorSettings.Instance.SavePath);
        }
        void SaveFile()
        {
            FieldEditorSettings.Instance.SavePath = EditorUtility.OpenFilePanel("Save Json", FieldEditorSettings.Instance.SavePath, "txt,json");
            if (!File.Exists(FieldEditorSettings.Instance.SavePath)) return;
            OnClickSaveButton?.Invoke();
        }
        void IFieldEditorUI.OnGUI()
        {
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