using Unity.AI.Navigation;
using UnityEngine;

namespace FieldEditorTool
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CellDataComponent))]
    [RequireComponent(typeof(NavMeshModifierVolume))]
    internal class CellDataModifier : MonoBehaviour, IFieldEditorElement
    {
        NavMeshModifierVolume modifierVolume;
        CellDataComponent cellData;

        string IFieldEditorElement.GetJson()
        {
            return null;
        }

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

            cellData ??= GetComponent<CellDataComponent>();
            if (cellData == null) return;

            UpdateHeaderType();
        }
    }
}
