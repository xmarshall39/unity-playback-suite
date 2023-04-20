using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Execution
{
    public class PBScaleReflection : UPBS.Execution.PBReflection
    {
        public override void Refresh()
        {
            if (PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBScaleFrameData>(trackerID.ID, out var frameData, name))
            {
                transform.position = frameData.WorldPosition;
                transform.eulerAngles = frameData.EulerRotation;
                transform.localScale = frameData.LocalScale;
            }
            
        }
    }
}

