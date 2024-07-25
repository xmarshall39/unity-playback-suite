using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace UPBS.Data
{
    public class PBPositionRotationTracker : UPBSTracker
    {
        public override PBFrameDataBase FrameDataType => new PBPositionRotationFrameData();
        public override Type ReflectionType => typeof(UPBS.Player.PBPositionRotationReflection);

        public override string MeasurementDescriptor => base.MeasurementDescriptor + UPBS.Constants.UPBS_POS_ROT_DESC;
        public override UXFDataType UXFDType => UXFDataType.AdditionalTrackers;

        protected override void Start()
        {
            base.Start();
            if (Session.instance && !Session.instance.trackedObjects.Contains(this))
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
                    ("EulerRotation_z", transform.eulerAngles.z)
                }
                );
            return row;
        }
    }
}

