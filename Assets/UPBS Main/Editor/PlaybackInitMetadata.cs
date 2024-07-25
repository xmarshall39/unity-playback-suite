using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UPBS.EditorScripts
{
    [ExecuteAlways]
    public class PlaybackInitMetadata : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Awake()
        {
            var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (currentScene.path.Contains(Constants.EDITOR_PLAYBACK_INIT_SCENE_NAME))
            {
                EditorPrefs.SetString(Constants.EDITOR_PLAYBACK_INIT_SCENE_PATH, currentScene.path);
                Debug.Log($"Set Playback Init Path to: {currentScene.path}");
            }
        }
#endif
    }
}
