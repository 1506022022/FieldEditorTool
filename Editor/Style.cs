using UnityEngine;

public static class Style
{
    public static bool Button(Texture2D icon, string tooltip)
    {
        var content = new GUIContent(icon) { tooltip = tooltip };
        return GUILayout.Button(content, GUI.skin.button,
            GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true),
            GUILayout.Width(50), GUILayout.MaxWidth(80),
            GUILayout.Height(50), GUILayout.MaxHeight(80));
    }

    public static bool Button(string text)
    {
        var content = new GUIContent(text);
        return GUILayout.Button(content, GUI.skin.button,
            GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true),
            GUILayout.Width(50), GUILayout.MaxWidth(80),
            GUILayout.Height(50), GUILayout.MaxHeight(80));
    }
}
