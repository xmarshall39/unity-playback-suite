using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace UPBS.Data
{
    public class PBScaleTracker : UPBSTracker
    {
        public override PBFrameDataBase FrameDataType => new PBScaleFrameData();
        public override Type ReflectionType => typeof(UPBS.Player.PBScaleReflection);

        public override string MeasurementDescriptor => base.MeasurementDescriptor + UPBS.Constants.UPBS_SCALE_DESC;
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

                    ("LocalScale_x", transform.localScale.x),
                    ("LocalScale_y", transform.localScale.y),
                    ("LocalScale_z", transform.localScale.z)

                }
                );
            return row;
        }
    }
}

