using UnityEditor;
using UnityEngine;
namespace FieldEditorTool
{
    public static class FieldEditorUtility
    {
        public static NavigationAreasCustomData GetCustomNavigationAreas()
        {
            var path = $"Packages/com.1506022022.field_editor_tool/Resources/{nameof(NavigationAreasCustomData)}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<NavigationAreasCustomData>(path);

            for (int i = 0; i < 32; i++)
            {
                if (string.IsNullOrEmpty(asset.AreaTypes[i]))
                {
                    asset.AreaTypes[i] = Types.GetDerivedTypeNames<AreaData>()[0];
                    EditorUtility.SetDirty(asset);
                }
            }

            return asset;
        }
        public static object FromJson(string json)
        {
            var type = JsonUtility.FromJson<AreaData>(json).HeaderType;
            var instance = JsonUtility.FromJson(json, Types.FindTypeByName<AreaData>(type));
            return instance;
        }
    }
}
