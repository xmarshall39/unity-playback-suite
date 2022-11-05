using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Execution
{
    public class PBPositionRotationReflection : UPBS.Execution.PBReflection
    {
        public override void Refresh()
        {
            var frameData = PBFrameLibraryManager.Instance.GetCurrentLibraryEntry(trackerID.ID) as Data.PBTrackerFrameData;
            transform.position = frameData.WorldPosition;
            transform.eulerAngles = frameData.EulerRotation;

        }
    }
}
