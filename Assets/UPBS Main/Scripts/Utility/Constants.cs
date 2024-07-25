using UnityEngine;

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
        public readonly static string UPBS_COL_DESC = "-COLOR-";
        public readonly static string UPBS_SCALE_DESC = "-SCALE-";


        public readonly static string ADDITIONAL_DATA_DIR = "AdditionalTrackers";
        public readonly static string MANDATORY_DATA_DIR = "Mandatory";
        //public readonly static string[] CAM_TRACKER_DIR = new string[] {"Mandatory Trackers", ""}
        public readonly static string PB_EXPORT_DIR = "Playback Export";
        public readonly static string ADDITIONAL_PB_INFO_DIR = "Additional Playback Info";
        public readonly static string PLAYBACK_SCENE_NAME_TAG = "-PLAYBACK-";


        #region Player Pref Keys
        public readonly static string LAST_TRIAL_DIRECTORY = "PB_LAST_TRIAL_DIRECTORY";
        #endregion

        #region Editor
        public readonly static string EDITOR_DEFAULT_SCENE_GENERATOR = "UPBS_EDITOR_DEFAULT_GENERATOR_PATH";
        public readonly static string EDITOR_PLAYBACK_INIT_SCENE_PATH = "UPBS_EDITOR_PLAYBACK_INIT_SCENE_PATH";
        public readonly static string EDITOR_PLAYBACK_INIT_SCENE_NAME = "PlaybackInitialization";
        public readonly static string REFLECTION_ADDRESSABLE_GROUP = "UPBS_REFLECTION";
        #endregion

        #region Colors
        public static Color NormalizeColor(float r, float g, float b) => new Color(r / 255, g / 255, b / 255);
        public static UnityEngine.Color BRAND_COLOR => NormalizeColor(116, 184, 248);
        public static UnityEngine.Color SECONDARY_COLOR => NormalizeColor(198, 228, 255);
        public static Color EDITOR_BRAND_COLOR_LIGHT = NormalizeColor(222, 55, 247);
        public static Color EDITOR_BRAND_COLOR_DARK = NormalizeColor(51, 31, 50);
        public static Color EDITOR_SECONDARY_BRAND_COLOR_LIGHT = NormalizeColor(222, 55, 247);
        public static Color EDITOR_SECONDARY_BRAND_COLOR_DARK = NormalizeColor(61, 53, 63);
        public static Color UNITY_ERROR_COLOR = NormalizeColor(222, 55, 247);
        #endregion

    }
}

