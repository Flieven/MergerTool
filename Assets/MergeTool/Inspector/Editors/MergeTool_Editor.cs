using UnityEngine;
using UnityEditor;
using MergeTool;

[CustomEditor(typeof(MergerTool)), CanEditMultipleObjects]
public class MergeTool_Editor : Editor
{
    MergerTool tool;
    GameObject objRef;

    SerializedObject serializedTarget;

    private void OnEnable()
    {
        tool = (MergerTool)target;

        objRef = ((MergerTool)target).gameObject;

        serializedTarget = new SerializedObject(tool);

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        serializedTarget.ApplyModifiedProperties();
    }

    //private void OnDestroy()
    //{
    //    if(null == target && null != objRef)
    //    {
    //        DestroyImmediate(objRef.GetComponent<MeshRegistry>());
    //        DestroyImmediate(objRef.GetComponent<MaterialMaker>());
    //        DestroyImmediate(objRef.GetComponent<Texture2DRegistry>());
    //    }
    //    else if (null == target && null == objRef)
    //    { }
    //}
}
