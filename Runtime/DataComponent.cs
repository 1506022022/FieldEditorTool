using System;
using System.Linq;
using UnityEngine;

namespace FieldEditorTool
{
    public class DataComponent : MonoBehaviour, IFieldEditorElement
    {
        EntityData data = new();

        public string HeaderType
        {
            get => data.HeaderType;
            set => data.HeaderType = value;
        }

        public EntityData Data
        {
            get => data;
            set
            {
                data = value;
                data.HeaderType = data.GetType().Name;
            }
        }

        EntityData GetDefaultData()
        {
            return (EntityData)Activator.CreateInstance(Types.GetDerivedTypes<EntityData>().First());
        }

        string IFieldEditorElement.GetJson()
        {
            if (data.GetType() == typeof(EntityData)) Data = GetDefaultData();
            return JsonUtility.ToJson(data);
        }
    }
}
