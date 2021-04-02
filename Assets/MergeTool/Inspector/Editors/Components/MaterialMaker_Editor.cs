using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MaterialMaker)), CanEditMultipleObjects]
public class MaterialMaker_Editor : Editor
{
    private void OnEnable()
    {
        MaterialMaker tool = (MaterialMaker)target;
        //tool.hideFlags = HideFlags.NotEditable;
    }
}
