using System;
using UnityEngine;
namespace FieldEditorTool
{
    [Serializable]
    public partial class ActorData : EntityData
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;
    }

    [Serializable]
    public partial class FieldData : ActorData
    {
        public Vector3Int Size;
    }
}