using System;
using System.Linq;
using UnityEngine;

namespace FieldEditorTool
{
    [SerializeField]
    public class DataComponent : MonoBehaviour, IFieldEditorElement, ISerializationCallbackReceiver
    {
        [HideInInspector, SerializeField] EntityData data = new();
        [HideInInspector, SerializeField] string serializeJson;

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
#if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
            serializeJson = JsonUtility.ToJson(Data);
        }

        public void OnAfterDeserialize()
        {
            var type = JsonUtility.FromJson<EntityData>(serializeJson).HeaderType;
            Data = (EntityData)JsonUtility.FromJson(serializeJson, Types.FindTypeByName<EntityData>(type));
        }
#endif
    }
}
