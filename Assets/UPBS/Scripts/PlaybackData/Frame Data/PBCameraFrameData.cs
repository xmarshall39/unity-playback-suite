using UnityEngine;
using UPBS.Utility;
namespace UPBS.Data
{
    public class PBCameraFrameData : PBFrameDataBase
    {
        public Matrix4x4 TransformMatrix { get; protected set; } = Matrix4x4.identity;
        public Vector3 WorldPosition { get; protected set; } = Vector3.zero;
        public Vector3 EulerRotation { get; protected set; } = Vector3.zero;

        public PBCameraFrameData() : base()
        {

        }

        public PBCameraFrameData(PBCameraFrameData other) : base(other)
        {
            TransformMatrix = other.TransformMatrix;
            WorldPosition = other.WorldPosition;
            EulerRotation = other.EulerRotation;
        }

        protected override bool ParseRowInternal(PBFrameParser parser, string[] row, int rowNumber)
        {
            bool allClear = base.ParseRowInternal(parser, row, rowNumber);
            if (parser.GetColumnValuesAsFloats("WorldPosition", row, rowNumber, out float[] vals, WorldPosition.HeaderAppends()))
            {
                WorldPosition = new Vector3(vals[0], vals[1], vals[2]);
            }

            else
            {
                allClear = false;
                Debug.LogWarning($"WorldPosition value in row {rowNumber} could not be parsed!");
            }

            if (parser.GetColumnValuesAsFloats("EulerRotation", row, rowNumber, out vals, EulerRotation.HeaderAppends()))
            {
                EulerRotation = new Vector3(vals[0], vals[1], vals[2]);
            }

            else
            {
                allClear = false;
                Debug.LogWarning($"EulerRotation value in row {rowNumber} could not be parsed!");
            }

            if (parser.GetColumnValuesAsFloats("TransformMatrix", row, rowNumber, out vals, TransformMatrix.HeaderAppends()))
            {
                TransformMatrix = new Matrix4x4
                    (
                    new Vector4(vals[0], vals[1], vals[2], vals[3]),
                    new Vector4(vals[4], vals[5], vals[6], vals[7]),
                    new Vector4(vals[8], vals[9], vals[10], vals[11]),
                    new Vector4(vals[12], vals[13], vals[14], vals[15])
                    );
            }

            else
            {
                allClear = false;
                Debug.LogWarning($"EulerRotation value in row {rowNumber} could not be parsed!");
            }

            return allClear;
        }

        public override string[] GetClassHeader()
        {
            return base.GetClassHeader().Concat(
                TransformMatrix.Header("TransformMatrix"),
                WorldPosition.Header("WorldPosition"),
                EulerRotation.Header("EulerRotation"));
        }

        public override string[] GetVariableNameDisplay()
        {
            return base.GetVariableNameDisplay().Concat
            (
                new string[]
                {
                    "FPS"
                }
            );
        }
        public override string[] GetVariableErrorValuesDisplay()
        {
            return base.GetVariableErrorValuesDisplay().Concat
            (
                new string[]
                {
                    float.NaN.ToString(),
                }
            );
        }

        public override string[] GetVariableValuesDisplay()
        {

            return base.GetVariableValuesDisplay().Concat
            (
                new string[]
                {
                    WorldPosition.ToString(),
                }
            );
        }

        public override string[] GetVariableNullValuesDisplay()
        {
            return new string[] { "0" };
        }
    }
}

