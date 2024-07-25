using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Reflection;
using UPBS.Utility;

namespace UPBS.EditorScripts
{
    [InitializeOnLoad]
    public static class GeneratePlaybackSceneStatics
    {
        static GeneratePlaybackSceneStatics()
        {
            string defaultPath = EditorPrefs.GetString(Constants.EDITOR_DEFAULT_SCENE_GENERATOR, string.Empty);
            GeneratePlaybackScene generator = AssetDatabase.LoadAssetAtPath<GeneratePlaybackScene>(defaultPath);
            if (generator != null)
            {
                GeneratePlaybackScene.DefaultGenerator = generator;
            }
        }

        [MenuItem("UPBS/Generate Playback Scene")]
        public static void StaticGenerate()
        {
            if (GeneratePlaybackScene.DefaultGenerator == null)
            {
                Debug.LogWarning("No Default Scene Generator Found!");
                return;
            }

            GeneratePlaybackScene.DefaultGenerator.Generate();
        }
    }
    public class ComponentDependencyNode
    {
        public Component comp;
        public List<Component> dependencies = new List<Component>();

        public ComponentDependencyNode(Component co)
        {
            comp = co;
            var dependencyAttribs = co.GetType().GetCustomAttributes<RequireComponent>();
            foreach (var attrib in dependencyAttribs)
            {
                if (attrib.m_Type0 != null && attrib.m_Type0 != default(System.Type))
                {
                    Component[] dependantComponents = comp.gameObject.GetComponents(attrib.m_Type0);
                    if (dependantComponents != null && dependantComponents.Length > 0)
                    {
                        dependencies.AddRange(dependantComponents);
                    }
                }

                if (attrib.m_Type1 != null && attrib.m_Type1 != default(System.Type))
                {
                    Component[] dependantComponents = comp.gameObject.GetComponents(attrib.m_Type1);
                    if (dependantComponents != null && dependantComponents.Length > 0)
                    {
                        dependencies.AddRange(dependantComponents);
                    }
                }
                if (attrib.m_Type2 != null && attrib.m_Type2 != default(System.Type))
                {
                    Component[] dependantComponents = comp.gameObject.GetComponents(attrib.m_Type2);
                    if (dependantComponents != null && dependantComponents.Length > 0)
                    {
                        dependencies.AddRange(dependantComponents);
                    }
                }
            }
        }
    }
    public class ComponentDependencyNetwork
    {
        List<Component> comps;
        List<ComponentDependencyNode> nodes = new List<ComponentDependencyNode>();
        public ComponentDependencyNetwork(GameObject go)
        {
            comps = go.GetComponents<Component>().ToList();
            comps.RemoveAt(0); //Automatically exclude the Transform component
            foreach (var comp in comps)
            {
                nodes.Add(new ComponentDependencyNode(comp));
            }
        }

        public List<Component> GenerateDependencyChain()
        {
            bool noChange = false;
            List<ComponentDependencyNode> traversalList = new List<ComponentDependencyNode>(nodes);
            List<Component> retVal = new List<Component>();
            while (traversalList.Count != 0 && !noChange)
            {
                noChange = true;
                for(int i = traversalList.Count - 1; i >= 0; --i)
                {
                    bool anyActiveDepenants = false;
                    // Check if anyone else depends on this component. If not, we can have it be the next queued for deletion
                    for(int j = 0; j < traversalList.Count; ++j)
                    {
                        if (j != i)
                        {
                            if (traversalList[j].dependencies.Contains(traversalList[i].comp))
                            {
                                anyActiveDepenants = true;
                            }
                        }
                    }

                    if (!anyActiveDepenants)
                    {
                        retVal.Add(traversalList[i].comp);
                        traversalList.RemoveAt(i);
                        noChange = false;
                    }
                }
            }
            return retVal;
        }
    }

    [ExecuteAlways]
    [CreateAssetMenu(fileName = "Data", menuName = "UPBS/Playback Scene Generator", order = 1)]
    public class GeneratePlaybackScene : ScriptableObject
    {
        public static GeneratePlaybackScene DefaultGenerator = null;

        [HideInInspector]
        public List<SerializableSystemType> replicationTypes = new List<SerializableSystemType>
        {
        new SerializableSystemType(typeof(PBTrackerID)),
        new SerializableSystemType(typeof(Camera)),
        new SerializableSystemType(typeof(MeshRenderer)),
        new SerializableSystemType(typeof(MeshFilter)),
        new SerializableSystemType(typeof(Collider)),
        new SerializableSystemType(typeof(Light))
        };

        [HideInInspector]
        public List<bool> useDerivedClasses = new List<bool>
        {
            true, true, true, true, true, true
        };
        public GameObject[] autoGeneratedPrefabs;

