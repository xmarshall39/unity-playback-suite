using UnityEngine;
using System.Collections.Generic;
using UPBS.Utility;
namespace UPBS.Data
{
    public class PBTrackerFrameData : PBFrameDataBase
    {
        public Vector3 WorldPosition { get; protected set; } = Vector3.zero;
        public Vector3 EulerRotation { get; protected set; } = Vector3.zero;

        public PBTrackerFrameData() : base()
        {

        }

        public PBTrackerFrameData(PBTrackerFrameData other) : base(other)
        {
            this.WorldPosition = other.WorldPosition;
            this.EulerRotation = other.EulerRotation;
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

            return allClear;
        }

        public override string[] GetClassHeader()
        {
            return base.GetClassHeader().Concat(
                WorldPosition.Header("WorldPosition"),
                EulerRotation.Header("EulerRotation"));
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
}

