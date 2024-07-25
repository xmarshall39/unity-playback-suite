using System;
namespace UPBS.Data
{
    /// <summary>
    /// A simple class that allows us to serialize extra, constant properties of a tracked gameobject.
    /// </summary>
    [System.Serializable]
    public class PBTrackerInfo : ICloneable
    {
        public int TID = -1;
        public int prefabHierarchyKey = -1;
        public int prefabInstanceID = 0;
        public float recordingRate = -1;
        public string frameDataAssemblyName = "";
        public string originalSceneName = "";

        // Optional settigns for dynamically instantiated GameOjbects
        public bool dynamicallyCreated = false;
        public string assetAddress = ""; //Used for addressables
        public string replicationPrefabAssetPath = ""; //MIght replace with AssetBundle instead
        public long firstTimestamp = -1;
        public long lastTimestamp = -1;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool IsValid()
        {
            return 
                TID >= 0 &&
                recordingRate >= 0 &&
                !string.IsNullOrEmpty(frameDataAssemblyName) &&
                !string.IsNullOrEmpty(originalSceneName) &&
                System.Type.GetType(frameDataAssemblyName).IsSubclassOf(typeof(PBFrameDataBase));
        }
    }
}