        public enum PlaybackRepicationPrecisionSettings
        {
            Exact, Partial, OnlyNecessary
        }
        public bool replicateCurrentSceneOnly = true;
        [Tooltip("Automatically save the generated scene and add it to the project settings." +
            " If you're saving a scene manually, make sure it follows this format in order to be recognized at runtime:" +
            " [ReplicatedSceneName]-PLAYBACK-")]
        public bool autoSaveGeneratedScene = true;
        public PlaybackRepicationPrecisionSettings replicationPrecision;
        public string sceneSuffix = "ver1";

        private void Awake()
        {
            string defaultPath = EditorPrefs.GetString(Constants.EDITOR_DEFAULT_SCENE_GENERATOR, string.Empty);

            if (DefaultGenerator == null)
            {
                DefaultGenerator = AssetDatabase.LoadAssetAtPath<GeneratePlaybackScene>(defaultPath);
                if (DefaultGenerator)
                {
                    DefaultGenerator = this;
                    EditorPrefs.SetString(Constants.EDITOR_DEFAULT_SCENE_GENERATOR, AssetDatabase.GetAssetPath(this));
                }
            }
        }

        /// <summary>
        /// Replicate all root gameobjects along with their heirarchy under a parent gameobject
        /// </summary>
        /// <param name="roots"></param>
        private List<GameObject> ReplicateFullHierarchy(List<GameObject> roots)
        {
            List<GameObject> replicatedRoots = new List<GameObject>();
            foreach (GameObject go in roots)
            {
                GameObject replicatedRoot = Instantiate(go);
                replicatedRoot.name = go.name;
                replicatedRoots.Add(replicatedRoot);
            }

            return replicatedRoots;
        }

