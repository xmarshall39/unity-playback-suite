using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
namespace UPBS.UI
{
    //TODO: Add UPBS player prefs prefix to globals
    public class FillableDirectoryField : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _directoryText;
        [SerializeField]
        private TMP_Dropdown validPlaybackScenesDropdown;
        [SerializeField]
        private Button loadButton;
        public delegate void OnDirectoryChanged();
        public OnDirectoryChanged directoryChanged;

        private void Start()
        {
            if(PlayerPrefs.HasKey(UPBS.Constants.LAST_TRIAL_DIRECTORY))
            {
                SetDirectory(PlayerPrefs.GetString(UPBS.Constants.LAST_TRIAL_DIRECTORY));
            }
        }

        private void SetDirectory(string directory)
        {
            _directoryText.text = directory;
            PlayerPrefs.SetString(UPBS.Constants.LAST_TRIAL_DIRECTORY, directory);
            directoryChanged?.Invoke();

            validPlaybackScenesDropdown.ClearOptions();
            if (UPBS.Execution.PBLoadingManager.Instance != null && UPBS.Execution.PBLoadingManager.Instance.ValidateDirectory(_directoryText.text, out string baseSceneName))
            {
                TryPopulateSceneDropdown(baseSceneName);
            }

            else
            {
                Debug.LogWarning("Invalid Directory");
                validPlaybackScenesDropdown.gameObject.SetActive(false);
                loadButton.interactable = false;
            }
            
        }

        public bool IsDirectoryFilled() => string.IsNullOrWhiteSpace(_directoryText.text);

        public void SetTrialDirectory()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            string[] selected = SFB.StandaloneFileBrowser.OpenFolderPanel("Select data directory", _directoryText.text, false);
            if (selected != null && selected.Length > 0)
            {
                SetDirectory(selected[0]);
            }
#else
            Utilities.UXFDebugLogError("Cannot select directory unless on PC platform!");
#endif
        }

        public void Clear()
        {
            SetDirectory("");
        }

        public void LoadData()
        {
            UPBS.Execution.PBLoadingManager.Instance?.Load(_directoryText.text, validPlaybackScenesDropdown.options[validPlaybackScenesDropdown.value].text);
        }

        public void ReturnToStart()
        {
            SceneManager.LoadScene(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(0)));
        }

        public void ValidateSceneSelection()
        {
            
        }

        public void TryPopulateSceneDropdown(string baseSceneName)
        {
            List<string> validScenes = new List<string>();
            for(int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                if (sceneName.StartsWith(baseSceneName) && sceneName.Contains(Constants.PLAYBACK_SCENE_NAME_TAG) && sceneName != baseSceneName)
                {
                    validScenes.Add(sceneName);
                }
            }

            if(validScenes.Count > 0)
            {
                validPlaybackScenesDropdown.AddOptions(validScenes);
                validPlaybackScenesDropdown.gameObject.SetActive(true);
                loadButton.interactable = true;
            }

            else
            {
                validPlaybackScenesDropdown.gameObject.SetActive(false);
                loadButton.interactable = false;
                Debug.LogWarning("No valid scenes generated in the build");
            }
        }
    }
}

