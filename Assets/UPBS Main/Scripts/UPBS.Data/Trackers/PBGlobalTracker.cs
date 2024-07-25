using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Player;
using UXF;

namespace UPBS.Data
{
    public class PBGlobalTracker : UPBSTracker
    {
        public override PBFrameDataBase FrameDataType { get => new PBGlobalFrameData(); }
        public override System.Type ReflectionType => typeof(PBGlobalReflection);

        public override UXFDataType UXFDType => UXFDataType.PBMandatory;
        public override string MeasurementDescriptor => base.MeasurementDescriptor + UPBS.Constants.UPBS_GLOBAL_DESC;

        protected override void Start()
        {
            base.Start();
            if (PBGlobalFrameRate.Instance == null)
            {
                Debug.LogError("PBGlobalFrameRate Instance not found!");
            }

            if (Session.instance && !Session.instance.trackedObjects.Contains(this))
            {
                Session.instance.trackedObjects.Add(this);
                print("Added PBGlobalTracker to tracked objects!");
            }
        }

        protected override UXFDataRow GetCurrentValues()
        {
            var row = base.GetCurrentValues();
            row.AddRange
            (
                new List<(string, object)>() 
                { 
                    ("FPS", PBGlobalFrameRate.Instance.FrameRate)
                }

            );

            return row;
        }
        
    }
}

