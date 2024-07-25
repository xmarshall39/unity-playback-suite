using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using UPBS.Data;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UPBS.Player
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

        public static string LoadedDirectory { get; private set; }

        public bool DirectoryValidated { get; private set; }
        public PBTrackerInfo GlobalTrackerInfo { get; private set; } = null;
        public PBTrackerInfo CameraTrackerInfo { get; private set; } = null;
        private string globalTrackerPath = null, cameraTrackerPath = null;
        public bool ValidateDirectory(string trialDataPath, out string baseSceneName)
        {
            baseSceneName = "";
            globalTrackerPath = null;
            string globalInfoPath = null;
            cameraTrackerPath = null;
            string cameraInfoPath = null;

            string mandatoryFilePath = Path.Combine(trialDataPath, Constants.MANDATORY_DATA_DIR);
            if (!Directory.Exists(mandatoryFilePath)) return false;

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
                Debug.LogWarning("Directory Not Valid: Global Tracker Path not found!");
                DirectoryValidated = false;
                return false;
            }
            if (globalInfoPath == null)
            {
                Debug.LogWarning("Directory Not Valid: Global Info Path not found!");
                DirectoryValidated = false;
                return false;
            }
            if (cameraTrackerPath == null)
            {
                Debug.LogWarning("Directory Not Valid: Camera Tracker Path not found!");
                DirectoryValidated = false;
                return false;
            }
            if (cameraInfoPath == null)
            {
                Debug.LogWarning("Directory Not Valid: Camera Info Path not found!");
                DirectoryValidated = false;
                return false;
            }

            GlobalTrackerInfo = JsonUtility.FromJson<PBTrackerInfo>(File.ReadAllText(globalInfoPath));
            print(JsonUtility.ToJson(GlobalTrackerInfo));
            CameraTrackerInfo = JsonUtility.FromJson<PBTrackerInfo>(File.ReadAllText(cameraInfoPath));
            if (!GlobalTrackerInfo.IsValid() || !CameraTrackerInfo.IsValid())
            {
                Debug.LogWarning("Directory Not Valid: Unable to parse mandatory tracker info");
                DirectoryValidated = false;
                return false;
            }

            DirectoryValidated = true;
            baseSceneName = GlobalTrackerInfo.originalSceneName;
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
            UPBS.UI.LoadingBar.Instance?.BeginLoading(1);
            if (DirectoryValidated == false)
            {
                Debug.LogWarning("Provided Data Path not validated!");
                yield break;
            }

            yield return SceneManager.LoadSceneAsync(playbackSceneName);
            UPBS.UI.LoadingBar.Instance?.UpdateProgress(1);
            yield return null;
            if(PBFrameLibraryManager.Instance == null)
            {
                Debug.LogWarning("No lib found in new scene");
            }

            if 
            (
                !PBFrameLibraryManager.Instance.AddLibraryEntry(GlobalTrackerInfo, File.ReadAllLines(globalTrackerPath)) ||
                !PBFrameLibraryManager.Instance.AddLibraryEntry(CameraTrackerInfo, File.ReadAllLines(cameraTrackerPath))
            )
            {
                Debug.LogError("Unable to add mandatory Library entry!");
                yield break;
            }


            PBFrameLibraryManager.Instance.SetGlobalTID(GlobalTrackerInfo.TID);
            Debug.Log("Mandatory trackers successfully loaded in the dictionary");
            string additionalFilePath = Path.Combine(trialDataPath, Constants.ADDITIONAL_DATA_DIR);
            string[] additionalFiles = Directory.GetFiles(additionalFilePath);
            UPBS.UI.LoadingBar.Instance?.AddLoadingObjectives(additionalFiles.Length);

            Dictionary<int, TrackerProcessor> processingDict = new Dictionary<int, TrackerProcessor>();
            Transform dynamicReflectionRoot = GameObject.Find("=== Generated Environment ===").transform;
            DynamicReflectionLoader reflectionLoader = new DynamicReflectionLoader(dynamicReflectionRoot);
            Coroutine processTrackerRoutine = StartCoroutine(reflectionLoader.ProcessTrackers());
            
            foreach(var file in additionalFiles)
            {
                //Update the loading bar early so we prompt it to completion slightly before loading is done
                UPBS.UI.LoadingBar.Instance?.UpdateProgress(1);//This will need to be async to work.
                string fileName = Path.GetFileName(file);
                // string fileNameSansExtension = Path.GetFileNameWithoutExtension(file);
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
                        processingDict[tid] = new TrackerProcessor(reflectionLoader);
                    }

                    if (Path.GetExtension(fileName) == Constants.TRACKER_EXTENSION)
                    {
                        processingDict[tid].TrackerPath = file;
                    }

                    else if (Path.GetExtension(fileName) == Constants.PB_INFO_EXTENSION)
                    {
                        PBTrackerInfo info = JsonUtility.FromJson<PBTrackerInfo>(File.ReadAllText(file));
                        processingDict[tid].TrackerInfo = info;
                    }
                }
                Resources.UnloadUnusedAssets();
                yield return null;
            }

            reflectionLoader.readyToDie = true;
            yield return processTrackerRoutine;

            LoadedDirectory = trialDataPath;
            //TODO: Implement a threaded solution to loading
            //Okay, global data is loaded into the library. Now let's do this again for the camera tracker...

            //If neither failed, we're free to load individual trackers...

            //Whatever we got, it's probably fine. Most parsing issues for basic trackers aren't critical.
            //So unless something exceptional happens, we're good.
            PBFrameLibraryManager.Instance.CleanUp();
            PBFrameController.Instance?.Initialize(); // Make this event-based later
            PBCameraManager.Instance?.Initialize();
            //IN PARALLELL: Any external data should be loaded as well as determined by the developer. Let's assume that's all done for now...

            //With all our loading complete, we can now enable the FrameControllerManager and then Initialize the UI
            UPBS.UI.LoadingBar.Instance?.FinishLoading();
        }


        ///Handles the automatic addition of TrackerInformation into the FrameLibrary
        private class TrackerProcessor
        {
            private DynamicReflectionLoader _dynamicLoader;
            public TrackerProcessor(DynamicReflectionLoader dynamicLoader)
            {
                _dynamicLoader = dynamicLoader;
            }

            private PBTrackerInfo _trackerInfo = null;
            public PBTrackerInfo TrackerInfo { get => _trackerInfo; set { _trackerInfo = value; Validate(); } }

            private string _trackerPath = null;
            public string TrackerPath { get => _trackerPath; set { _trackerPath = value; Validate(); } }

            private void Validate()
            {
                if(TrackerInfo != null && TrackerPath != null)
                {
                    if(
                        PBFrameLibraryManager.Instance.AddLibraryEntry(TrackerInfo, File.ReadAllLines(TrackerPath))
                        && TrackerInfo.dynamicallyCreated
                        )
                    {
                        _dynamicLoader.Add(TrackerInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the created of reflecitons belonging to dynamically instantiated trackers.
        /// DOC: Dynamically instantiated trackers are handled using the following process:
        /// 1.) Create a prefab of your tracked gameobject(s) as normal. Note that Tracker ID's don't need to be initialized
        /// 2.) Find the PlaybackSceneGenerator used for your scene and set it as the default
        /// 3.) Right click on the prefab asset and select Asset > UPBS > Create Reflection Prefab
        ///     a.) This will duplicate the prefab using the reflection settings for your scene and link both prefabs using metadata
        /// 4.) When instantiating the prefabs in script, call InitializeDynamicallySpawnedTracker() on every tracker in it's hierarchy
        /// 5.) Once data has been collected 
        /// </summary>
        public class DynamicReflectionLoader
        {
            public DynamicReflectionLoader(Transform objectParent)
            {
                _parent = objectParent;
            }

            private Transform _parent;
            private Queue<PBTrackerInfo> trackerInfoQueue = new Queue<PBTrackerInfo>();
            public bool readyToDie = false;

            private Dictionary<int, (GameObject, PBTrackerID[])> spawnHistory = new Dictionary<int, (GameObject, PBTrackerID[])>(); 

            public void Add(PBTrackerInfo trackerInfo)
            {
                trackerInfoQueue.Enqueue(trackerInfo);
            }

            public IEnumerator ProcessTrackers()
            {
                HashSet<string> spawnedAddressables = new HashSet<string>();
                while(!readyToDie || trackerInfoQueue.Count > 0)
                {
                    if (trackerInfoQueue.Count <= 0)
                    {
                        yield return null;
                        continue;
                    }

                    PBTrackerInfo info = trackerInfoQueue.Dequeue();
                    if (!spawnHistory.ContainsKey(info.prefabInstanceID))
                    {
                        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(info.assetAddress, _parent);
                        yield return handle;

                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            spawnedAddressables.Add(info.assetAddress);

                            if (handle.Result.TryGetComponent(out PBTrackerID id))
                            {
                                id.SetIDManual(info.TID);
                                id.gameObject.name += $"_{info.TID}";
                                handle.Result.name += $"_{info.TID}";
                            }
                            spawnHistory.Add(info.prefabInstanceID, (handle.Result, handle.Result.GetComponentsInChildren<PBTrackerID>()));
                        }
                    }
                    else
                    {
                        PBTrackerID matchedID = spawnHistory[info.prefabInstanceID].Item2.Where(x => x.PositionInPrefabHierarchy == info.prefabHierarchyKey) as PBTrackerID;
                        if (matchedID)
                        {
                            matchedID.SetIDManual(info.TID);
                            matchedID.gameObject.name += $"_{info.TID}";
                            spawnHistory[info.prefabInstanceID].Item1.name += $"_{info.TID}";
                        }
                    }
                }
            }
        }
    }

}

