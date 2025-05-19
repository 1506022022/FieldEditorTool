using UnityEditor;
using UnityEngine;
namespace FieldEditorTool
{
    public static class FieldEditorUtility
    {
        public static CustomNavigationAreas GetCustomNavigationAreas()
        {
            var path = $"Packages/com.1506022022.field_editor_tool/Resources/{nameof(CustomNavigationAreas)}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<CustomNavigationAreas>(path);

            for(int i= 0; i<32;i++)
            {
                if (string.IsNullOrEmpty(asset.AreaTypes[i]))
                {
                    asset.AreaTypes[i] = AreaType.GetDerivedTypeNames()[0];
                    EditorUtility.SetDirty(asset);
                }
            }

            return asset;
        }
        public static object FromJson(string json)
        {
            var type = JsonUtility.FromJson<AreaType>(json).HeaderType;
            var instance = JsonUtility.FromJson(json, AreaType.FindTypeByName(type));
            return instance;
        }
    }
}
