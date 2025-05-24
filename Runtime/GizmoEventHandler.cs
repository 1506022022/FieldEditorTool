using System;
using UnityEngine;

namespace FieldEditorTool
{
    public class GizmoEventHandler : MonoBehaviour
    {
        public event Action DrawGizmosEvent;
        public event Action DrawGizmosSelectedEvent;

        void OnDrawGizmos()
        {
            DrawGizmosEvent?.Invoke();
        }
        void OnDrawGizmosSelected()
        {
            DrawGizmosSelectedEvent?.Invoke();
        }
    }

}
