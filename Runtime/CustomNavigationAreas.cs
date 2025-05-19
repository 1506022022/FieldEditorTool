using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    [CreateAssetMenu(menuName = "ScriptableObject/CustomNavigationAreas")]
    public class CustomNavigationAreas : ScriptableObject
    {
        public string[] AreaTypes = new string[32];

        public SerializedObject CreateSerializedObject()
        {
            return new SerializedObject(this);
        }

        public SerializedProperty GetProperty()
        {
            return CreateSerializedObject().FindProperty(nameof(AreaTypes));
        }
    }
}
