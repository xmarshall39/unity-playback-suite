namespace UPBS
{
    public static class Constants
    {
        public readonly static char FILENAME_DELIM = '_';
        public readonly static char TRACKER_DELIM = ',';


        public readonly static string TRACKER_EXTENSION = ".csv";
        public readonly static string PB_INFO_EXTENSION = ".json";


        public readonly static string STR_NULL = "NULL";
        public readonly static string NAN = "NAN";

        public readonly static string UPBS_TRACKER_DESC = "-UPBSTRACKER-";
        public readonly static string UPBS_GLOBAL_DESC = "-GLOBAL-";
        public readonly static string UPBS_CAMERA_DESC = "-CAMERA-";
        public readonly static string UPBS_POS_ROT_DESC = "-POS_ROT-";

        public readonly static string ADDITIONAL_DATA_DIR = "AdditionalTrackers";
        public readonly static string MANDATORY_DATA_DIR = "Mandatory";
        //public readonly static string[] CAM_TRACKER_DIR = new string[] {"Mandatory Trackers", ""}
        public readonly static string PB_EXPORT_DIR = "Playback Export";
        public readonly static string ADDITIONAL_PB_INFO_DIR = "Additional Playback Info";


        #region Player Pref Keys
        public readonly static string LAST_TRIAL_DIRECTORY = "PB_LAST_TRIAL_DIRECTORY";
        #endregion

    }
}

