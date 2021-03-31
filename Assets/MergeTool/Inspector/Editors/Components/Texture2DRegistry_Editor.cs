using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Texture2DRegistry)), CanEditMultipleObjects]
public class Texture2DRegistry_Editor : Editor
{
    private void OnEnable()
    {
        Texture2DRegistry tool = (Texture2DRegistry)target;
        tool.hideFlags = HideFlags.NotEditable;
    }
}
