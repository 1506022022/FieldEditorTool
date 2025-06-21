using UnityEngine;
namespace FieldEditorTool
{
    public partial class CellData : EntityData
    {
        [HideInInspector] public Vector2Int Index;
    }

    public partial class Walkable : CellData
    {
        public GameObject Who;
        public Vector3 Where;
        public float When;
    }

    public partial class NotWalkable : CellData
    {
        public string Why;

    }
}