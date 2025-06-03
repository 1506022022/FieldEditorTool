using System;
using UnityEngine;
namespace FieldEditorTool
{
    [Serializable]
    public class ActorData : EntityData
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;
    }

    [Serializable]
    public class FieldData : ActorData
    {
        public Vector3Int Size;
    }
}