using UnityEngine;
namespace FieldEditorTool
{
    public class CellData : EntityData
    {
        [HideInInspector] public Vector2Int Index;
    }

    public class Walkable : CellData
    {
        public GameObject Who;
        public Vector3 Where;
        public float When;
    }

    public class NotWalkable : CellData
    {
        public string Why;

    }
}