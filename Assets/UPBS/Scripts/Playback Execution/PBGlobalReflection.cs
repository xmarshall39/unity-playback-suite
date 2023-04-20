using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPBS.Execution
{
    public class PBGlobalReflection : UPBS.Execution.PBReflection
    {
        public override void Refresh()
        {
            if (PBFrameLibraryManager.Instance.TryGetCurrentLibraryEntry<Data.PBGlobalFrameData>(trackerID.ID, out var frameData, name))
            {

            }

        }
    }
}