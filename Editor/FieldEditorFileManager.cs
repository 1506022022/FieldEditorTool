using System;
using System.IO;
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
        void ReadFile() => OnClickReadButton?.Invoke(FieldEditorSettings.Instance.SavePath);
        void SaveFile() => OnClickSaveButton?.Invoke();
        void NewFile() { }
        void IFieldEditorUI.OnGUI()
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (Style.Button(nameof(ReadFile)))
            {
                ReadFile();
            }
            if (Style.Button(nameof(SaveFile)))
            {
                SaveFile();
            }
            if (Style.Button(nameof(NewFile)))
            {
                NewFile();
            }
            GUILayout.EndHorizontal();
        }
    }
}