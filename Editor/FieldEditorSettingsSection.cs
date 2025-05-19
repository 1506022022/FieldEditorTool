using UnityEditor;
using UnityEngine;
namespace FieldEditorTool
{
    internal class FieldEditorSettingsSection : IFieldEditorUI
    {
        void IFieldEditorUI.OnGUI()
        {
            GUILayout.Label("�ɼ�", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Save Path : {FieldEditorSettings.Instance.SavePath ?? ""}");
        }
    }
}