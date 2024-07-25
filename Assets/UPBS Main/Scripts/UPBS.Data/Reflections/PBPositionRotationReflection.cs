using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Player
{
    public class PBPositionRotationReflection : UPBS.Player.PBReflection
    {
        public override void Refresh()
        {
            base.Refresh();
            if (PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBPositionRotationFrameData>(trackerID.ID, out var frameData, name))
            {
                transform.position = frameData.WorldPosition;
                transform.eulerAngles = frameData.EulerRotation;
            }
        }
    }
}
