#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Linq;


namespace UPBS.EditorScripts
{
    public class PBPrefabFactory
    {
        [MenuItem("Assets/Create/UPBS", false, 10)]
        [MenuItem("Assets/Create/UPBS/Create Reflection Prefab", false, 0)]
        static public void CreateReflectionPrefab()
        {
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string basePath = Path.GetDirectoryName(selectedPath);
            string originalFileName = Path.GetFileNameWithoutExtension(selectedPath);
            string reflectionSavePath = Path.Combine(basePath, originalFileName + "_Reflection.prefab");
            Debug.Log(reflectionSavePath);

            // Use the transform sibling index to generate a unique key on each tracker that maps to a reflection
            using (var baseObjEditScope = new PrefabUtility.EditPrefabContentsScope(selectedPath))
            {
                foreach (var id in baseObjEditScope.prefabContentsRoot.GetComponentsInChildren<PBTrackerID>())
                {
                    id.SetHierarchyPosition(id.transform.GetSiblingIndex());
                }
            }

            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(reflectionSavePath) != null)
            {
                AssetDatabase.DeleteAsset(reflectionSavePath);
                AssetDatabase.Refresh();
            }
            AssetDatabase.CopyAsset(selectedPath, reflectionSavePath);
            AssetDatabase.Refresh();

            using (var editScope = new PrefabUtility.EditPrefabContentsScope(reflectionSavePath))
            {
                GeneratePlaybackScene.DefaultGenerator.EnforceReplicationPattern(new List<GameObject>() { editScope.prefabContentsRoot }, null);
            }

            SetAddressableGroup(AssetDatabase.LoadAssetAtPath<GameObject>(reflectionSavePath), Constants.REFLECTION_ADDRESSABLE_GROUP);
            AssetDatabase.Refresh();

            using (var baseObjEditScope = new PrefabUtility.EditPrefabContentsScope(selectedPath))
            {
                foreach (var tracker in baseObjEditScope.prefabContentsRoot.GetComponentsInChildren<UPBS.Data.UPBSTracker>())
                {
                    string path = reflectionSavePath.Replace('\\', '/');
                    tracker.reflectionPrefabAssetPath = path;
                    tracker.reflectionPrefabAddress = path;
                }
                PrefabUtility.SaveAsPrefabAsset(baseObjEditScope.prefabContentsRoot, selectedPath);
            }
        }

        /// Allow Reflection prefab creation if any gameobject in the prefab heirarchy has a UPBS Tracker
        [MenuItem("Assets/Create/UPBS/Create Reflection Prefab", true)]
        private static bool CreateReflectionPrefab_Validation()
        {
            return (Selection.activeGameObject != null && Selection.activeGameObject.GetComponentInChildren<Data.UPBSTracker>() != null && PrefabUtility.IsPartOfPrefabAsset(Selection.activeGameObject));
        }

        /// <summary>
        /// Mark a asset as addressable and assign a group
        /// </summary>
        public static void SetAddressableGroup(Object obj, string groupName)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings)
            {
                var group = settings.FindGroup(groupName);
                if (!group)
                    group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));

                var assetpath = AssetDatabase.GetAssetPath(obj);
                var guid = AssetDatabase.AssetPathToGUID(assetpath);

                var e = settings.CreateOrMoveEntry(guid, group, false, false);
                var entriesAdded = new List<AddressableAssetEntry> { e };

                group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
            }
        }

        [MenuItem("UPBS/Open Playback Init Scene")]
        public static void OpenPlaybackInitScene()
        {
            string initScenePath = EditorPrefs.GetString(Constants.EDITOR_PLAYBACK_INIT_SCENE_PATH, string.Empty);

            if (
                initScenePath != string.Empty
                && UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()
                )
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(initScenePath);
            }

            else if (initScenePath == string.Empty)
            {
                Debug.LogWarning($"The following path cannot open the playback init scene: {initScenePath} . Please open the \"{Constants.EDITOR_PLAYBACK_INIT_SCENE_NAME}\" scene from the Project window to enable this feature");
            }
        }
    }
}

#endif