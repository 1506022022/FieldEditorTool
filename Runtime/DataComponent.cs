using UnityEngine;

namespace FieldEditorTool
{
    public class DataComponent : MonoBehaviour, IFieldEditorElement
    {
        AreaData area = new();

        public Vector2Int Index
        {
            get => area.Index;
            set => area.Index = value;
        }

        public string HeaderType
        {
            get => area.HeaderType;
            set => area.HeaderType = value;
        }

        public AreaData Area
        {
            get => area;
            set
            {
                var currentIndex = area.Index;
                area = value;
                area.Index = currentIndex;
                area.HeaderType = area.GetType().Name;
            }
        }

        string IFieldEditorElement.GetJson()
        {
            return JsonUtility.ToJson(area);
        }
    }
}
