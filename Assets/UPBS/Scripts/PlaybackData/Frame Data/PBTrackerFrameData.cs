using UnityEngine;
using System.Collections.Generic;
using UPBS.Utility;
namespace UPBS.Data
{
    public class PBTrackerFrameData : PBFrameDataBase
    {
        public Matrix4x4 transformMatrix;
        public Vector3 position;
        public Vector3 eulerRotation;

        public PBTrackerFrameData(PBTrackerFrameData other) : base(other)
        {
            this.transformMatrix = other.transformMatrix;
            this.position = other.position;
            this.eulerRotation = other.eulerRotation;
        }

        public override string[] GetClassHeader()
        {
            return HelperFunctions.ConcatArrays
            (
                base.GetClassHeader(),
                transformMatrix.Header("TransformationMatrix"),
                position.Header("Position"),
                eulerRotation.Header("EulerRotation")
            );
        }

        public override string[] GetVariableValuesDisplay()
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetVariableNullValuesDisplay()
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetVariableErrorValuesDisplay()
        {
            throw new System.NotImplementedException();
        }
    }
    /*
    /// <summary>
    /// Move this to a file for examples and use in a later experiment with custom data fields
    /// </summary>
    public class PBFTrackerFrameDataCustom : PBTrackerFrameData
    {
        public Vector4 inputAxes;
        public override string[] GetVariableNames()
        {
            return base.GetVariableNames();
        }
    }
    */
}

