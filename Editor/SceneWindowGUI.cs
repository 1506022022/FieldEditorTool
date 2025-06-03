using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    public class SceneWindowGUI : IFieldEditorUI, IFieldEditorElement
    {
        Transform root;
        readonly string AddButtonToolTip = "bake target의 DataComponent가 filedData로 초기화됩니다.";
        public string GetJson()
        {
            if (!DoesRootHaveFieldData()) return "";
            return FieldEditorElementUtility.GetJsonFromChilds(root);
        }
        public void OnGUI()
        {
            root = (Transform)EditorGUILayout.ObjectField("bake target", root, typeof(Transform), true);
            if (root != null && !DoesRootHaveFieldData()) AddFieldData();
        }
        void OnInitializeRoot()
        {
            if (!root.gameObject.TryGetComponent<DataComponent>(out var container))
                container = root.gameObject.AddComponent<DataComponent>();

            container.Data = new FieldData() { Name = "Unknown", Size = Vector3Int.one };
        }
        void AddFieldData()
        {
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (Style.Button(nameof(AddFieldData), AddButtonToolTip)) OnInitializeRoot();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        bool DoesRootHaveFieldData()
        {
            if (root == null) return false;
            if (!root.TryGetComponent<DataComponent>(out var data)) return false;
            return data.HeaderType == nameof(FieldData);
        }
    }
}
