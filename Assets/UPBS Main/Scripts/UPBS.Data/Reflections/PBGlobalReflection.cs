using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Player
{
    public class PBGlobalReflection : UPBS.Player.PBReflection
    {
        public override void Refresh()
        {
            if (PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBGlobalFrameData>(trackerID.ID, out var frameData, name))
            {

            }

        }
    }
}