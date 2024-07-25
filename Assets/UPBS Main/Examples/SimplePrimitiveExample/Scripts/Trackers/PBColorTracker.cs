using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UXF;

namespace UPBS.Data
{
    public class PBColorTracker : UPBSTracker
    {
        public Material mat;
        public string colorKey = "_Tint Color";
        public override PBFrameDataBase FrameDataType => new PBColorFrameData();
        public override Type ReflectionType => typeof(UPBS.Player.PBColorReflection);

        public override string MeasurementDescriptor => base.MeasurementDescriptor + UPBS.Constants.UPBS_COL_DESC;
        public override UXFDataType UXFDType => UXFDataType.AdditionalTrackers;
        
        protected override void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
            base.Start();
            if (Session.instance)
            {
                Session.instance.trackedObjects.Add(this);
            }
        }

        protected override UXFDataRow GetCurrentValues()
        {
            Color col = mat.GetColor(colorKey);
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

                    ("Color_r", col.r),
                    ("Color_g", col.g),
                    ("Color_b", col.b),
                    ("Color_a", col.a)

                }
                );
            return row;
        }
    }
}

