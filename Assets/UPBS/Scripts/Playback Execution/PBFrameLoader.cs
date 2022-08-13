using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.IO;
using UPBS.Data;

namespace UPBS.Execution
{
    /// <summary>
    /// Find all frame data from the desired directory and trigger the initialization of 
    /// </summary>
    public class PBFrameLoader : MonoBehaviour
    {
        #region Singleton
        private static PBFrameLoader _instance;
        public static PBFrameLoader Instance
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
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        public string globalDataPath;

        //This way we won't need to initialize parsers for every tracker we read from

        /// <summary>
        /// This should load in all of our Tracker data provided a base directory.
        /// This is also the start of our Manager initialization chain (not actually a chain)
        /// </summary>
        [EasyButtons.Button]
        public void Load(string trialFolderDirectory)
        {
            //TODO: Implement a comprehensive file search system
            //FileLocation: [TrialDir]/[MandatoryTrackerDir]/[gameobjectName]_UPBSTracker_Global_[TrialNumber].csv
            //So we check for non-variable substrings by splitting by underscore
            //When I fork UXF, I might make the measurement descriptor static...
            string globalTrackerPath = null;
            string globalInfoPath = null;
            string cameraTrackerPath = null;
            string camerInfoPath = null;

            string mandatoryFilePath = Path.Combine(trialFolderDirectory, Constants.MANDATORY_DATA_DIR);
            foreach(var file in Directory.GetFiles(mandatoryFilePath))
            {
                string fileName = Path.GetFileName(file);
                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                if (Path.GetExtension(file) == Constants.TRACKER_EXTENSION)
                {
                    string[] fileNameSplit = fileName.Split(Constants.FILENAME_DELIM);

                    if (fileNameSplit.Length >= 3)
                    {
                        if (fileNameSplit[1] == Constants.UPBS_TRACKER_DESC)
                        {
                            //Found the global tracker. Let's remember this...
                            if (fileNameSplit[2] == Constants.UPBS_GLOBAL_DESC)
                            {
                                globalTrackerPath = file;
                            }

                            //Found the camera tracker. Let's remember this...
                            else if (fileNameSplit[2] == Constants.UPBS_CAMERA_DESC)
                            {
                                cameraTrackerPath = file;
                            }

                        }

                    }

                }

                else if (Path.GetExtension(file) == Constants.PB_INFO_EXTENSION)
                {
                    string[] fileNameSplit = fileName.Split(Constants.FILENAME_DELIM);

                    //Not sure how I want to verify file name yet...
                }
                
            }

            if (globalTrackerPath == null)
            {
                Debug.LogWarning("Global Tracker Path not found!");
                return;
            }
            if (globalInfoPath == null)
            {
                Debug.LogWarning("Global Tracker Path not found!");
                //return;
            }
            if (cameraTrackerPath == null)
            {
                Debug.LogWarning("Camer Tracker Path not found!");
                //return;
            }
            if (camerInfoPath == null)
            {
                Debug.LogWarning("Camera Info Path not found!");
                //return;
            }

            PBTrackerInfo globalTrackerInfo = JsonUtility.FromJson<PBTrackerInfo>(File.ReadAllText(globalInfoPath));
            if (!globalTrackerInfo.IsValid())
            {
                Debug.LogWarning("Invalid Global Tracker Info!");
                // return;
            }

            //TODO: Get proper TID
            if (!PBFrameLibraryManager.Instance.AddLibraryEntry(globalTrackerInfo, File.ReadAllLines(globalDataPath)))
            {
                Debug.LogError("Global data path not found!");
                return;
            }

            

            

            //TODO: Implement a threaded solution to loading
            //Okay, global data is loaded into the library. Now let's do this again for the camera tracker...

            //If neither failed, we're free to load individual trackers...

            //Whatever we got, it's probably fine. Most parsing issues for basic trackers aren't critical.
            //So unless something exceptional happens, we're good.
            PBFrameLibraryManager.Instance.CleanUp();
            //IN PARALLELL: Any external data should be loaded as well as determined by the developer. Let's assume that's all done for now...

            //With all our loading complete, we can now enable the FrameControllerManager and then Initialize the UI

        }

    }

}

