using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Execution
{
    public class PBPositionRotationReflection : UPBS.Execution.PBReflection
    {
        public override void Refresh()
        {
            if (PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBTrackerFrameData>(trackerID.ID, out var frameData, name))
            {
                transform.position = frameData.WorldPosition;
                transform.eulerAngles = frameData.EulerRotation;
            }
        }
    }
}
