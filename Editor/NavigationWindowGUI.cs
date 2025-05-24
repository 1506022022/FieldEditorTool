using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal class NavigationWindowGUI : BaseWindowGUI
    {
        EditorWindow navigation;
        EditorWindow subWindow;
        WireframeGenerator wireframeGenerator;
        NavMeshSurface navMeshSurface;
        bool cellGizmo = true;

        protected override void OnGUI_Child()
        {
            DrawBoundEditor();
            GUILayout.Space(10);
            DrawDebugToggles();
            GUILayout.Space(10);
            DrawActionButtons();
            GUILayout.Space(10);
            DrawSideWindowButton();
        }

        public void DrawSideWindowButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (Style.Button(nameof(OpenSide)))
            {
                OpenSide();
            }
            if (Style.Button(nameof(CloseSide)))
            {
                CloseSide();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void CloseSide()
        {
            navigation?.Close();
            subWindow?.Close();
            navigation = null;
            subWindow = null;
        }
        void OpenSide()
        {
            CloseSide();
            EditorApplication.ExecuteMenuItem("Window/AI/Navigation");
            Rect nextPosition = new Rect();
            if (EditorWindow.focusedWindow.titleContent.text == "Navigation")
            {
                navigation = EditorWindow.focusedWindow;
                var navigationRect = navigation.position;
                navigationRect.position = Vector3.zero;
                navigation.position = navigationRect;

                nextPosition = new Rect(
                    navigationRect.xMax + 10,
                    navigationRect.y,
                    navigationRect.width,
                    navigationRect.height
                );
            }

            subWindow = ScriptableObjectEditorWindow.ShowWindow(FieldEditorUtility.GetCustomNavigationAreas());
            var expansionPosition = subWindow.position;
            nextPosition.width = expansionPosition.width;
            subWindow.position = nextPosition;
            subWindow.name = nameof(NavigationAreasCustomData);
        }

        Rect GetSidePosition()
        {
            Rect sideRect = new Rect();
            var navigationRect = navigation.position;
            var subWindowRect = subWindow.position;

            sideRect = new Rect(
                navigationRect.xMax + 10,
                navigationRect.y,
                navigationRect.width,
                navigationRect.height
            );

            sideRect = new Rect(

                subWindowRect.xMin - 10,
                navigationRect.y,
                navigationRect.width,
                navigationRect.height
            );
            return sideRect;
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

        protected override void Initialize_Child()
        {
            var prefabPath = "Packages/com.1506022022.field_editor_tool/Resources/WireframeGenerator.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var instance = GameObject.Instantiate(prefab, root);

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

        protected override void Dispose_Child()
        {
            CloseSide();
        }

        protected override void OnReadFile_Child(List<EntityData> areaData)
        {
            if (wireframeGenerator == null) return;

            List<CellData> list = areaData.Where(x => x is CellData).Select(x => x as CellData).ToList();

            wireframeGenerator.ClearPreview();

            int maxX = areaData.Count == 0 ? 1 : list.Max(a => a.Index.x) + 1;
            int maxZ = areaData.Count == 0 ? 1 : list.Max(a => a.Index.y) + 1;
            wireframeGenerator.bounds = new Vector3Int(maxX, 1, maxZ);

            GenerateField();

            foreach (var area in list)
            {
                var cell = wireframeGenerator.FindCellByIndex(area.Index.x, area.Index.y);
                cell.Data = area;
            }
        }
    }
}