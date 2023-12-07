namespace UPBS.Data
{
    /// <summary>
    /// A simple class that allows us to serialize extra, constant properties of a tracked gameobject.
    /// </summary>
    [System.Serializable]
    public class PBTrackerInfo
    {
        public int TID = -1;
        public float recordingRate = -1;
        public string frameDataAssemblyName = "";
        public string originalSceneName = "";

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

