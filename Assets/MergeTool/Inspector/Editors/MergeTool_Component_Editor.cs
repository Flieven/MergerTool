using UnityEngine;
using UnityEditor;
using MergeTool;

[CustomEditor(typeof(MergerTool_Component)), CanEditMultipleObjects]
public class MergeTool_Component_Editor : Editor
{
    MergerTool_Component tool;

    protected static bool showData = false;
    protected static bool showList = false;

    SerializedObject serializedTarget;

    #region SerializedPRoperties
    SerializedProperty ID;
    SerializedProperty distanceToRoot;
    SerializedProperty material;
    SerializedProperty prefabIndex;
    SerializedProperty meshRegistry;
    SerializedProperty uvList;
    #endregion

    private void OnEnable()
    {
        tool = (MergerTool_Component)target;

        serializedTarget = new SerializedObject(tool);

        PopulateProperties();
    }

    private void PopulateProperties()
    {
        ID = serializedTarget.FindProperty("ID");
        distanceToRoot = serializedTarget.FindProperty("maximumDistanceToRoot");
        material = serializedTarget.FindProperty("customMaterial");
        prefabIndex = serializedTarget.FindProperty("prefabIndex");
        meshRegistry = serializedTarget.FindProperty("meshRegistry");
        uvList = serializedTarget.FindProperty("uvList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //base.OnInspectorGUI();

        EditorGUILayout.PropertyField(ID);

        showData = EditorGUILayout.Foldout(showData, new GUIContent("Data", "All data currently used by component.\nNOTE: Read Only"));
        if(showData)
        {
            EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefab Index");
            EditorGUILayout.PropertyField(prefabIndex, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Distance To Root Object");
            EditorGUILayout.PropertyField(distanceToRoot, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Custom Material");
            EditorGUILayout.PropertyField(material, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mesh Registry");
            EditorGUILayout.PropertyField(meshRegistry, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUI.indentLevel++;
            DrawListWithoutSize(uvList, uvList.arraySize);
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }
        serializedTarget.ApplyModifiedProperties();
    }

    public void DrawListWithoutSize(SerializedProperty property, int size)
    {
        if(showList = EditorGUILayout.Foldout(showList, new GUIContent("UV List")))
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < size; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element);
            }

            EditorGUI.indentLevel++;
        }
    }

}
