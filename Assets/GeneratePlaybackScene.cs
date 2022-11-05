﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UPBS
{
    [ExecuteAlways]
    public class GeneratePlaybackScene : MonoBehaviour
    {
        [HideInInspector]

        public List<SerializableSystemType> replicationTypes = new List<SerializableSystemType>
        {
        new SerializableSystemType(typeof(UPBS.Execution.PBTrackerID)),
        new SerializableSystemType(typeof(MeshRenderer)),
        new SerializableSystemType(typeof(MeshFilter)),
        new SerializableSystemType(typeof(Collider)),
        new SerializableSystemType(typeof(Light))
        };

        [HideInInspector]
        public List<bool> useDerivedClasses = new List<bool>
        {
            true, true, true, true, true
        };
        public GameObject[] autoGeneratedPrefabs;


        /*
        public string[] cosmeticTypes = new string[]
        {
            new SerializableSystemType(typeof(MeshRenderer)).ToString(),
            new SerializableSystemType(typeof(MeshFilter)).ToString(),
            new SerializableSystemType(typeof(Collider)).ToString(),
            new SerializableSystemType(typeof(Light)).ToString()
        };
        */
        public enum PlaybackRepicationPrecisionSettings
        {
            Exact, Partial, OnlyNecessary
        }
        [Header("Settings")]
        public bool replicateCurrentSceneOnly;
        public bool includeDerivedClassesInReplication; //Eventually do this on a class-by-class basis
        public PlaybackRepicationPrecisionSettings replicationPrecision;

        private void ReplicateFullHierarchy(List<GameObject> roots, Transform replicationParent)
        {
            foreach (GameObject go in roots)
            {
                ReplicateFullHierarchy_Internal(go, replicationParent);
            }
        }

        private void ReplicateFullHierarchy_Internal(GameObject originalGameObject, Transform replicationParent)
        {
            GameObject replicatedCurrent = ReplicateGameObject(originalGameObject, replicationParent);
            for (int i = 0; i < originalGameObject.transform.childCount; ++i)
            {
                GameObject replicatedChild = ReplicateGameObject(originalGameObject.transform.GetChild(i).gameObject, replicatedCurrent.transform);
                ReplicateFullHierarchy_Internal(originalGameObject.transform.GetChild(i).gameObject, replicatedChild.transform);
            }

            print(originalGameObject.name);
        }

        private bool TrySaveScenes(UnityEngine.SceneManagement.Scene[] scenes)
        {
            bool allClear = true;

            foreach (var scene in scenes)
            {
                if (!scene.isDirty && !string.IsNullOrEmpty(scene.name))
                {
                    continue;
                }

                else
                {
                    if (!UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene))
                    {
                        allClear = false;
                        Debug.Log($"Failed to save scene: {scene.name}");
                        break;
                    }

                    else
                    {
                        Debug.Log($"Saved Scene: {scene.name}");
                    }
                }
            }

            return allClear;
        }

        private bool GameobjectHasCosmeticChildren(GameObject root)
        {
            bool hasCosmeticChildren = false;
            foreach(var child in root.GetComponentsInChildren<Component>())
            {
                SerializableSystemType componentType = new SerializableSystemType(child.GetType());
                if
                    (
                    replicationTypes.Contains(componentType) ||
                    (includeDerivedClassesInReplication && replicationTypes.Any(x => componentType.SystemType.IsSubclassOf(x.SystemType)))
                    )
                {
                    hasCosmeticChildren = true;
                    break;
                }
            }

            return hasCosmeticChildren;
        }

        private GameObject ReplicateGameObject(GameObject originalGameObject, Transform parent)
        {
            if (originalGameObject == null)
            {
                return null;
            }
            Component[] comps = originalGameObject.GetComponents<Component>();
            GameObject replicatedClone = null;
            bool validRepComponentFound = false;

            foreach (var comp in comps)
            {
                SerializableSystemType componentType = new SerializableSystemType(comp.GetType());
                if (
                    replicationPrecision == PlaybackRepicationPrecisionSettings.Exact ||
                    replicationTypes.Contains(componentType) ||
                    (includeDerivedClassesInReplication && replicationTypes.Any(x => componentType.SystemType.IsSubclassOf(x.SystemType)))
                    )
                {
                    validRepComponentFound = true;
                    break;
                }
            }

            if (validRepComponentFound)
            {
                replicatedClone = Instantiate(originalGameObject);
                replicatedClone.name = originalGameObject.name;
                replicatedClone.transform.parent = parent;
                Component[] replicatedComps = replicatedClone.GetComponents<Component>();
                
                List<System.Type> reflectionsToAdd = new List<System.Type>();
                for (int i = replicatedComps.Length - 1; i > 0; --i) //Automatically exclude the Transform component
                {
                    //If we have a UPBS Tracker, get ready to add a Reflection
                    Data.UPBSTracker trackerComponent = replicatedComps[i] as UPBS.Data.UPBSTracker;
                    if (trackerComponent)
                    {
                        if (trackerComponent.ReflectionType.IsInstanceOfType(typeof(UPBS.Execution.PBReflection)))
                        {
                            reflectionsToAdd.Add(trackerComponent.ReflectionType);
                        }

                        else
                        {
                            Debug.LogWarning($"ReflectionType field of {trackerComponent.GetType().Name} must inherit from the class PBReflection");
                        }
                        
                    }

                    SerializableSystemType replicatedComponentType = new SerializableSystemType(replicatedComps[i].GetType());
                    if (!replicationTypes.Contains(replicatedComponentType) &&
                        !(includeDerivedClassesInReplication && replicationTypes.Any(x => replicatedComponentType.SystemType.IsSubclassOf(x.SystemType)))
                        )
                    {
                        DestroyImmediate(replicatedComps[i]);
                    }
                }
                foreach(var reflectionType in reflectionsToAdd)
                {
                    gameObject.AddComponent(reflectionType);
                }
            }

            return replicatedClone;
        }

        [ContextMenu("generate scene")]
        void Generate()
        {
            var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (EditorUtility.DisplayDialog
                ("Save Open Scenes?",
                "Playback Generation requires that all open scenes be saved. Are you sure you want to save?",
                "Save",
                "Do Not Save")
                )
            {
                UnityEngine.SceneManagement.Scene[] replicationScenes;
                UnityEngine.SceneManagement.Scene[] allOpenScenes;

                allOpenScenes = new UnityEngine.SceneManagement.Scene[UnityEditor.SceneManagement.EditorSceneManager.sceneCount];
                for (int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; ++i)
                {
                    allOpenScenes[i] = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);
                }

                if (replicateCurrentSceneOnly)
                {
                    replicationScenes = new UnityEngine.SceneManagement.Scene[] { UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene() };
                }

                else
                {
                    replicationScenes = allOpenScenes;
                }

                if (TrySaveScenes(allOpenScenes))
                {
                    List<GameObject> roots = new List<GameObject>();
                    foreach (var scene in replicationScenes)
                    {
                        roots.AddRange(scene.GetRootGameObjects().Where(x => GameobjectHasCosmeticChildren(x)));
                    }



                    var newScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Additive);
                    GameObject replicationParent = new GameObject();
                    replicationParent.name = "=== Generated Environment ===";
                    switch (replicationPrecision)
                    {
                        case PlaybackRepicationPrecisionSettings.Exact:
                            ReplicateFullHierarchy(roots, replicationParent.transform);
                            break;


                        default:
                            List<GameObject> allGameObjects = new List<GameObject>();
                            foreach (var go in roots)
                            {
                                allGameObjects.Add(go);
                                //allGameObjects.AddRange(go.transform.child)
                            }
                            break;
                    }

                    GameObject autoGeneratedParent = new GameObject();
                    autoGeneratedParent.name = "=== Playback Prefabs ===";
                    foreach(var prefab in autoGeneratedPrefabs)
                    {
                        if(PrefabUtility.IsPartOfAnyPrefab(prefab))
                        {
                            PrefabUtility.InstantiatePrefab(prefab, autoGeneratedParent.transform);
                        }

                        else
                        {
                            Instantiate(prefab, autoGeneratedParent.transform);
                        }
                        
                    }

                    new GameObject().name = "=== [Your Visualizers Here] ===";

                    //Now we do some cleanup...
                    //Do we build lighting?
                    //What other playback-essential gameobjects do we add?
                    //Do we close up all other open scenes
                }
            }
        }

        private void OnGUI()
        {

        }
    }
}
