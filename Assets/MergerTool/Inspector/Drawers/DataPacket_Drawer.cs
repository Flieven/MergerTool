using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DataPacket))]
public class DataPacket_Drawer : PropertyDrawer
{
    #region SerializedProperties

    SerializedProperty ID;
    SerializedProperty material;
    SerializedProperty prefabArray;
    SerializedProperty textureStruct;

    #endregion

    float staticPropertyHeight = 18.0f;
    int arraySizeModifier = 0;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(property.isExpanded)
        {
            arraySizeModifier = property.FindPropertyRelative("prefabs").isExpanded ?
                                property.FindPropertyRelative("prefabs").FindPropertyRelative("Array.size").intValue * 2 : 0;
        }
        else { arraySizeModifier = 0; }
        arraySizeModifier += property.isExpanded ? 4 : 1;

        return ( staticPropertyHeight * arraySizeModifier) + ((int)EditorGUIUtility.standardVerticalSpacing * arraySizeModifier) + 10;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        PopulateSerializedProperties(property);

        EditorGUI.BeginProperty(position, label, property);

        Rect boxRect = new Rect (position.x, 
            position.y, 
            position.width,
            (staticPropertyHeight * arraySizeModifier) + ((int)EditorGUIUtility.standardVerticalSpacing * arraySizeModifier) + EditorGUIUtility.standardVerticalSpacing * 3);
        EditorGUI.HelpBox(boxRect, "", MessageType.None);

        position.y += (EditorGUIUtility.standardVerticalSpacing * 2);
        position.height = EditorGUIUtility.singleLineHeight;

        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        DataSetArrayGUI(position, property, label);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    private void DataSetArrayGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect IDRect = new Rect(position.x, position.y, position.width, position.height);
        EditorGUI.PropertyField(IDRect, property.FindPropertyRelative("ID"), new GUIContent("ID"));

        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
        if (property.isExpanded)
        {
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            // READ ONLY MATERIAL SHOWCASE FOR DEBUG
            Rect textureSizeRect = new Rect(position.x + position.width * 0.48f, position.y, position.width * 0.5f, position.height);
            Rect textureSizeRectLabel = new Rect(position.x, position.y, position.width * 0.98f, position.height);
            EditorGUI.LabelField(textureSizeRectLabel, new GUIContent("[READ ONLY] Texture Size", "Size of textures within this data packet.\nValue provided by albedo texture of prefab 0.\nNOTE: Read Only."));
            EditorGUI.PropertyField(textureSizeRect, property.FindPropertyRelative("textureSize"), GUIContent.none);
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            // READ ONLY MATERIAL SHOWCASE FOR DEBUG
            Rect MaterialRect = new Rect(position.x + position.width * 0.48f, position.y, position.width * 0.5f, position.height);
            Rect MaterialRectLabel = new Rect(position.x, position.y, position.width * 0.98f, position.height);
            EditorGUI.LabelField(MaterialRectLabel, new GUIContent("[READ ONLY] Merged Material", "Resulting optimized material used by prefabs within the data packet.\nNOTE: Read Only."));
            EditorGUI.PropertyField(MaterialRect, property.FindPropertyRelative("mergedMaterial"), GUIContent.none);
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            PrefabArrayGUI(position, prefabArray, label);
        }
    }

    private void PrefabArrayGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");
        Rect prefabRect = new Rect(position.x, position.y, position.width * 0.98f, position.height);

        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(prefabRect, arraySizeProp, new GUIContent("Prefabs"));

        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        prefabArray.isExpanded = EditorGUI.Foldout(prefabRect, prefabArray.isExpanded, GUIContent.none);
        
        if(prefabArray.isExpanded)
        {
            for (int i = 0; i < arraySizeProp.intValue; i++)
            {
                Rect DistanceRect = new Rect(position.x + position.width * 0.5f, position.y + position.height + (i * (position.height)), position.width - position.width * 0.52f, position.height);
                Rect StaticRect = new Rect(position.x + position.width * 0.5f, position.y + (i * (position.height)), position.width - position.width * 0.52f, position.height);
                Rect ObjectRect = new Rect(position.x, position.y + ( i * (position.height)), position.width * 0.5f, position.height * 2.0f);

                EditorGUI.PropertyField(ObjectRect, property.GetArrayElementAtIndex(i).FindPropertyRelative("prefab"), GUIContent.none);
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(StaticRect, property.GetArrayElementAtIndex(i).FindPropertyRelative("isStatic"), new GUIContent("Is Static", "Is the object static in the world?\nIf false objects can be interacted with at individual level.\nNOTE: Do not call 'ReleaseFromRoot' on static objects!."));
                EditorGUI.PropertyField(DistanceRect, property.GetArrayElementAtIndex(i).FindPropertyRelative("maximumDistanceToRoot"), new GUIContent("Max Distance To Root", "Maximum distance object can have from a mergeable Root object"));
            }
        }
    }

    private void PopulateSerializedProperties(SerializedProperty prop)
    {
        ID = prop.FindPropertyRelative("ID");
        prefabArray = prop.FindPropertyRelative("prefabs");
        textureStruct = prop.FindPropertyRelative("textureRegistry");
    }

}
