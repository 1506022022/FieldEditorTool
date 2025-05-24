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

        public enum FileType { Json }
        [field: SerializeField] public string SaveFolder { get; set; }
        [SerializeField] private string fileName;
        public string FileName
        {
            get => string.IsNullOrEmpty(fileName) ? "Unknown" : fileName;
            set => fileName = value;
        }
        public string FileNameWithExtension => $"{FileName}.{Extension}";
        public string SavePath => Path.Combine(SaveFolder, FileNameWithExtension);
        [field: SerializeField] public bool UseNavigation { get; private set; }

        [field: SerializeField] public FileType Extension { get; private set; }

        void OnValidate()
        {
            ValidateSavePath();
            OnValidateEvent?.Invoke();
        }

        void ValidateSavePath()
        {
            if (string.IsNullOrEmpty(SaveFolder))
                return;

            var directoryPath = Path.GetDirectoryName(SaveFolder);
            var fileName = Path.GetFileNameWithoutExtension(SaveFolder);
            var extension = this.Extension.ToString();

            SaveFolder = Path.Combine(directoryPath, $"{fileName}.{extension}");
        }
    }
}
