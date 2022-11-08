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
            cosmeticTypesProp = serializedObject.FindProperty("replicationTypes");
            useDerivedClassesProp = serializedObject.FindProperty("useDerivedClasses");
            listLength = cosmeticTypesProp.arraySize;
            deleteButtonContent = new GUIContent(EditorGUIUtility.IconContent("P4_DeletedLocal").image); //@2x
        }

        private void AddComponent(System.Type type)
        {
            if (!sceneGenerator.replicationTypes.Contains(new SerializableSystemType(type)))
            {
                sceneGenerator.replicationTypes.Add(new SerializableSystemType(type));
                sceneGenerator.useDerivedClasses.Add(false);
            }

            else
            {
                Debug.LogWarning("Selected class already in the list");
            }
        }

        private void RemoveAt(int index)
        {
            Debug.Log(index);
            sceneGenerator.replicationTypes.RemoveAt(index);
            sceneGenerator.useDerivedClasses.RemoveAt(index);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ShowList(cosmeticTypesProp, useDerivedClassesProp);
            if (GUILayout.Button(new GUIContent("Add Component")))
            {
                var searchProvider = CreateInstance<ComponentSearchProvider>();
                searchProvider.AssignCallback(AddComponent);
                var searchWindowContext = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
                SearchWindow.Open(searchWindowContext, searchProvider);
                Debug.Log("Here");
            }

            base.OnInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }

        public void ShowList(SerializedProperty cosmeticList, SerializedProperty boolList)
        {
            if (!cosmeticList.isArray)
            {
                EditorGUILayout.HelpBox(cosmeticList.name + " is neither an array nor a list!", MessageType.Error);
                return;
            }

            Color originalContentColor = GUI.contentColor;
            using(new EditorGUI.IndentLevelScope(1))
            {
                using (var vertScope = new EditorGUILayout.VerticalScope(new GUIStyle()))
                {
                    
                    EditorGUI.DrawRect(vertScope.rect, UPBS.Constants.SECONDARY_COLOR);
                    using (var header = new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUI.DrawRect(header.rect, UPBS.Constants.BRAND_COLOR);

                        DrawVerticalLine(Color.gray, 1);
                        GUI.contentColor = Color.black;
                        GUILayout.Label("Replication Types", EditorStyles.boldLabel);
                        GUI.contentColor = originalContentColor;
                        DrawVerticalLine(Color.gray, 1);
                    }

                    System.Action RemovalAction = null;
                    if (cosmeticList.arraySize == boolList.arraySize)
                    {
                        DrawHorizontalLine(Color.gray, thickness: 1, padding: 0);
                        for (int i = 0; i < cosmeticList.arraySize; ++i)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                int x = i;
                                DrawVerticalLine(Color.gray, 1);

                                GUI.contentColor = Color.black;
                                EditorGUILayout.PropertyField(cosmeticList.GetArrayElementAtIndex(i), EditorStyles.miniBoldFont);
                                GUI.contentColor = originalContentColor;

                                DrawVerticalLine(Color.gray, 1);
                                sceneGenerator.useDerivedClasses[i] = EditorGUILayout.Toggle(boolList.GetArrayElementAtIndex(i).boolValue);

                                DrawVerticalLine(Color.gray, 1);
                                if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButton))
                                {
                                    RemovalAction = () => RemoveAt(x);
                                    Debug.Log("Delete Button Pushed");
                                }
                                DrawVerticalLine(Color.gray, 1);
                                //EditorGUILayout.PropertyField(boolList.GetArrayElementAtIndex(i));
                            }
                            DrawHorizontalLine(Color.gray, thickness: 1, padding: 0);
                        }
                    }

                    if (RemovalAction != null)
                    {
                        RemovalAction.Invoke();
                    }
                }
                DrawHorizontalLine(Color.gray, thickness: 1, padding: 0);

            }
            GUI.contentColor = originalContentColor;
        }

        public static void DrawHorizontalLine(Color color, int thickness = 2, int padding = 10, int xOffset = 0)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= xOffset;
            //r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawVerticalLine(Color color, int width = 1, int extraHeight = 4, int yOffset = -2, int xOffset = 0)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(width));
            r.height += extraHeight;
            r.y += yOffset;
            r.x += xOffset;
            EditorGUI.DrawRect(r, color);
        }
    }
}