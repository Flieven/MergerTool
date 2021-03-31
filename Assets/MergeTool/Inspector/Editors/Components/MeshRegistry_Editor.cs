using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshRegistry)), CanEditMultipleObjects]
public class MeshRegistry_Editor : Editor
{
    private void OnEnable()
    {
        MeshRegistry tool = (MeshRegistry)target;
        tool.hideFlags = HideFlags.NotEditable;
    }
}
