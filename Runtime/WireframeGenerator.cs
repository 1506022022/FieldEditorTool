using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace FieldEditorTool
{
    public class WireframeGenerator : MonoBehaviour
    {
        static readonly string CELL_PATH = "Packages/com.1506022022.field_editor_tool/Resources/CellData_NavMesh.prefab";
        public Vector3Int bounds;
        public float cellSize = 1.0f;
        internal LayerMask raycastMask = ~0;

        ObjectPool<GameObject> cellPool;
        ObjectPool<GameObject> CellPool
        {
            get
            {
                if (cellPool == null)
                {
                    cellPool = new(OnCreateCell, OnGetCell, OnReleaseCell, OnDestroyCell);
                }
                return cellPool;
            }
        }

        Bounds Bounds => new(transform.position + (Vector3)bounds / 2f, bounds);
        GameObject OnCreateCell()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CELL_PATH);
            var instance = Instantiate(prefab, transform);
            instance.transform.localEulerAngles = Vector3.right * 90f;
            instance.transform.localScale = Vector3.one * cellSize;

            var modifier = instance.GetComponent<NavMeshModifierVolume>();
            modifier.size = Vector3.right + Vector3.up + Vector3.forward * bounds.y;
            modifier.center = Vector3.back * (instance.transform.localPosition.y + bounds.y / 2f);

            if (DefaultMaterial == null)
            {
                var material = GameObject.CreatePrimitive(PrimitiveType.Quad).GetComponent<Renderer>();
                DefaultMaterial = material.sharedMaterial;
                GameObject.DestroyImmediate(material);
            }
            instance.GetComponent<Renderer>().material = DefaultMaterial;

            return instance;
        }

        void OnGetCell(GameObject cell)
        {
            cell.SetActive(true);
            cell.GetComponent<NavMeshModifierVolume>().area = 0;
        }
        void OnReleaseCell(GameObject cell)
        {
            cell.SetActive(false);
        }
        void OnDestroyCell(GameObject cell)
        {
            DestroyImmediate(cell);
        }
        public void GenerateVoxelWireframe()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                CellPool.Release(transform.GetChild(i).gameObject);
            }

            var originX = Bounds.min.x;
            var originZ = Bounds.min.z;
            var topY = Bounds.max.y;
            var height = Bounds.size.y;
            int sizeX = Mathf.CeilToInt(Bounds.size.x / cellSize);
            int sizeZ = Mathf.CeilToInt(Bounds.size.z / cellSize);

            var centers = new Vector3[sizeX, sizeZ];

            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    float worldX = originX + (x + 0.5f) * cellSize;
                    float worldZ = originZ + (z + 0.5f) * cellSize;
                    Vector3 origin = new Vector3(worldX, topY, worldZ);
                    Vector3 hitPoint = origin + Vector3.down * height;

                    if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, height, raycastMask))
                    {
                        hitPoint.y = hit.point.y;
                    }

                    centers[x, z] = hitPoint;
                }
            }

            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    var cell = CellPool.Get();
                    cell.transform.position = centers[x, z];

                    var data = cell.GetComponent<DataComponent>();
                    var modifier = cell.GetComponent<NavMeshModifierVolume>();

                    modifier.center = Vector3.forward * (0.1f + cell.transform.localPosition.y - bounds.y / 2f);
                    modifier.size = Vector3.right + Vector3.up + Vector3.forward * cellSize * bounds.y;
                    data.Index = new Vector2Int(x, z);
                    Type type = Types.GetDerivedTypes<AreaData>()[0];
                    data.HeaderType = type?.Name ?? "Unknown";
                    data.Area = (AreaData)Activator.CreateInstance(type);
                }
            }
            OrderChilds();
        }

        public DataComponent FindCellByIndex(int x, int z)
        {
            int index = z * bounds.x + x;
            if (transform.childCount <= index) return null;
            return transform.GetChild(index).GetComponent<DataComponent>();
        }
        void OrderChilds()
        {
            List<Transform> list = new List<Transform>();

            for (int i = 0; i < transform.childCount; i++)
            {
                list.Add(transform.GetChild(i));
            }

            list.Sort((a, b) =>
            {
                var compA = a.GetComponent<DataComponent>();
                var compB = b.GetComponent<DataComponent>();

                var indexA = compA.Index;
                var indexB = compB.Index;

                int priorityA = !a.gameObject.activeSelf ? int.MaxValue : indexA.y * bounds.x + indexA.x;
                int priorityB = !b.gameObject.activeSelf ? int.MaxValue : indexB.y * bounds.x + indexB.x;

                return priorityA.CompareTo(priorityB);
            });

            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetSiblingIndex(i);
            }
        }
        public void ClearPreview()
        {
            CellPool.Clear();
        }

        public void SetShow(bool enabled)
        {
            var material = enabled ? DefaultMaterial : null;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Renderer>().material = material;
            }
        }

        static Material DefaultMaterial;
    }
}