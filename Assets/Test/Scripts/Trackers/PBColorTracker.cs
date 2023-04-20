using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace UPBS.Data
{
    public class PBColorTracker : UPBSTracker
    {
        public override PBFrameDataBase FrameDataType => new PBTrackerFrameData();
        public override Type ReflectionType => typeof(UPBS.Execution.PBColorReflection);

        public override string MeasurementDescriptor => base.MeasurementDescriptor + UPBS.Constants.UPBS_COL_DESC;
        public override UXFDataType UXFDType => UXFDataType.AdditionalTrackers;

        protected override void Start()
        {
            base.Start();
            if (Session.instance)
            {
                Session.instance.trackedObjects.Add(this);
            }
        }

        protected override UXFDataRow GetCurrentValues()
        {
            var row = base.GetCurrentValues();
            row.AddRange
                (
                new List<(string, object)>()
                {
                    ("WorldPosition_x", transform.position.x),
                    ("WorldPosition_y", transform.position.y),
                    ("WorldPosition_z", transform.position.z),

                    ("EulerRotation_x", transform.eulerAngles.x),
                    ("EulerRotation_y", transform.eulerAngles.y),
                    ("EulerRotation_z", transform.eulerAngles.z),

                    ("Color_r", transform.eulerAngles.x),
                    ("Color_g", transform.eulerAngles.y),
                    ("Color_b", transform.eulerAngles.z),
                    ("Color_a", transform.eulerAngles.x)

                }
                );
            return row;
        }
    }
}

