using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
using UPBS.Player;

namespace UPBS.Examples
{
    public class SampleDataPostProcessor : PBPostProcessor_DataBase
    {
        public override string GenerateTrackedObjectData(PBTrackerID trackedObject, PBFrameDataBase frameData)
        {
            var casted = frameData as PBPositionRotationFrameData;
            if (casted != null)
            {
                return casted.WorldPosition.sqrMagnitude.ToString();
            }

            return string.Empty;
        }
    }

}