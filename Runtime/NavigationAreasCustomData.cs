using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    [CreateAssetMenu(menuName = "ScriptableObject/NavigationAreasCustomData")]
    public class NavigationAreasCustomData : ScriptableObject
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
