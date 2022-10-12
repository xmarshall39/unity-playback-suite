using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//Good reference: https://docs.unity3d.com/ScriptReference/Editor.html
[CustomEditor(typeof(GeneratePlaybackScene))]
public class GeneratePlaybackSceneEditor : Editor
{
    SerializedProperty cosmeticTypesProp;

    private void OnEnable()
    {
        cosmeticTypesProp = serializedObject.FindProperty("cosmeticTypes");
        //cosmeticTypesProp.InsertArrayElementAtIndex
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ShowList(cosmeticTypesProp);
        
        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();
    }

    public static void ShowList(SerializedProperty list)
    {
        if (!list.isArray)
        {
            EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
            return;
        }

        EditorGUILayout.PropertyField(list);
        EditorGUI.indentLevel += 1;


        for (int i = 0; i < list.arraySize; ++i)
        {
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
        }
        
        
        EditorGUI.indentLevel -= 1;
    }
}
