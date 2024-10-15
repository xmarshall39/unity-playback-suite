using System;
namespace UPBS.Data
{
    /// <summary>
    /// A simple class that allows us to serialize extra, constant properties of a tracked gameobject.
    /// </summary>
    [System.Serializable]
    public class PBExtrnalDataInfo : ICloneable
    {
        public string filename;
        public string frameDataAssemblyName = "";

        public long firstTimestamp = -1;
        public long lastTimestamp = -1;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool IsValid()
        {
            return
                !string.IsNullOrEmpty(filename) &&
                !string.IsNullOrEmpty(frameDataAssemblyName) &&
                System.Type.GetType(frameDataAssemblyName).IsSubclassOf(typeof(PBFrameDataBase));
        }
    }
}

