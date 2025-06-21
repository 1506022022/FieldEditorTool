using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FieldEditorTool
{
    internal static class FieldEditorElementUtility
    {
        internal static string GetJsonFromChilds(Transform root)
        {
            if (root == null) return "";
            var json = string.Join('\n', root.GetComponentsInChildren<IFieldEditorElement>().Select(x => x.GetJson()).Where(x => !string.IsNullOrEmpty(x)).ToArray());
            return json;
        }
    }

}