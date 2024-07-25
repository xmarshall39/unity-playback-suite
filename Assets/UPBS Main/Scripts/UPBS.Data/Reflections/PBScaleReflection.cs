using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Player
{
    public class PBScaleReflection : UPBS.Player.PBReflection
    {
        public override void Refresh()
        {
            base.Refresh();
            if (PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBScaleFrameData>(trackerID.ID, out var frameData, name))
            {
                transform.position = frameData.WorldPosition;
                transform.eulerAngles = frameData.EulerRotation;
                transform.localScale = frameData.LocalScale;
            }
            
        }
    }
}

