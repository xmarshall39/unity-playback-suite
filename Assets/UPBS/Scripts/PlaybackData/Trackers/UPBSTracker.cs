using System.Collections.Generic;
using UXF;
using UnityEngine;
using UPBS.Execution;
namespace UPBS.Data
{
    [RequireComponent(typeof(PBTrackerID))]
    public abstract class UPBSTracker : Tracker
    {
        private int TID = -1;

        public abstract PBFrameDataBase FrameDataType { get; }
        public PBTrackerID TrackerID { get; private set; }

        public override IEnumerable<string> CustomHeader => FrameDataType.GetClassHeader();
        public override string MeasurementDescriptor => TID.ToString() + "_" + UPBS.Constants.UPBS_TRACKER_DESC + "_";
        public override string DataName
        {
            get
            {
                Debug.AssertFormat(MeasurementDescriptor.Length > 0, "No measurement descriptor has been specified for this Tracker!");
                return string.Join("_", new string[]{ MeasurementDescriptor, objectName});
            }
            
        }

        protected void OnValidate()
        {
            RefreshTID();
        }

        protected virtual void Start()
        {
            if (Application.isPlaying)
            {
                TrackerID = GetComponent<PBTrackerID>();
                if (TrackerID == null)
                {
                    Debug.LogError($"Tracker attached to {gameObject.name} cannot be found!");
                }

                //Check for singleton validity
                if (PBGlobalTimestamp.Instance == null)
                {
                    Debug.LogError("PBGlobalTimestamp Instance not found!");
                }
            }
            
        }

        protected override UXFDataRow GetCurrentValues()
        {
            return new UXFDataRow() 
            {
                ("Timestamp", PBGlobalTimestamp.Instance.Timestamp)
            };
        }

        public void RefreshTID()
        {
            if (TrackerID == null)
            {
                if (TryGetComponent(out PBTrackerID temp))
                {
                    TrackerID = temp;
                    TID = temp.ID;
                }
            }

            else
            {
                TID = TrackerID.ID;
            }
        }
    }
}