        /// <summary>
        /// Alter each root gameobject and it's children in accordance to the current replication settings
        /// </summary>
        /// <param name="roots"></param>
        /// <param name="replicationParent"></param>
        public void EnforceReplicationPattern(List<GameObject> roots, Transform replicationParent)
        {
            for(int root = 0; root < roots.Count; ++root)
            {
                EnforceReplicationPattern_Internal(roots[root].gameObject);
                if (replicationParent) roots[root].transform.parent = replicationParent;

                List<GameObject> children = new List<GameObject>();
                for (int child = 0; child < roots[root].transform.childCount; ++child)
                {
                    children.Add(roots[root].transform.GetChild(child).gameObject);
                }
                if (children.Count > 0)
                {
                    EnforceReplicationPattern(children, roots[root].transform);
                }
            }
            /*
            for (int root = 0; root < roots.Count; ++root)
            {
                EnforceReplicationPattern_Internal(roots[root].gameObject);
                roots[root].transform.parent = replicationParent;

                for (int child = 0; child < roots[root].transform.childCount; ++child)
                {
                    EnforceReplicationPattern_Internal(roots[root].transform.GetChild(child).gameObject);
                }
            }
            */
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

        private bool GameOjbectHasReplicableChildren(GameObject root)
        {
            bool hasCosmeticChildren = false;
            foreach(var child in root.GetComponentsInChildren<Component>())
            {
                SerializableSystemType componentType = new SerializableSystemType(child.GetType());
                if (CheckComponentForReplicationPattern(child))
                {
                    hasCosmeticChildren = true;
                    break;
                }
            }

            return hasCosmeticChildren;
        }

        /// <summary>
        /// Determine if the provided component can be replicated according to current settings
        /// </summary>
        /// <param name="comp"></param>
        /// <returns>True if component can be replicated</returns>
        private bool CheckComponentForReplicationPattern(Component comp)
        {
            SerializableSystemType componentType = new SerializableSystemType(comp.GetType());
            if (replicationTypes.Contains(componentType))
            {
                return true;
            }

            else
            {
                if(useDerivedClasses.Count != replicationTypes.Count)
                {
                    Debug.LogWarning("Replication Types and Derived Classes are of different sizes!!!");
                }

                for(int i = 0; i < replicationTypes.Count; ++i)
                {
                    if(useDerivedClasses[i] && comp.GetType().IsSubclassOf(replicationTypes[i].SystemType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void EnforceReplicationPattern_Internal(GameObject go)
        {
            ComponentDependencyNetwork network = new ComponentDependencyNetwork(go);
            List<Component> comps = network.GenerateDependencyChain();
            if (comps.Count != go.GetComponents<Component>().Length -1)
            {
                Debug.LogWarning("Circular Dependency Found on " + go.name);
            }
            bool validRepComponentFound = false;

            foreach (var comp in comps)
            {
                if (
                    replicationPrecision == PlaybackRepicationPrecisionSettings.Exact || //THIS SEEMS ODD. INVESTIGATE LATER
                    CheckComponentForReplicationPattern(comp)
                    )
                {
                    validRepComponentFound = true;
                    break;
                }
            }

            if (validRepComponentFound)
            {
                List<System.Type> reflectionsToAdd = new List<System.Type>();
                for (int i = 0; i < comps.Count; ++i) 
                {

                    //If we have a UPBS Tracker, get ready to add a Reflection
                    Data.UPBSTracker trackerComponent = comps[i] as UPBS.Data.UPBSTracker;
                    if (trackerComponent)
                    {
                        if (trackerComponent.ReflectionType.IsSubclassOf(typeof(UPBS.Player.PBReflection)))
                        {
                            reflectionsToAdd.Add(trackerComponent.ReflectionType);
                        }

                        else
                        {
                            Debug.LogWarning($"ReflectionType field of {trackerComponent.GetType().Name} must inherit from the class PBReflection");
                        }

                    }

                    if (!CheckComponentForReplicationPattern(comps[i]))
                    {
                        DestroyImmediate(comps[i], true);
                    }
                }

                foreach (var reflectionType in reflectionsToAdd)
                {
                    go.AddComponent(reflectionType);
                }
            }

            else
            {
                for (int i = 0; i < comps.Count; ++i)
                {
                    DestroyImmediate(comps[i], true);
                }
            }
        }

        /*
        private bool TryDestroyComponent(GameObject go, Component[] comps, int targetCompIndex)
        {
            if (!comps[targetCompIndex])
            {
                return false;
            }
            try
            {
                DestroyImmediate(comps[targetCompIndex]);
            }
            catch
            {

            }
        }
        */

        [ContextMenu("Generate Scene")]
        public void Generate()
        {
            var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (EditorUtility.DisplayDialog
                ("Save Open Scenes?",
                "Playback Generation requires that all open scenes be saved. Are you sure you want to save?",
                "Save",
                "Do Not Save")
                )
            {
                UnityEngine.SceneManagement.Scene[] scenesToReplicate;
                UnityEngine.SceneManagement.Scene[] allOpenScenes;

                allOpenScenes = new UnityEngine.SceneManagement.Scene[UnityEditor.SceneManagement.EditorSceneManager.sceneCount];
                for (int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; ++i)
                {
                    allOpenScenes[i] = UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i);
                }

                if (replicateCurrentSceneOnly)
                {
                    scenesToReplicate = new UnityEngine.SceneManagement.Scene[] { UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene() };
                }

                else
                {
                    scenesToReplicate = allOpenScenes;
                }

                if (TrySaveScenes(allOpenScenes))
                {
                    List<GameObject> roots = new List<GameObject>();
                    foreach (var scene in scenesToReplicate)
                    {
                        roots.AddRange(scene.GetRootGameObjects().Where(x => GameOjbectHasReplicableChildren(x)));
                    }



                    var newScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Additive);
                    GameObject replicationParent = new GameObject();
                    replicationParent.name = "=== Generated Environment ===";
                    switch (replicationPrecision)
                    {
                        case PlaybackRepicationPrecisionSettings.Exact:
                            var relicatedHeirarchy = ReplicateFullHierarchy(roots);
                            EnforceReplicationPattern(relicatedHeirarchy, replicationParent.transform);
                            break;

                            //TODO: IMPLEMENT OTHER REPLICATION METHODS

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

                    Lightmapping.BakeAsync();
                    //Now we do some cleanup...
                    //Do we build lighting?
                    //What other playback-essential gameobjects do we add?
                    //Do we close up all other open scenes

                    if (autoSaveGeneratedScene)
                    {
                        string pathToMimic = scenesToReplicate[0].path;
                        string filename = Path.GetFileNameWithoutExtension(pathToMimic) + Constants.PLAYBACK_SCENE_NAME_TAG + sceneSuffix;
                        string savePath = Path.Combine(Path.GetDirectoryName(pathToMimic), filename) + Path.GetExtension(pathToMimic);
                        Debug.Log(savePath);
                        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(newScene, savePath);

                        EditorBuildSettingsScene playbackSceneBuildSettings = new EditorBuildSettingsScene(savePath, true);
                        if (!EditorBuildSettings.scenes.Any(x => x.path == playbackSceneBuildSettings.path))
                        {
                            EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(playbackSceneBuildSettings).ToArray();

                            if (newScene.buildIndex == EditorBuildSettings.scenes.Length)
                            {
                                Debug.LogWarning("Failed to add scene to build settings!");
                            }
                        }
                        
                    }
                }
            }
        }

        public void SetAsDefault()
        {
            DefaultGenerator = this;
            EditorPrefs.SetString(Constants.EDITOR_DEFAULT_SCENE_GENERATOR, AssetDatabase.GetAssetPath(this));
        }

    }
}
