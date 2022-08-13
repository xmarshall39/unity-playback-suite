using System.Collections.Generic;
using UXF;
using UnityEngine;
using UPBS.Execution;
namespace UPBS.Data
{
    [RequireComponent(typeof(PBTrackerID))]
    public abstract class UPBSTracker : Tracker
    {
        public abstract PBFrameDataBase FrameDataType { get; }
        public virtual void Start()
        {
            //Check for singleton validity
            if (PBGlobalTimestamp.Instance == null)
            {
                Debug.LogError("PBGlobalTimestamp Instance not found!");
            }
        }
        public override IEnumerable<string> CustomHeader => FrameDataType.GetClassHeader();
        public override string MeasurementDescriptor => UPBS.Constants.UPBS_TRACKER_DESC + "_";
        protected override UXFDataRow GetCurrentValues()
        {
            return new UXFDataRow() 
            {
                ("Timestamp", PBGlobalTimestamp.Instance.Timestamp)
            };
        }
    }
}

