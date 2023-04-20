using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Execution
{
    public class PBColorReflection : UPBS.Execution.PBReflection
    {
        public override void Refresh()
        {
            if(PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBColorFrameData>(trackerID.ID, out var frameData, name))
            {
                transform.position = frameData.WorldPosition;
                transform.eulerAngles = frameData.EulerRotation;
            }
        }
    }
}
