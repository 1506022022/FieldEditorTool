using UnityEditor;
using UnityEngine;
namespace FieldEditorTool
{
    internal class FieldEditorSettingsSection : IFieldEditorUI
    {
        void IFieldEditorUI.OnGUI()
        {
            GUILayout.Label("¿É¼Ç", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Save Path : {FieldEditorSettings.Instance.SavePath ?? ""}");
        }
    }
}