namespace UPBS.Data
{
    /// <summary>
    /// A simple class that allows us to serialize extra, constant properties of a tracked gameobject.
    /// </summary>
    [System.Serializable]
    public class PBTrackerInfo
    {
        public int TID = -1;
        public string frameDataAssemblyName = "";

        public bool IsValid()
        {
            return 
                TID >= 0 &&
                !string.IsNullOrEmpty(frameDataAssemblyName) &&
                System.Type.GetType(frameDataAssemblyName).IsSubclassOf(typeof(PBFrameDataBase));
        }
    }
}

