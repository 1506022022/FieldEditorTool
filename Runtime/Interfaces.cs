using System.Collections.Generic;
using UnityEngine;
namespace FieldEditorTool
{
    public interface IFieldEditorDispose
    {
        public void Dispose();
    }
    public interface IFieldEditorElement
    {
        public string GetJson();
    }
    public interface IFieldEditorInitialize
    {
        public void Initialize();
    }
    public interface IFieldEditorUI
    {
        public void OnGUI();
    }
    public interface IFieldEditorWindow
    {
        public void ShowWindow();
    }
    public interface IFieldEditorFile
    {
        public void OnReadFile(List<EntityData> areaData);
    }
}