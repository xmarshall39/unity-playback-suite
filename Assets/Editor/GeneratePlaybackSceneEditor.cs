using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace UPBS
{
    //Good reference: https://docs.unity3d.com/ScriptReference/Editor.html
    [CustomEditor(typeof(GeneratePlaybackScene))]
    public class GeneratePlaybackSceneEditor : Editor
    {
        private struct ComponentData
        {
            public System.Type type;
            public System.Reflection.Assembly assembly;

            public override string ToString()
            {
                return $"Type: {type.Name} | Assembly: {assembly.GetName().Name}";
            }
        }
        private List<ComponentData> components = new List<ComponentData>();

        GeneratePlaybackScene sceneGenerator;
        SerializedProperty cosmeticTypesProp, useDerivedClassesProp;
        static GUIContent deleteButtonContent;
        int listLength;
        private void OnEnable()
        {

            if (sceneGenerator = serializedObject.targetObject as GeneratePlaybackScene)
            {
                Debug.Log("Clear");
            }
            cosmeticTypesProp = serializedObject.FindProperty("cosmeticTypes");
            useDerivedClassesProp = serializedObject.FindProperty("useDerivedClasses");
            listLength = cosmeticTypesProp.arraySize;
            deleteButtonContent = new GUIContent(EditorGUIUtility.IconContent("P4_DeletedLocal").image); //@2x
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ShowList(cosmeticTypesProp, useDerivedClassesProp);
            if (GUILayout.Button(new GUIContent("Add Component")))
            {
                var searchProvider = CreateInstance<ComponentSearchProvider>();
                var searchWindowContext = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
                SearchWindow.Open(searchWindowContext, searchProvider);
                Debug.Log("Here");
            }

            base.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }

        public static void ShowList(SerializedProperty cosmeticList, SerializedProperty boolList)
        {
            if (!cosmeticList.isArray)
            {
                EditorGUILayout.HelpBox(cosmeticList.name + " is neither an array nor a list!", MessageType.Error);
                return;
            }

            EditorGUILayout.PropertyField(cosmeticList);
            EditorGUI.indentLevel += 1;

            if(cosmeticList.arraySize == boolList.arraySize)
            {
                for (int i = 0; i < cosmeticList.arraySize; ++i)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(cosmeticList.GetArrayElementAtIndex(i));
                        EditorGUILayout.Toggle(false);
                        if(GUILayout.Button(deleteButtonContent, EditorStyles.miniButton))
                        {
                            Debug.Log("Delete Button Pushed");
                        }
                        //EditorGUILayout.PropertyField(boolList.GetArrayElementAtIndex(i));
                    }

                }
            }
            


            EditorGUI.indentLevel -= 1;
        }
    }
}