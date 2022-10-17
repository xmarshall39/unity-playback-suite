using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.IO;
using UPBS.Data;
using UnityEngine.SceneManagement;

namespace UPBS.Execution
{
    /// <summary>
    /// Find all frame data from the desired directory and trigger the initialization of 
    /// </summary>
    public class PBLoadingManager : MonoBehaviour
    {
        #region Singleton
        private static PBLoadingManager _instance;
        public static PBLoadingManager Instance
        {
            get
            {
                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        public bool DirectoryValidated { get; private set; }
        private PBTrackerInfo globalTrackerInfo, cameraTrackerInfo;
        private string globalTrackerPath = null, cameraTrackerPath = null;
        public bool ValidateDirectory(string trialDataPath, out string baseSceneName)
        {
            baseSceneName = "";
            globalTrackerPath = null;
            string globalInfoPath = null;
            cameraTrackerPath = null;
            string cameraInfoPath = null;

            string mandatoryFilePath = Path.Combine(trialDataPath, Constants.MANDATORY_DATA_DIR);

            foreach (var file in Directory.GetFiles(mandatoryFilePath))
            {
                string fileName = Path.GetFileName(file);
                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                bool trackerTagFound = false;
                bool camTagFound = false, globalTagFound = false;

                if (Path.GetExtension(file) == Constants.TRACKER_EXTENSION)
                {
                    string[] fileNameSplit = fileName.Split(Constants.FILENAME_DELIM);

                    foreach (string segment in fileNameSplit)
                    {
                        if (segment == Constants.UPBS_TRACKER_DESC)
                        {
                            trackerTagFound = true;
                        }
                        if (segment == Constants.UPBS_GLOBAL_DESC)
                        {
                            globalTagFound = true;
                        }
                        if (segment == Constants.UPBS_CAMERA_DESC)
                        {

                            camTagFound = true;
                        }
                    }

                    if (trackerTagFound && camTagFound && globalTagFound)
                    {
                        Debug.LogWarning($"{file} has tags for both a Camera tracker and Global tracker.");
                    }

                    else if (trackerTagFound && camTagFound)
                    {
                        cameraTrackerPath = file;
                    }

                    else if (trackerTagFound && globalTagFound)
                    {
                        globalTrackerPath = file;
                    }
                }

                else if (Path.GetExtension(file) == Constants.PB_INFO_EXTENSION)
                {
                    string[] fileNameSplit = fileName.Split(Constants.FILENAME_DELIM);

                    foreach (string segment in fileNameSplit)
                    {
                        if (segment == Constants.UPBS_TRACKER_DESC) trackerTagFound = true;
                        if (segment == Constants.UPBS_GLOBAL_DESC) globalTagFound = true;
                        if (segment == Constants.UPBS_CAMERA_DESC) camTagFound = true;
                    }

                    if (trackerTagFound && camTagFound && globalTagFound)
                    {
                        Debug.LogWarning($"{file} has tags for both a Camera tracker and Global tracker.");
                    }

                    else if (trackerTagFound && camTagFound)
                    {
                        cameraInfoPath = file;
                    }

                    else if (trackerTagFound && globalTagFound)
                    {
                        globalInfoPath = file;
                    }
                }

            }

            if (globalTrackerPath == null)
            {
                Debug.LogWarning("Global Tracker Path not found!");
                DirectoryValidated = false;
                return false;
            }
            if (globalInfoPath == null)
            {
                Debug.LogWarning("Global Info Path not found!");
                DirectoryValidated = false;
                return false;
            }
            if (cameraTrackerPath == null)
            {
                Debug.LogWarning("Camer Tracker Path not found!");
                DirectoryValidated = false;
                return false;
            }
            if (cameraInfoPath == null)
            {
                Debug.LogWarning("Camera Info Path not found!");
                DirectoryValidated = false;
                return false;
            }

            print(JsonUtility.ToJson(new PBTrackerInfo()));

            globalTrackerInfo = JsonUtility.FromJson<PBTrackerInfo>(File.ReadAllText(globalInfoPath));
            print(JsonUtility.ToJson(globalTrackerInfo));
            cameraTrackerInfo = JsonUtility.FromJson<PBTrackerInfo>(File.ReadAllText(cameraInfoPath));
            if (!globalTrackerInfo.IsValid() || !cameraTrackerInfo.IsValid())
            {
                Debug.LogWarning("Unable to parse mandatory tracker info");
                DirectoryValidated = false;
                return false;
            }

            DirectoryValidated = true;
            baseSceneName = globalTrackerInfo.originalSceneName;
            return true;
        }

        public void Load(string trialDataPath, string playbackSceneName)
        {
            StartCoroutine(Load_Internal(trialDataPath, playbackSceneName));
        }

        /// <summary>
        /// This should load in all of our Tracker data provided a base directory.
        /// This is also the start of our Manager initialization chain (not actually a chain)
        /// </summary>
        public IEnumerator Load_Internal(string trialDataPath, string playbackSceneName)
        {
            //TODO: Implement a comprehensive file search system
            //FileLocation: [TrialDir]/[MandatoryTrackerDir]/[gameobjectName]_UPBSTracker_Global_[TrialNumber].csv
            //So we check for non-variable substrings by splitting by underscore
            //When I fork UXF, I might make the measurement descriptor static...
            
            if (DirectoryValidated == false)
            {
                Debug.LogWarning("Provided Data Path not validated!");
                yield break;
            }

            yield return SceneManager.LoadSceneAsync(playbackSceneName);
            yield return null;
            if(PBFrameLibraryManager.Instance == null)
            {
                Debug.LogWarning("No lib found in new scene");
            }

            if 
            (
                !PBFrameLibraryManager.Instance.AddLibraryEntry(globalTrackerInfo, File.ReadAllLines(globalTrackerPath)) ||
                !PBFrameLibraryManager.Instance.AddLibraryEntry(cameraTrackerInfo, File.ReadAllLines(cameraTrackerPath))
            )
            {
                Debug.LogError("Unable to add mandatory Library entry!");
                yield break;
            }


            PBFrameLibraryManager.Instance.SetGlobalTID(globalTrackerInfo.TID);
            Debug.Log("Mandatory trackers successfully loaded in the dictionary");
            string additionalFilePath = Path.Combine(trialDataPath, Constants.ADDITIONAL_DATA_DIR);
            string[] additionalFiles = Directory.GetFiles(additionalFilePath);
            UPBS.UI.LoadingBar.Instance?.BeginLoading(additionalFiles.Length);

            Dictionary<int, TrackerProcessor> processingDict = new Dictionary<int, TrackerProcessor>();

            
            foreach(var file in additionalFiles)
            {
                //Update the loading bar early so we prompt it to completion slightly before loading is done
                UPBS.UI.LoadingBar.Instance?.UpdateProgress(1);//This will need to be async to work.
                string fileName = Path.GetFileName(file);
                string fileNameSansExtension = Path.GetFileNameWithoutExtension(file);
                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                string[] fileNameSplit = fileName.Split(Constants.FILENAME_DELIM);
                if (float.TryParse(fileNameSplit[0], out float temp) && temp >= 0)
                {
                    int tid = (int)temp;
                    if (!processingDict.ContainsKey(tid))
                    {
                        processingDict[tid] = new TrackerProcessor();
                    }

                    if (Path.GetExtension(fileName) == Constants.TRACKER_EXTENSION)
                    {
                        processingDict[tid].TrackerPath = fileName;
                    }

                    else if (Path.GetExtension(fileName) == Constants.PB_INFO_EXTENSION)
                    {
                        PBTrackerInfo info = JsonUtility.FromJson<PBTrackerInfo>(File.ReadAllText(fileName));
                        processingDict[tid].TrackerInfo = info;
                    }
                }
                yield return null;
            }

            //TODO: Implement a threaded solution to loading
            //Okay, global data is loaded into the library. Now let's do this again for the camera tracker...

            //If neither failed, we're free to load individual trackers...

            //Whatever we got, it's probably fine. Most parsing issues for basic trackers aren't critical.
            //So unless something exceptional happens, we're good.
            PBFrameLibraryManager.Instance.CleanUp();
            PBFrameController.Instance?.Initialize(); // Make this event-based later
            //IN PARALLELL: Any external data should be loaded as well as determined by the developer. Let's assume that's all done for now...

            //With all our loading complete, we can now enable the FrameControllerManager and then Initialize the UI
            UPBS.UI.LoadingBar.Instance?.FinishLoading();
        }

        private async Task MyTask(int i)
        {
            Task.FromResult("");
        }


        //A class to handle the automatic addition of TrackerInformation into the FrameLibrary
        private class TrackerProcessor
        {
            private PBTrackerInfo _trackerInfo = null;
            public PBTrackerInfo TrackerInfo { get => _trackerInfo; set { _trackerInfo = value; Validate(); } }

            private string _trackerPath = null;
            public string TrackerPath { get => _trackerPath; set { _trackerPath = value; Validate(); } }

            private void Validate()
            {
                if(TrackerInfo != null && TrackerPath != null)
                {
                    PBFrameLibraryManager.Instance.AddLibraryEntry(TrackerInfo, File.ReadAllLines(TrackerPath));
                }
            }
        }
    }

}

