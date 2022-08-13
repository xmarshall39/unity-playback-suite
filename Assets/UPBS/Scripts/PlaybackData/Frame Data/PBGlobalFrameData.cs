using UPBS.Utility;
using UnityEngine;
namespace UPBS.Data
{
    public class PBGlobalFrameData : PBFrameDataBase
    {
        public int FPS;

        public PBGlobalFrameData() : base()
        {

        }

        public PBGlobalFrameData(PBGlobalFrameData other) : base(other)
        {
            FPS = other.FPS;
        }
        protected override bool ParseRowInternal(PBFrameParser parser, string[] row, int rowNumber)
        {
            bool allClear = base.ParseRowInternal(parser, row, rowNumber);
            if (float.TryParse(parser.GetColumnValue("FPS", row, rowNumber), out float temp))
            {
                FPS = (int)temp;
            }
            else
            {
                allClear = false;
                Debug.LogWarning($"FPS value in row {rowNumber} could not be parsed!");
            }
            return allClear;
        }
        
        public override string[] GetClassHeader()
        {
            return base.GetClassHeader().Concat
            (
                new string[]
                {
                    "FPS"
                }
            );
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
                    FPS.ToString(),
                }
            );
        }

        public override string[] GetVariableNullValuesDisplay()
        {
            return new string[] { "0" };
        }
    }

    /// <summary>
    /// Put this custom class in the examples folder for use during the sample experiment
    /// </summary>
    public class PBGlobalFrameDataCustom : PBGlobalFrameData
    {
        public PBGlobalFrameDataCustom(PBGlobalFrameDataCustom other) : base(other)
        {
            
        }
        public override string[] GetVariableErrorValuesDisplay()
        {
            return base.GetVariableErrorValuesDisplay();
        }

        public override string[] GetVariableValuesDisplay()
        {
            return base.GetVariableValuesDisplay();
        }

        public override string[] GetVariableNullValuesDisplay()
        {
            return base.GetVariableNullValuesDisplay();
        }
    }
}

