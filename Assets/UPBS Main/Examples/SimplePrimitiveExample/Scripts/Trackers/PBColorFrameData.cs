using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Utility;

namespace UPBS.Data
{
    public class PBColorFrameData : PBPositionRotationFrameData
    {
        public Color MaterialColor { get; protected set; } = Color.white;

        public PBColorFrameData() : base()
        {

        }

        public PBColorFrameData(PBColorFrameData other) : base(other)
        {
            MaterialColor = other.MaterialColor;
        }

        protected override bool ParseRowInternal(PBFrameParser parser, string[] row, int rowNumber)
        {
            bool allClear = base.ParseRowInternal(parser, row, rowNumber);

            if (parser.GetColumnValuesAsFloats("Color", row, rowNumber, out float[] vals, MaterialColor.HeaderAppends()))
            {
                MaterialColor = new Color(vals[0], vals[1], vals[2], vals[3]);
            }

            else
            {
                allClear = false;
                Debug.LogWarning($"Color value in row {rowNumber} could not be parsed!");
            }

            return allClear;
        }

        public override string[] GetClassHeader()
        {
            return base.GetClassHeader().Concat(
                MaterialColor.Header("Color"));
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

