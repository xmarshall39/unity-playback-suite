using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UXF
{
    
    public abstract class DataHandler : MonoBehaviour
    {
        public bool active = true;
        public Session session { get; private set; }

        public void Initialise(Session session)
        {
            this.session = session;
        }

        public abstract bool CheckIfRiskOfOverwrite(string experiment, string ppid, int sessionNum, string rootPath = ""); 
        public virtual void SetUp() { }
        public abstract string HandleDataTable(UXFDataTable table, string experiment, string ppid, int sessionNum, string dataName, UXFDataType dataType, int optionalTrialNumber = 0);
        public abstract string HandleJSONSerializableObject(List<object> serializableObject, string experiment, string ppid, int sessionNum, string dataName, UXFDataType dataType, int optionalTrialNumber = 0);
        public abstract string HandleJSONSerializableObject(Dictionary<string, object> serializableObject, string experiment, string ppid, int sessionNum, string dataName, UXFDataType dataType, int optionalTrialNumber = 0);
        public abstract string HandleJSONSerializableObject(object serializableObject, string experiment, string ppid, int sessionNum, string dataName, UXFDataType dataType, int optionalTrialNumber);
        public abstract string HandleText(string text, string experiment, string ppid, int sessionNum, string dataName, UXFDataType dataType, int optionalTrialNumber = 0);
        public abstract string HandleBytes(byte[] bytes, string experiment, string ppid, int sessionNum, string dataName, UXFDataType dataType, int optionalTrialNumber = 0);
        public virtual void CleanUp() { }

# if UNITY_EDITOR
        /// <summary>
        /// Returns true if this data handler is definitley compatible with this build target group.
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        public virtual bool IsCompatibleWith(UnityEditor.BuildTargetGroup buildTarget) { return false; }

         /// <summary>
        /// Returns true if this data handler is definitley incompatible with this build target group.
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        public virtual bool IsIncompatibleWith(UnityEditor.BuildTargetGroup buildTarget) { return false; }
# endif

    }

    public enum UXFDataType
    {
        TrialResults, SessionLog, Settings, ParticipantDetails, AdditionalTrackers, SummaryStatistics, OtherTrialData, OtherSessionData, PBMandatory, PBAdditionalTrackerInfo 
    }

    public enum UXFDataLevel
    {
        PerTrial, PerSession
    }


    public static class DataHandlerEnumExtensions
    {
        public static string GetFolderName(this UXFDataType dt)
        {
            switch (dt)
            {
                case UXFDataType.TrialResults:
                    return "";
                case UXFDataType.SessionLog:
                case UXFDataType.Settings:
                case UXFDataType.ParticipantDetails:
                case UXFDataType.SummaryStatistics:
                    return "session_info";
                case UXFDataType.AdditionalTrackers:
                    if (Session.instance)
                    {
                        return System.IO.Path.Combine(string.Format("Trial_{0}", Session.instance.currentTrialNum), UPBS.Constants.ADDITIONAL_DATA_DIR);
                    }
                    return UPBS.Constants.ADDITIONAL_DATA_DIR;
                case UXFDataType.PBMandatory:
                    if (Session.instance)
                    {
                        return System.IO.Path.Combine(string.Format("Trial_{0}", Session.instance.currentTrialNum), UPBS.Constants.MANDATORY_DATA_DIR);
                    }
                    return UPBS.Constants.MANDATORY_DATA_DIR;
                case UXFDataType.PBAdditionalTrackerInfo:
                    if (Session.instance)
                    {
                        return System.IO.Path.Combine(string.Format("Trial_{0}", Session.instance.currentTrialNum), UPBS.Constants.ADDITIONAL_DATA_DIR);
                    }
                    return UPBS.Constants.ADDITIONAL_DATA_DIR;
                default:
                    return "Other";
            }
        }

        static Dictionary<UXFDataType, UXFDataLevel> typeLevelMapping = new Dictionary<UXFDataType, UXFDataLevel>
        {
            { UXFDataType.TrialResults, UXFDataLevel.PerSession },
            { UXFDataType.SessionLog, UXFDataLevel.PerSession },
            { UXFDataType.Settings, UXFDataLevel.PerSession },
            { UXFDataType.ParticipantDetails, UXFDataLevel.PerSession },
            { UXFDataType.SummaryStatistics, UXFDataLevel.PerSession },
            { UXFDataType.OtherSessionData, UXFDataLevel.PerSession },
            { UXFDataType.AdditionalTrackers, UXFDataLevel.PerTrial },
            { UXFDataType.OtherTrialData, UXFDataLevel.PerTrial },
            { UXFDataType.PBMandatory, UXFDataLevel.PerTrial },
            { UXFDataType.PBAdditionalTrackerInfo, UXFDataLevel.PerTrial },
        };

        public static UXFDataLevel GetDataLevel(this UXFDataType dt)
        {
            return typeLevelMapping[dt];
        }

        public static IEnumerable<UXFDataType> GetValidDataTypes(this UXFDataLevel level)
        {
            return typeLevelMapping
                .Where(kvp => kvp.Value == level)
                .Select(kvp => kvp.Key);
        }
    }

    public interface IDataAssociatable
    {
        void SaveDataTable(UXFDataTable table, string dataName, UXFDataType dataType);
        void SaveJSONSerializableObject(List<object> serializableObject, string dataName, UXFDataType dataType);
        void SaveJSONSerializableObject(Dictionary<string, object> serializableObject, string dataName, UXFDataType dataType);
        void SaveText(string text, string dataName, UXFDataType dataType);
        void SaveBytes(byte[] bytes, string dataName, UXFDataType dataType);
        bool CheckDataTypeIsValid(string dataName, UXFDataType dataType); 
    }


}
