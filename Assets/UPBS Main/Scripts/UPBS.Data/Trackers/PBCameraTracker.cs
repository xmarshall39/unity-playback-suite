using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Player;
using UXF;
namespace UPBS.Data
{
    public class PBCameraTracker : UPBSTracker
    {
        public override PBFrameDataBase FrameDataType { get => new PBCameraFrameData(); }
        public override System.Type ReflectionType => typeof(PBCameraReflection);

        public override string MeasurementDescriptor => base.MeasurementDescriptor + UPBS.Constants.UPBS_CAMERA_DESC;
        public override UXFDataType UXFDType => UXFDataType.PBMandatory;

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
            Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
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

                    ("TransformMatrix_0_0", worldToLocal.m00),
                    ("TransformMatrix_0_1", worldToLocal.m01),
                    ("TransformMatrix_0_2", worldToLocal.m02),
                    ("TransformMatrix_0_3", worldToLocal.m03),

                    ("TransformMatrix_1_0", worldToLocal.m10),
                    ("TransformMatrix_1_1", worldToLocal.m11),
                    ("TransformMatrix_1_2", worldToLocal.m12),
                    ("TransformMatrix_1_3", worldToLocal.m13),

                    ("TransformMatrix_2_0", worldToLocal.m20),
                    ("TransformMatrix_2_1", worldToLocal.m21),
                    ("TransformMatrix_2_2", worldToLocal.m22),
                    ("TransformMatrix_2_3", worldToLocal.m23),

                    ("TransformMatrix_3_0", worldToLocal.m30),
                    ("TransformMatrix_3_1", worldToLocal.m31),
                    ("TransformMatrix_3_2", worldToLocal.m32),
                    ("TransformMatrix_3_3", worldToLocal.m33),
                }
                );
            return row;
        }
    }
}

