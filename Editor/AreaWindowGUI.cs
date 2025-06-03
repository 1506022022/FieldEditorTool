using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal static class FieldEditorElementUtility
    {
        internal static string GetJsonFromChilds(Transform root)
        {
            if (root == null) return "";
            var json = string.Join('\n', root.GetComponentsInChildren<IFieldEditorElement>().Select(x => x.GetJson()).Where(x => !string.IsNullOrEmpty(x)).ToArray());
            return json;
        }
    }

    internal abstract class BaseWindowGUI : IFieldEditorUI, IFieldEditorInitialize, IFieldEditorDispose, IFieldEditorElement, IFieldEditorFile
    {
        protected FieldData fieldData { get; private set; }
        protected Transform root { get; private set; }

        public void Dispose()
        {
            if (root != null)
            {
                GameObject.DestroyImmediate(root.gameObject);
            }
            Dispose_Child();
        }
        protected abstract void Dispose_Child();
        public string GetJson()
        {
            return FieldEditorElementUtility.GetJsonFromChilds(root);
        }

        public void Initialize()
        {
            if (root != null) return;
            var field = new GameObject("Field").AddComponent<DataComponent>();
            fieldData = new FieldData();
            fieldData.Size = Vector3Int.one * 10;
            field.Data = fieldData;
            root = field.transform;
            Initialize_Child();
        }
        protected abstract void Initialize_Child();
        public void OnGUI()
        {
            GUILayout.Label("필드", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"{nameof(FieldEditorSettings.Instance.SavePath)}: {FieldEditorSettings.Instance.SavePath}", Style.WrapLabel);
            if (fieldData == null) return;
            fieldData.Name = EditorGUILayout.TextField(nameof(fieldData.Name), string.IsNullOrEmpty(fieldData.Name) ? "Unknown" : fieldData.Name);
            FieldEditorSettings.Instance.FileName = fieldData.Name;
            fieldData.Size = EditorGUILayout.Vector3IntField(nameof(fieldData.Size), fieldData.Size);
            fieldData.Size = Vector3Int.Max(fieldData.Size, Vector3Int.one);
            GUILayout.Space(50);
            OnGUI_Child();
        }
        protected abstract void OnGUI_Child();
        public void OnReadFile(List<EntityData> areaData)
        {
            if (areaData.Count == 0 || areaData.First().HeaderType != nameof(FieldData))
                return;

            if (root == null) Initialize();
            var field = areaData.First();
            root.GetComponent<DataComponent>().Data = field;

            for (int i = 0; i < root.childCount; i++)
            {
                GameObject.DestroyImmediate(root.GetChild(i).gameObject);
            }

            for (int i = 1; i < areaData.Count; i++)
            {
                var obj = CreateElement();
                obj.Data = areaData[i];
            }
        }
        protected abstract void OnReadFile_Child(List<EntityData> areaData);
        protected DataComponent CreateElement()
        {
            var obj = new GameObject("element").AddComponent<DataComponent>();
            obj.transform.SetParent(root);
            Selection.activeGameObject = obj.gameObject;
            return obj;
        }
    }

    internal class AreaWindowGUI : BaseWindowGUI
    {
        Color gizmoColor = new(0.5f, 0.66f, 0.69f, 0.15f);

        protected override void Dispose_Child() { }

        protected override void Initialize_Child()
        {
            if (root != null && root.gameObject.GetComponent<GizmoEventHandler>() == null)
                root.gameObject.AddComponent<GizmoEventHandler>().DrawGizmosEvent += DrawGizmo;
        }

        protected override void OnGUI_Child()
        {
            GUILayout.Label("요소", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (Style.Button("생성")) CreateElement();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        protected override void OnReadFile_Child(List<EntityData> areaData) { }

        private void DrawGizmo()
        {
            if (root == null) return;
            Gizmos.color = gizmoColor;
            Vector3 center = root.position + (Vector3)fieldData.Size / 2f;
            Vector3 size = fieldData.Size;

            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, size);
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
            Gizmos.DrawWireCube(Vector3.zero, size);
        }
    }
}