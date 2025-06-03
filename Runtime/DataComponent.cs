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
        Color gizmoColor = new(0.5f, 0.66f, 0.69f, 0.15f);
        void OnDrawGizmos()
        {
            if (Data is FieldData field)
            {
                Gizmos.color = gizmoColor;
                Vector3 center = field.Position + (Vector3)field.Size / 2f;
                Vector3 size = field.Size;

                Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one);
                Gizmos.DrawCube(Vector3.zero, size);
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
                Gizmos.DrawWireCube(Vector3.zero, size);
            }

            if (Data is ActorData actor)
            {
                actor.Position = transform.localPosition;
                actor.Rotation = transform.localEulerAngles;
                // actor.Name = gameObject.name;
            }
        }

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
