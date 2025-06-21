using System;
using UnityEngine;
namespace FieldEditorTool
{
    [Serializable]
    public partial class FieldData : EntityData
    {
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3Int Size;
    }
}