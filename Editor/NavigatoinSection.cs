using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal class NavigationSection : IFieldEditorUI, IFieldEditorInitialize, IFieldEditorDispose, IFieldEditorElement, IFieldEditorFile
    {
        private WireframeGenerator wireframeGenerator;
        private NavMeshSurface navMeshSurface;
        private bool cellGizmo = true;

        void IFieldEditorUI.OnGUI()
        {
            DrawBoundEditor();
            GUILayout.Space(10);
            DrawDebugToggles();
            GUILayout.Space(10);
            DrawActionButtons();
        }

        void DrawBoundEditor()
        {
            wireframeGenerator.bounds = EditorGUILayout.Vector3IntField("Bound", wireframeGenerator.bounds);
            var clamp = wireframeGenerator.bounds;
            clamp.x = Mathf.Max(clamp.x, 1);
            clamp.y = Mathf.Max(clamp.y, 1);
            clamp.z = Mathf.Max(clamp.z, 1);
            wireframeGenerator.bounds = clamp;
            SyncNavMeshBoundsWithGenerator();
        }

        void DrawDebugToggles()
        {
            GizmoUtility.TryGetGizmoInfo(typeof(NavMeshModifierVolume), out var volumeGizmoInfo);
            volumeGizmoInfo.gizmoEnabled = GUILayout.Toggle(volumeGizmoInfo.gizmoEnabled, "Volume Gizmo");
            GizmoUtility.SetGizmoEnabled(typeof(NavMeshModifierVolume), volumeGizmoInfo.gizmoEnabled, true);

            var previousCellGizmo = cellGizmo;
            cellGizmo = GUILayout.Toggle(cellGizmo, "Cell Gizmo");
            if (previousCellGizmo != cellGizmo)
            {
                wireframeGenerator.SetShow(cellGizmo);
            }
        }

        void DrawActionButtons()
        {
            if (wireframeGenerator == null) return;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DrawButtonWithIcon("NavMeshModifierVolume Icon.png", "Generate Field", GenerateField);
            DrawButtonWithIcon("NavMeshSurface Icon.png", "Bake Navigation", BakeNavigation);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void DrawButtonWithIcon(string iconFileName, string tooltip, System.Action action)
        {
            var iconPath = $"Packages/com.1506022022.field_editor_tool/Gizmos/{iconFileName}";
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (Style.Button(icon, tooltip))
            {
                action?.Invoke();
            }
        }

        internal void GenerateField()
        {
            wireframeGenerator.GenerateVoxelWireframe();
        }

        void BakeNavigation()
        {
            navMeshSurface.BuildNavMesh();
        }

        void IFieldEditorInitialize.Initialize()
        {
            var prefabPath = "Packages/com.1506022022.field_editor_tool/Resources/WireframeGenerator.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var instance = GameObject.Instantiate(prefab);

            wireframeGenerator = instance.GetComponent<WireframeGenerator>();
            navMeshSurface = instance.GetComponent<NavMeshSurface>();
            navMeshSurface.collectObjects = CollectObjects.Volume;

            SyncNavMeshBoundsWithGenerator();
        }

        void SyncNavMeshBoundsWithGenerator()
        {
            navMeshSurface.size = wireframeGenerator.bounds;
            navMeshSurface.center = navMeshSurface.size / 2f + Vector3.down * 0.1f;
        }

        void IFieldEditorDispose.Dispose()
        {
            if (wireframeGenerator?.gameObject != null)
            {
                GameObject.DestroyImmediate(wireframeGenerator.gameObject);
            }
        }

        string IFieldEditorElement.GetJson()
        {
            if (wireframeGenerator == null || wireframeGenerator.transform.childCount == 0)
            {
                return string.Empty;
            }

            var lines = new List<string>();
            for (int z = 0; z < wireframeGenerator.bounds.z; z++)
            {
                for (int x = 0; x < wireframeGenerator.bounds.x; x++)
                {
                    var elements = wireframeGenerator.FindCellByIndex(x, z).GetComponents<IFieldEditorElement>();
                    var json = string.Join('\n', elements.Where(x => !string.IsNullOrEmpty(x.GetJson())).Select(x => x.GetJson()).ToArray());
                    lines.Add(json);
                }
            }

            return string.Join("\n", lines);
        }

        void IFieldEditorFile.OnReadFile(List<AreaType> areaData)
        {
            if (wireframeGenerator == null) return;

            wireframeGenerator.ClearPreview();

            int maxX = areaData.Count == 0 ? 1 : areaData.Max(a => a.Index.x) + 1;
            int maxZ = areaData.Count == 0 ? 1 : areaData.Max(a => a.Index.y) + 1;
            wireframeGenerator.bounds = new Vector3Int(maxX, 1, maxZ);

            GenerateField();

            foreach (var area in areaData)
            {
                var cell = wireframeGenerator.FindCellByIndex(area.Index.x, area.Index.y);
                cell.Area = area;
            }
        }
    }
}