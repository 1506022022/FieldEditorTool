using Unity.AI.Navigation;
using UnityEngine;

namespace FieldEditorTool
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DataComponent))]
    [RequireComponent(typeof(NavMeshModifierVolume))]
    internal class AreaDataModifierComponent : MonoBehaviour
    {
        NavMeshModifierVolume modifierVolume;
        DataComponent cellData;

        void UpdateHeaderType()
        {
            var customTypes = FieldEditorUtility.GetCustomNavigationAreas();
            if (customTypes.AreaTypes.Length <= modifierVolume.area) return;

            var expectedType = customTypes.AreaTypes[modifierVolume.area];
            if (expectedType == null) return;

            if (cellData.HeaderType != expectedType)
                cellData.HeaderType = expectedType;
        }

        void OnGUI()
        {
            modifierVolume ??= GetComponent<NavMeshModifierVolume>();
            if (modifierVolume == null) return;

            cellData ??= GetComponent<DataComponent>();
            if (cellData == null) return;

            UpdateHeaderType();
        }
    }
}
