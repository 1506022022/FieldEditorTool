using UnityEngine;
namespace FieldEditorTool
{
    public class ActorData : EntityData
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;
    }
    public class FieldData : ActorData
    {
        public Vector3Int Size;
    }
}