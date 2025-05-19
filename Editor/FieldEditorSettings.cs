using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    [CreateAssetMenu(menuName = "ScriptableObject/FieldEditorSettings")]
    public class FieldEditorSettings : ScriptableObject
    {
        static readonly string assetPath = $"Packages/com.1506022022.field_editor_tool/Resources/{nameof(FieldEditorSettings)}.asset";
        static FieldEditorSettings instance;
        public static FieldEditorSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<FieldEditorSettings>(assetPath);
                }
                if (instance == null)
                {
                    instance = CreateInstance<FieldEditorSettings>();
                    AssetDatabase.CreateAsset(instance, assetPath);
                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = instance;
                    Debug.Log($"ScriptableObject created at: {assetPath}");
                }
                return instance;
            }
        }

        internal event Action OnValidateEvent;

        enum FileType { Json }
        [field: SerializeField] public string SavePath { get; set; }
        [field: SerializeField] public string FieldPresetPath { get; private set; }
        [field: SerializeField] public bool UseNavigation { get; private set; }

        [SerializeField] FileType fileType;

        void OnValidate()
        {
            ValidateSavePath();
            OnValidateEvent?.Invoke();
        }

        void ValidateSavePath()
        {
            if (string.IsNullOrEmpty(SavePath))
                return;

            var directoryPath = Path.GetDirectoryName(SavePath);
            var fileName = Path.GetFileNameWithoutExtension(SavePath);
            var extension = fileType.ToString();

            SavePath = Path.Combine(directoryPath, $"{fileName}.{extension}");
        }
    }
}
